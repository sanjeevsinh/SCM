using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using SCM.Models.ServiceModels;
using SCM.Models.NetModels.AttachmentNetModels;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace SCM.Services.SCMServices
{
    public class AttachmentService : BaseService, IAttachmentService
    {

        public AttachmentService(IUnitOfWork unitOfWork, IMapper mapper, INetworkSyncService netSync) : base(unitOfWork, mapper, netSync)
        {
        }

        public async Task<Attachment> GetByIDAsync(int id)
        {
            var dbResult = await UnitOfWork.InterfaceRepository.GetAsync(q => q.ID == id, includeProperties:
                "Port.Device.Location,Port.Device.Plane,Vrf.BgpPeers,InterfaceBandwidth,ContractBandwidthPool.ContractBandwidth",
                AsTrackable: false);
            return Mapper.Map<Attachment>(dbResult.SingleOrDefault());
        }

        public async Task<List<Attachment>> GetAllByTenantAsync(Tenant tenant)
        {
            var ifaces = await UnitOfWork.InterfaceRepository.GetAsync(q => q.Port.TenantID == tenant.TenantID,
                includeProperties: "Port.Device.Location.SubRegion.Region,Port.Device.Plane,"
                + "Port.Interface.Vrf,Port.Interface.InterfaceBandwidth,Port.Interface.ContractBandwidthPool", AsTrackable: false);

            return Mapper.Map<List<Attachment>>(ifaces);
        }

        public async Task<ServiceResult> AddAsync(AttachmentRequest attachmentRequest)
        {
            var serviceResult = new ServiceResult();
            serviceResult.IsSuccess = true;

            // Get all devices in the requested location

            var devices = await UnitOfWork.DeviceRepository.GetAsync(q => q.LocationID == attachmentRequest.LocationID,
                includeProperties: "Ports.PortBandwidth");

            // Filter devices collection to include only those devices which belong to the requested plane (if specified)

            if (attachmentRequest.PlaneID != null)
            {
                devices = devices.Where(q => q.PlaneID == attachmentRequest.PlaneID).ToList();
            }

            // Filter devices collection to only those devices which have free ports (ports which are not already assigned to a tenant) whcih are
            // of the requested bandwidth

            var bandwidth = await UnitOfWork.InterfaceBandwidthRepository.GetByIDAsync(attachmentRequest.BandwidthID);
            devices = devices.Where(q => q.Ports.Where(p => p.TenantID == null && p.PortBandwidth.BandwidthGbps == bandwidth.BandwidthGbps).Count() > 0).ToList();

            Device device = null;

            if (devices.Count == 0)
            {
                serviceResult.Add("A device with a free attachment port matching the requested location and bandwidth parameters could not be found. "
                    + "Please change the input parameters and try again, or contact your system adminstrator to report this issue.");

                serviceResult.IsSuccess = false;

                return serviceResult;
            }
            else if (devices.Count > 1)
            {
                // Get device with the least number of tenant-assigned ports.

                device = devices.Aggregate((current, x) =>
                (x.Ports.Where(p => p.TenantID != null).Count() < current.Ports.Where(p => p.TenantID != null).Count() ? x : current));
            }
            else
            {
                device = devices.Single();
            }

            Port port;
            var ports = device.Ports.Where(q => q.TenantID == null && q.PortBandwidth.BandwidthGbps == bandwidth.BandwidthGbps);
            if (ports.Count() > 0)
            {
                port = ports.First();
            }
            else
            {
                serviceResult.Add("A port matching the requested bandwidth parameter could not be found. "
                    + "Please change your bandwidth request and try again, or contact your system adminstrator and report this issue.");

                serviceResult.IsSuccess = false;

                return serviceResult;
            }

            port.TenantID = attachmentRequest.TenantID;

            var iface = Mapper.Map<Interface>(attachmentRequest);
            iface.ID = port.ID;

            Vrf vrf = null;
            if (attachmentRequest.IsLayer3)
            {
                vrf = Mapper.Map<Vrf>(attachmentRequest);
                vrf.DeviceID = device.ID;
            }

            // Need to implement Transaction Scope here when available in dotnet core

            try
            {
                UnitOfWork.PortRepository.Update(port);
                if (vrf != null)
                {
                    UnitOfWork.VrfRepository.Insert(vrf);
                    await this.UnitOfWork.SaveAsync();
                    iface.VrfID = vrf.VrfID;
                }
                UnitOfWork.InterfaceRepository.Insert(iface);
                await this.UnitOfWork.SaveAsync();
            }

            catch (Exception /** ex **/)
            {
                // Add logging for the exception here
                serviceResult.Add("Something went wrong during the database update. The issue has been logged."
                   + "Please try again, and contact your system admin if the problem persists.");
                serviceResult.IsSuccess = false;

                return serviceResult;
            }

            return serviceResult;
        }

        public async Task<ServiceResult> DeleteAsync(Attachment attachment)
        {

            var result = new ServiceResult();
            result.IsSuccess = true;

            // Check for VPN Attachment Sets - if a VRF for the Attachment exists, which 
            // is to be deleted, and one or more VPNs are bound to the  VRF, 
            // then quit and warn the user

            if (attachment.VrfID != null)
            {
                var checkVpnAttachmentSetsResult = await CheckVpnAttachmentSets(attachment.VrfID.Value);
                if (!checkVpnAttachmentSetsResult.IsSuccess)
                {
                    return checkVpnAttachmentSetsResult;
                }
            }

            // Delete from the network first

            var syncResult = await DeleteFromNetworkAsync(attachment.ID);

            if (!syncResult.IsSuccess && syncResult.NetworkHttpResponse.HttpStatusCode != HttpStatusCode.NotFound)
            {
                result.IsSuccess = false;
                result.Add(syncResult.GetAllMessages());
                return result;
            }

            // Now perform inventory updates

            var port = await UnitOfWork.PortRepository.GetByIDAsync(attachment.ID);
            port.TenantID = null;

            try
            {

                UnitOfWork.PortRepository.Update(port);
                await UnitOfWork.InterfaceRepository.DeleteAsync(attachment.ID);
                if (attachment.VrfID != null)
                {
                    await UnitOfWork.VrfRepository.DeleteAsync(attachment.VrfID);
                }

                await UnitOfWork.SaveAsync();
            }

            catch (DbUpdateException  /** ex **/ )
            {
                result.Add("The delete operation failed. The error has been logged. "
                   + "Please try again and contact your system administrator if the problem persists.");

                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<NetworkCheckSyncServiceResult> CheckNetworkSyncAsync(int id)
        {
            var attachment = await GetByIDAsync(id);

            if (attachment.IsTagged)
            {
                var taggedAttachmentServiceModelData = Mapper.Map<TaggedAttachmentInterfaceServiceNetModel>(attachment);
                var attachmentCheckSyncResult = await NetSync.CheckNetworkSyncAsync(taggedAttachmentServiceModelData,
                    "/attachment/pe/" + attachment.Device.Name
                    + "/tagged-attachment-interface/"
                    + taggedAttachmentServiceModelData.InterfaceType + ","
                    + taggedAttachmentServiceModelData.InterfaceID.Replace("/", "%2F"));

                return attachmentCheckSyncResult;
            }
            else
            {
                if (attachment.IsLayer3)
                {
                    var vrfServiceModelData = Mapper.Map<VrfServiceNetModel>(attachment);
                    var vrfCheckSyncResult = await NetSync.CheckNetworkSyncAsync(vrfServiceModelData,
                        "/attachment/pe/" + attachment.Device.Name + "/vrf/" + vrfServiceModelData.VrfName);
                    if (!vrfCheckSyncResult.InSync)
                    {
                        return vrfCheckSyncResult;
                    }
                }

                var untaggedAttachmentServiceModelData = Mapper.Map<UntaggedAttachmentInterfaceServiceNetModel>(attachment);
                var attachmentCheckSyncResult = await NetSync.CheckNetworkSyncAsync(untaggedAttachmentServiceModelData,
                    "/attachment/pe/" + attachment.Device.Name
                    + "/untagged-attachment-interface/"
                    + untaggedAttachmentServiceModelData.InterfaceType + ","
                    + untaggedAttachmentServiceModelData.InterfaceID.Replace("/", "%2F"));

                return attachmentCheckSyncResult;
            }
        }
        public async Task<NetworkSyncServiceResult> SyncToNetworkAsync(int attachmentID)
        {
            var attachment = await GetByIDAsync(attachmentID);

            if (attachment.IsTagged)
            {
                var taggedAttachmentServiceModelData = Mapper.Map<TaggedAttachmentInterfaceServiceNetModel>(attachment);
                var attachmentSyncResult = await NetSync.SyncNetworkAsync(taggedAttachmentServiceModelData,
                    "/attachment/pe/" + attachment.Device.Name
                    + "/tagged-attachment-interface/"
                    + taggedAttachmentServiceModelData.InterfaceType + ","
                    + taggedAttachmentServiceModelData.InterfaceID.Replace("/", "%2F"));

                return attachmentSyncResult;
            }
            else
            {
                if (attachment.IsLayer3)
                {
                    var vrfServiceModelData = Mapper.Map<VrfServiceNetModel>(attachment);
                    var vrfSyncResult = await NetSync.SyncNetworkAsync(vrfServiceModelData,
                        "/attachment/pe/"
                        + attachment.Device.Name + "/vrf/"
                        + vrfServiceModelData.VrfName);

                    if (!vrfSyncResult.IsSuccess)
                    {
                        return vrfSyncResult;
                    }
                }

                var untaggedAttachmentServiceModelData = Mapper.Map<UntaggedAttachmentInterfaceServiceNetModel>(attachment);
                var attachmentSyncResult = await NetSync.SyncNetworkAsync(untaggedAttachmentServiceModelData,
                    "/attachment/pe/" + attachment.Device.Name
                    + "/untagged-attachment-interface/"
                    + untaggedAttachmentServiceModelData.InterfaceType + ","
                    + untaggedAttachmentServiceModelData.InterfaceID.Replace("/", "%2F"));

                return attachmentSyncResult;
            }
        }

        public async Task<NetworkSyncServiceResult> DeleteFromNetworkAsync(int attachmentID)
        {
            var attachment = await GetByIDAsync(attachmentID);
            var syncResult = new NetworkSyncServiceResult();

            if (attachment == null)
            {
                syncResult.Add("The attachment was not found.");

                return syncResult;
            }

            // Check for VPN Attachment Sets - if a VRF for the Attachment exists, which 
            // is to be deleted, and one or more VPNs are bound to the  VRF, 
            // then quit and warn the user

            if (attachment.VrfID != null)
            {
                var checkVpnAttachmentSetsResult = await CheckVpnAttachmentSets(attachment.VrfID.Value);
                if (!checkVpnAttachmentSetsResult.IsSuccess)
                {
                    syncResult.Add(checkVpnAttachmentSetsResult.GetMessage());

                    return syncResult;
                }
            }

            if (attachment.IsTagged)
            {
                var taggedAttachmentServiceModelData = Mapper.Map<TaggedAttachmentInterfaceServiceNetModel>(attachment);
                var attachmentSyncResult = await NetSync.DeleteFromNetworkAsync("/attachment/pe/"
                    + attachment.Device.Name
                    + "/tagged-attachment-interface/"
                    + taggedAttachmentServiceModelData.InterfaceType + ","
                    + taggedAttachmentServiceModelData.InterfaceID.Replace("/", "%2F"));

                return attachmentSyncResult;
            }
            else
            {
                if (attachment.IsLayer3)
                {
                    var vrfServiceModelData = Mapper.Map<VrfServiceNetModel>(attachment);
                    var vrfSyncResult = await NetSync.DeleteFromNetworkAsync("/attachment/pe/"
                        + attachment.Device.Name
                        + "/vrf/" + vrfServiceModelData.VrfName);

                    if (!vrfSyncResult.IsSuccess)
                    {
                        return vrfSyncResult;
                    }
                }

                var untaggedAttachmentServiceModelData = Mapper.Map<UntaggedAttachmentInterfaceServiceNetModel>(attachment);
                var attachmentSyncResult = await NetSync.DeleteFromNetworkAsync("/attachment/pe/"
                    + attachment.Device.Name
                    + "/untagged-attachment-interface/"
                    + untaggedAttachmentServiceModelData.InterfaceType + ","
                    + untaggedAttachmentServiceModelData.InterfaceID.Replace("/", "%2F"));

                return attachmentSyncResult;
            }
        }

        public async Task<ServiceResult> ValidateAttachmentRequest(AttachmentRequest request)
        {
            var result = new ServiceResult();
            result.IsSuccess = true;

            var dbResult = await UnitOfWork.ContractBandwidthPoolRepository.GetAsync(q => 
                q.ContractBandwidthPoolID == request.ContractBandwidthPoolID, includeProperties:"Interfaces,BundleInterface");

            var contractBandwidthPool = dbResult.Single();

            if (contractBandwidthPool.Interfaces.Count > 0 )
            {
                var port = contractBandwidthPool.Interfaces.Single().Port;
                result.Add($"The Contract Bandwidth Pool is in-use for interface {port.Type} {port.Name}");
                result.IsSuccess = false;    
            }

            return result;
        }

        /// <summary>
        /// Check if one or more VPNs are bound to a VRF which belongs to an Attachment Set
        /// </summary>
        /// <param name="vrfID"></param>
        /// <returns></returns>
        private async Task<ServiceResult> CheckVpnAttachmentSets(int vrfID) 
        {
            var result = new ServiceResult();
            result.IsSuccess = true;

            var vpnAttachmentSets = await UnitOfWork.VpnAttachmentSetRepository.GetAsync(q => q.AttachmentSet.AttachmentSetVrfs
                    .Where(v => v.VrfID == vrfID).Count() > 0,
                    includeProperties: "Vpn");

            if (vpnAttachmentSets.Count() > 0)
            {
                result.Add("This attachment cannot be deleted because the following VPN services are bound to the VRF: "
                    + string.Join(",", vpnAttachmentSets.Select(q => q.Vpn.Name)));

                result.IsSuccess = false;
            }

            return result;
        }
    }
}
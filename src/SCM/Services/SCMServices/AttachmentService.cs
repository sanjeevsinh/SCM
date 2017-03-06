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
        private IVrfService VrfService { get; set; }

        public AttachmentService(IUnitOfWork unitOfWork, IMapper mapper, 
            INetworkSyncService netSync, IVrfService vrfService) : base(unitOfWork, mapper, netSync)
        {
            VrfService = vrfService;
        }

        public async Task<Attachment> GetFullAsync(Attachment attachment)
        {
            if (attachment.IsBundle)
            {

            }
            else 
            {
                var dbResult = await UnitOfWork.InterfaceRepository.GetAsync(q => q.ID == attachment.ID, includeProperties:
                    "Port.Device.Location,Port.Device.Plane,Vrf.BgpPeers,InterfaceBandwidth,ContractBandwidthPool.ContractBandwidth",
                    AsTrackable: false);

                return Mapper.Map<Attachment>(dbResult.SingleOrDefault());
            }

            return null;
        }

        public async Task<List<Attachment>> GetAllByTenantAsync(Tenant tenant)
        {
            var ifaces = await UnitOfWork.InterfaceRepository.GetAsync(q => q.Port.TenantID == tenant.TenantID,
                includeProperties: "Port.Device.Location.SubRegion.Region,Port.Device.Plane,"
                + "Port.Interface.Vrf,Port.Interface.InterfaceBandwidth,Port.Interface.ContractBandwidthPool", AsTrackable: false);

            return Mapper.Map<List<Attachment>>(ifaces);
        }

        public async Task<ServiceResult> AddAsync(AttachmentRequest request)
        {
            var result = new ServiceResult { IsSuccess = true };

            var ports = await FindPorts(request, result);
            if (ports.Count() == 0)
            {
                return result;
            }

            if (request.BundleRequired)
            {
                await AddBundleAttachment(request, ports, result); 
            }
            else
            {
                await AddAttachment(request, ports.First(), result);   
            }

            return result;
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
                var vrfValidationResult = await VrfService.ValidateDelete(attachment.VrfID.Value);
                if (!vrfValidationResult.IsSuccess)
                {
                    return vrfValidationResult;
                }
            }

            // Delete from the network first

            var syncResult = await DeleteFromNetworkAsync(attachment);

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

        public async Task<NetworkCheckSyncServiceResult> CheckNetworkSyncAsync(Attachment attachment)
        {
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
        public async Task<NetworkSyncServiceResult> SyncToNetworkAsync(Attachment attachment)
        { 
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

        public async Task<NetworkSyncServiceResult> DeleteFromNetworkAsync(Attachment attachment)
        {
            var syncResult = new NetworkSyncServiceResult();

            // Check for VPN Attachment Sets - if a VRF for the Attachment exists, which 
            // is to be deleted, and one or more VPNs are bound to the  VRF, 
            // then quit and warn the user

            if (attachment.VrfID != null)
            {
                var vrfValidationResult = await VrfService.ValidateDelete(attachment.VrfID.Value);
                if (!vrfValidationResult.IsSuccess)
                {
                    syncResult.Add(vrfValidationResult.GetMessage());
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

        /// <summary>
        /// Validates an attachment request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ServiceResult> Validate(AttachmentRequest request)
        {
            var result = new ServiceResult { IsSuccess = true };

            if (request.BundleRequired)
            {
                if (!request.Bandwidth.SupportedByBundle)
                {
                    result.Add("The rquested bandwidth is not supported by a bundle.");
                    result.IsSuccess = false;
                }
            }

            if (!request.IsTagged)
            {

                var dbResult = await UnitOfWork.ContractBandwidthPoolRepository.GetAsync(q =>
                    q.ContractBandwidthPoolID == request.ContractBandwidthPoolID, includeProperties: "Interfaces.Port");

                var contractBandwidthPool = dbResult.SingleOrDefault();
                if (contractBandwidthPool == null)
                {
                    result.Add("The requested contract bandwidth pool was not found.");
                    result.IsSuccess = false;
                    return result;
                }

                if (contractBandwidthPool.Interfaces.Count > 0)
                {
                    var port = contractBandwidthPool.Interfaces.Single().Port;
                    result.Add($"The selected contract bandwidth pool is in-use for interface {port.Type} {port.Name}. Select another contract bandwidth pool.");
                    result.IsSuccess = false;
                }
            }

            return result;
        }

        private async Task<IEnumerable<Port>> FindPorts(AttachmentRequest request, ServiceResult result)
        {
            var device = await FindDevice(request, result);

            if (device == null)
            {
                return Enumerable.Empty<Port>();
            }

            var ports = device.Ports.Where(q => q.TenantID == null && q.PortBandwidth.BandwidthGbps == request.Bandwidth.BandwidthGbps);

            if (ports.Count() == 0)
            {
                result.Add("Ports matching the requested bandwidth parameter could not be found. "
                    + "Please change your request and try again, or contact your system adminstrator and report this issue.");

                result.IsSuccess = false;
            }

            return ports;
        }

        private async Task<Device> FindDevice(AttachmentRequest request, ServiceResult result)
        {
            // Find all devices in the requested location

            var devices = await UnitOfWork.DeviceRepository.GetAsync(q => q.LocationID == request.LocationID,
                includeProperties: "Ports.PortBandwidth,Ports.Device");

            // Filter devices collection to include only those devices which belong to the requested plane (if specified)

            if (request.PlaneID != null)
            {
                devices = devices.Where(q => q.PlaneID == request.PlaneID).ToList();
            }

            // Filter devices collection to only those devices which have the required number of free ports
            // of the required bandwidth.
            // Free ports are not already assigned to a tenant.

            int numPortsRequired = 1;
            int bandwidthRequired = 0;
            if (request.BundleRequired)
            {
                numPortsRequired = request.Bandwidth.BandwidthGbps / request.Bandwidth.BundleOrMultiPortMemberBandwidthGbps.Value;
                bandwidthRequired = request.Bandwidth.BandwidthGbps / numPortsRequired;
            }

            devices = devices.Where(q => q.Ports.Where(p => p.TenantID == null 
                && p.PortBandwidth.BandwidthGbps == bandwidthRequired).Count() >= numPortsRequired).ToList();

            Device device = null;

            if (devices.Count == 0)
            {
                result.Add("A device with a free attachment port matching the requested location and bandwidth parameters could not be found. "
                    + "Please change the input parameters and try again, or contact your system adminstrator to report this issue.");

                result.IsSuccess = false;

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

            return device;
        }

        private async Task AddAttachment(AttachmentRequest request, Port port, ServiceResult result)
        {
            port.TenantID = request.TenantID;

            var iface = Mapper.Map<Interface>(request);
            iface.ID = port.ID;

            Vrf vrf = null;
            if (request.IsLayer3)
            {
                vrf = Mapper.Map<Vrf>(request);
                vrf.DeviceID = port.Device.ID;
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
                result.Add("Something went wrong during the database update. The issue has been logged."
                   + "Please try again, and contact your system admin if the problem persists.");
                result.IsSuccess = false;
            }
        }

        private async Task AddBundleAttachment(AttachmentRequest request, IEnumerable<Port> ports, ServiceResult result)
        {
            Vrf vrf = null;
            if (request.IsLayer3)
            {
                vrf = Mapper.Map<Vrf>(request);
                vrf.DeviceID = ports.First().Device.ID;
            }

            var bundleIface = Mapper.Map<BundleInterface>(request);

            // Need to implement Transaction Scope here when available in dotnet core

            try
            {
                if (vrf != null)
                {
                    UnitOfWork.VrfRepository.Insert(vrf);
                    await this.UnitOfWork.SaveAsync();
                    bundleIface.VrfID = vrf.VrfID;
                }

                UnitOfWork.BundleInterfaceRepository.Insert(bundleIface);
                await this.UnitOfWork.SaveAsync();

                foreach (var port in ports)
                {
                    port.TenantID = request.TenantID;
                    UnitOfWork.PortRepository.Update(port);

                    var bundleIfacePort = new BundleInterfacePort { PortID = port.ID, BundleInterfaceID = bundleIface.BundleInterfaceID };
                }
            }

            catch (Exception /** ex **/)
            {
                // Add logging for the exception here
                result.Add("Something went wrong during the database update. The issue has been logged."
                   + "Please try again, and contact your system admin if the problem persists.");

                result.IsSuccess = false;
            }
        }
    }
}
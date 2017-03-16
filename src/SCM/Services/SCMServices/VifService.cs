using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using SCM.Models.ServiceModels;
using System.Threading.Tasks;
using AutoMapper;
using SCM.Models.NetModels.AttachmentNetModels;
using System.Net;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SCM.Services.SCMServices
{
    public class VifService : BaseService, IVifService
    {
        public VifService(IUnitOfWork unitOfWork, IMapper mapper, IAttachmentService attachmentService,
            IVrfService vrfService, INetworkSyncService netSync) : base(unitOfWork, mapper, netSync)
        {
            AttachmentService = attachmentService;
            VrfService = vrfService;
        }
        
        private IAttachmentService AttachmentService { get; set; }
        private IVrfService VrfService { get; set; }

        public async Task<Vif> GetByIDAsync(int id)
        {
            var dbResult = await UnitOfWork.InterfaceVlanRepository.GetAsync(q => q.InterfaceVlanID == id, includeProperties:
                "Interface,Interface.Port,Interface.Device.Location.SubRegion.Region,Interface.Tenant,Interface.InterfaceBandwidth,Interface.Device.Plane," +
                "Vrf.BgpPeers,Vrf.Device,ContractBandwidthPool.ContractBandwidth,Tenant,Interface.BundleInterfacePorts.Port",
                AsTrackable: false);

            return Mapper.Map<Vif>(dbResult.SingleOrDefault());
        }

        public async Task<List<Vif>> GetAllByAttachmentIDAsync(int id)
        {
            var vifs = await UnitOfWork.InterfaceVlanRepository.GetAsync(q => q.InterfaceID == id, 
                includeProperties: "Interface,Interface.Port,Interface.Device.Location.SubRegion.Region,Interface.Tenant,"
                + "Interface.InterfaceBandwidth,Interface.Device.Plane,Vrf.BgpPeers,Vrf.Device,ContractBandwidthPool.ContractBandwidth,"
                + "Tenant,Interface.BundleInterfacePorts.Port",
                AsTrackable: false);

            return Mapper.Map<List<Vif>>(vifs);
        }

        public async Task<List<Vif>> GetAsync(Expression<Func<InterfaceVlan, bool>> filter = null)
        {
            var ifaceVlans = await UnitOfWork.InterfaceVlanRepository.GetAsync(filter,
                includeProperties: "Interface,Interface.Port,Interface.Device.Location.SubRegion.Region,Interface.Tenant,"
                + "Interface.InterfaceBandwidth,Interface.Device.Plane,Vrf.BgpPeers,Vrf.Device,ContractBandwidthPool.ContractBandwidth,"
                + "Tenant,Interface.BundleInterfacePorts.Port",
                AsTrackable: false);

            return Mapper.Map<List<Vif>>(ifaceVlans);
        }

        public async Task<ServiceResult> AddAsync(VifRequest request)
        {
            var result = new ServiceResult { IsSuccess = true };

            await AllocateVlanTag(request, result);
            if (!result.IsSuccess)
            {
                return result;
            }

            var ifaceVlan = Mapper.Map<InterfaceVlan>(request);

            Vrf vrf = null;
            if (request.IsLayer3)
            {
                var iface = await this.UnitOfWork.InterfaceRepository.GetByIDAsync(request.AttachmentID);
                vrf = Mapper.Map<Vrf>(request);
                vrf.DeviceID = iface.DeviceID;
            }

            try
            {
                if (vrf != null)
                {
                    var vrfResult = await VrfService.AddAsync(vrf);
                    if (!vrfResult.IsSuccess)
                    {
                        result.AddRange(vrfResult.GetMessageList());
                        result.IsSuccess = false;

                        return result;
                    }

                    ifaceVlan.VrfID = vrf.VrfID;
                }

                this.UnitOfWork.InterfaceVlanRepository.Insert(ifaceVlan);
                await this.UnitOfWork.SaveAsync();
            }

            catch (DbUpdateException /** ex **/)
            {
                // Add logging for the exception here
                result.Add("Something went wrong during the database update. The issue has been logged."
                   + "Please try again, and contact your system admin if the problem persists.");
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<NetworkCheckSyncServiceResult> CheckNetworkSyncAsync(Vif vif)
        {

            var attachment = await AttachmentService.GetByIDAsync(vif.AttachmentID);

            // Check VRF is in sync, if the vif is layer 3

            if (vif.IsLayer3)
            {
                var vrfServiceModelData = Mapper.Map<VrfServiceNetModel>(vif);
                var vrfCheckSyncResult = await NetSync.CheckNetworkSyncAsync(vrfServiceModelData,
                    $"/attachment/pe/{attachment.Device.Name }/vrf/{vrfServiceModelData.VrfName}");

                if (!vrfCheckSyncResult.InSync)
                {
                    return vrfCheckSyncResult;
                }
            }

            // Check the vif is in sync

            if (attachment.IsBundle)
            {
                var bundleVifServiceModelData = Mapper.Map<VifServiceNetModel>(vif);
                var checkSyncResult = await NetSync.CheckNetworkSyncAsync(bundleVifServiceModelData,
                    $"/attachment/pe/{attachment.Device.Name}/tagged-attachment-bundle-interface/{attachment.BundleID}/vif/{vif.VlanTag}");

                return checkSyncResult;
            }
            else
            {
                var attachmentVifServiceModelData = Mapper.Map<VifServiceNetModel>(vif);
                var checkSyncResult = await NetSync.CheckNetworkSyncAsync(attachmentVifServiceModelData,
                    $"/attachment/pe/{attachment.Device.Name}/tagged-attachment-interface/{attachment.InterfaceType},"
                    + $"{attachment.InterfaceName.Replace("/", "%2F")}/vif/{vif.VlanTag}");

                return checkSyncResult;
            }
        }

        public async Task<NetworkSyncServiceResult> SyncToNetworkAsync(Vif vif)
        {
            var attachment = await AttachmentService.GetByIDAsync(vif.AttachmentID);

            if (vif.IsLayer3)
            {
                var vrfServiceModelData = Mapper.Map<VrfServiceNetModel>(vif);
                var vrfSyncResult = await NetSync.SyncNetworkAsync(vrfServiceModelData,
                    $"/attachment/pe/{attachment.Device.Name }/vrf/{vrfServiceModelData.VrfName}");

                if (!vrfSyncResult.IsSuccess)
                {
                    return vrfSyncResult;
                }
            }

            if (attachment.IsBundle)
            {
                var bundleVifServiceModelData = Mapper.Map<VifServiceNetModel>(vif);
                var syncResult = await NetSync.SyncNetworkAsync(bundleVifServiceModelData,
                    $"/attachment/pe/{attachment.Device.Name}/tagged-attachment-bundle-interface/{attachment.BundleID}/vif/{vif.VlanTag}");

                return syncResult;
            }
            else
            {
                var attachmentVifServiceModelData = Mapper.Map<VifServiceNetModel>(vif);
                var syncResult = await NetSync.SyncNetworkAsync(attachmentVifServiceModelData,
                    $"/attachment/pe/{attachment.Device.Name}/tagged-attachment-interface/{attachment.InterfaceType},"
                    + $"{attachment.InterfaceName.Replace("/", "%2F")}/vif/{vif.VlanTag}");

                return syncResult;
            }
        }

        /// <summary>
        /// Delete a vif from the network and from the inventory
        /// </summary>
        /// <param name="vif"></param>
        /// <returns></returns>
        public async Task<ServiceResult> DeleteAsync(Vif vif)
        {
            var result = new ServiceResult { IsSuccess = true };

            if (vif.VrfID != null)
            {
                var validateVrfDelete = await VrfService.ValidateDeleteAsync(vif.VrfID.Value);
                if (!validateVrfDelete.IsSuccess)
                {
                    result.AddRange(validateVrfDelete.GetMessageList());
                    result.IsSuccess = false;

                    return result;
                }
            }

            var syncResult = await DeleteFromNetworkAsync(vif);

            // Delete from network may return IsSuccess false if the resource was not found - this should be ignored

            if (!syncResult.IsSuccess && syncResult.NetworkHttpResponse.HttpStatusCode != HttpStatusCode.NotFound)
            {
                result.IsSuccess = false;
                result.AddRange(syncResult.GetMessageList());

                return result;
            }

            try
            {
                await this.UnitOfWork.InterfaceVlanRepository.DeleteAsync(vif.ID);
                if (vif.VrfID != null)
                {
                    await UnitOfWork.VrfRepository.DeleteAsync(vif.VrfID);
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

        /// <summary>
        /// Delete a vif from the network only.
        /// </summary>
        /// <param name="vif"></param>
        /// <returns></returns>
        public async Task<NetworkSyncServiceResult> DeleteFromNetworkAsync(Vif vif)
        {
            var syncResult = new NetworkSyncServiceResult();
            var attachment = await AttachmentService.GetByIDAsync(vif.AttachmentID);

            // Check for VPN Attachment Sets - if a VRF for the Attachment exists, which 
            // is to be deleted, and one or more VPNs are bound to the  VRF, 
            // then quit and warn the user

            if (vif.VrfID != null)
            {
                var validateVrfDelete = await VrfService.ValidateDeleteAsync(vif.Vrf.VrfID);
                if (!validateVrfDelete.IsSuccess)
                {
                    syncResult.AddRange(validateVrfDelete.GetMessageList());

                    return syncResult;
                }
            }

            if (attachment.IsBundle)
            {
                syncResult = await NetSync.DeleteFromNetworkAsync($"/attachment/pe/{attachment.Device.Name}/tagged-attachment-bundle-interface/" 
                    + $"{attachment.BundleID}/vif/{vif.VlanTag}");

                return syncResult;
            }
            else
            {
                syncResult = await NetSync.DeleteFromNetworkAsync($"/attachment/pe/{attachment.Device.Name}/tagged-attachment-interface/"
                    + $"{attachment.InterfaceType},{attachment.InterfaceName.Replace("/", "%2F")}/vif/{vif.VlanTag}");

                return syncResult;
            }
        }

        /// <summary>
        /// Validates a vif request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ServiceResult> ValidateAsync(VifRequest request)
        {
            var result = new ServiceResult { IsSuccess = true };

            var dbInterfaceResult = await UnitOfWork.InterfaceRepository.GetAsync(q => q.InterfaceID == request.AttachmentID, includeProperties: "Port");
            var iface = dbInterfaceResult.SingleOrDefault();

            if (iface == null)
            {
                result.Add("The attachment interface was not found.");
                result.IsSuccess = false;

                return result;
            }

            if (!iface.IsTagged)
            {
                result.Add("A vif cannot be created for an untagged attachment interface.");
                result.IsSuccess = false;

                return result;
            }

            var dbResult = await UnitOfWork.ContractBandwidthPoolRepository.GetAsync(q =>
                   q.ContractBandwidthPoolID == request.ContractBandwidthPoolID, includeProperties: "Interfaces.Port,InterfaceVlans.Interface.Port");

            var contractBandwidthPool = dbResult.SingleOrDefault();
            if (contractBandwidthPool == null)
            {
                result.Add("The requested contract bandwidth pool was not found.");
                result.IsSuccess = false;

                return result;
            }

            if (contractBandwidthPool.Interfaces.Count > 0)
            {
                var intface = contractBandwidthPool.Interfaces.Single();
                if (intface.IsBundle)
                {
                    result.Add($"The selected contract bandwidth pool is in-use for attachment bundle {intface.BundleID}. "
                        + "Select another contract bandwidth pool.");
                }
                else
                {
                    result.Add($"The selected contract bandwidth pool is in-use for attachment {intface.Port.Type}{intface.Port.Name}. "
                        + "Select another contract bandwidth pool.");
                }

                result.IsSuccess = false;

                return result;
            }

            if (contractBandwidthPool.InterfaceVlans.Count > 0)
            {
                var ifaceVlan = contractBandwidthPool.InterfaceVlans.Single();
                if (ifaceVlan.Interface.IsBundle)
                {
                    result.Add($"The selected contract bandwidth pool is in-use for vif {ifaceVlan.Interface.BundleID}.{ifaceVlan.VlanTag}. "
                        + "Select another contract bandwidth pool.");
                }
                else
                {
                    result.Add($"The selected contract bandwidth pool is in-use for vif " 
                        + $"{ifaceVlan.Interface.Port.Type}{ifaceVlan.Interface.Port.Name}.{ifaceVlan.VlanTag}. "
                        + "Select another contract bandwidth pool.");
                }
                result.IsSuccess = false;

                return result;
            }

            var vifs = await GetAllByAttachmentIDAsync(iface.InterfaceID);
            var aggregateContractBandwidthMbps = vifs.Sum(q => q.ContractBandwidthPool.ContractBandwidth.BandwidthMbps);

            if ((aggregateContractBandwidthMbps + contractBandwidthPool.ContractBandwidth.BandwidthMbps) > iface.InterfaceBandwidth.BandwidthGbps * 1000)
            {
                result.Add("The selected contract bandwidth exceeds the remaining bandwidth of the attachment.");
                result.Add($"Remaining bandwidth : {(iface.InterfaceBandwidth.BandwidthGbps * 1000) - aggregateContractBandwidthMbps}.");
                result.Add($"Requested bandwidth : {contractBandwidthPool.ContractBandwidth.BandwidthMbps}.");

                result.IsSuccess = false;
            }

            return result;
        }

        private async Task AllocateVlanTag(VifRequest request, ServiceResult result)
        {
            if (request.AutoAllocateVlanTag)
            {
                var dbResult = await UnitOfWork.VlanTagRangeRepository.GetAsync(q => q.Name == "Default");
                var vlanTagRange = dbResult.SingleOrDefault();

                if (vlanTagRange == null)
                {
                    result.Add("The default vlan tag range was not found.");
                    result.IsSuccess = false;

                    return;
                }

                var currentVifs = await UnitOfWork.InterfaceVlanRepository.GetAsync(q => q.InterfaceID == request.AttachmentID &&
                q.VlanTagRangeID == vlanTagRange.VlanTagRangeID);

                // Allocate a new unused vlan tag from the vlan tag range

                int? newVlanTag = Enumerable.Range(vlanTagRange.VlanTagRangeStart, vlanTagRange.VlanTagRangeCount)
                    .Except(currentVifs.Select(v => v.VlanTag)).FirstOrDefault();

                if (newVlanTag == null)
                {
                    result.Add("Failed to allocate a free vlan tag. Please contact your administrator, or try another range.");
                    result.IsSuccess = false;

                    return;
                }

                request.VlanTagRangeID = vlanTagRange.VlanTagRangeID;
                request.AllocatedVlanTag = newVlanTag.Value;
            }
            else
            {
                if (request.RequestedVlanTag == null)
                {
                    result.Add("A requested vlan tag must be specified, or select the auto-allocate vlan tag option.");
                    result.IsSuccess = false;

                    return;
                }

                var interfaceVlans = await UnitOfWork.InterfaceVlanRepository.GetAsync(q => q.InterfaceID == request.AttachmentID);
                var vlanTags = interfaceVlans.Select(q => q.VlanTag);

                if (vlanTags.Contains(request.RequestedVlanTag.Value))
                {
                    var attachment = await AttachmentService.GetByIDAsync(request.AttachmentID);
                    result.Add($"The request vlan tag is already in use for attachment '{attachment.Name}.");
                    result.Add("Try again with a different vlan tag.");
                    result.IsSuccess = false;

                    return;
                }
                else
                {
                    request.AllocatedVlanTag = request.RequestedVlanTag.Value;

                    return;
                }
            }
        }
    }
}
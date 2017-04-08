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
using System.Net.Http;

namespace SCM.Services.SCMServices
{
    public class VifService : BaseService, IVifService
    {
        public VifService(IUnitOfWork unitOfWork, IMapper mapper, IAttachmentService attachmentService,
            IVrfService vrfService, IContractBandwidthPoolService contractBandwidthPoolService, INetworkSyncService netSync) : base(unitOfWork, mapper, netSync)
        {
            AttachmentService = attachmentService;
            VrfService = vrfService;
            ContractBandwidthPoolService = contractBandwidthPoolService;
        }

        private IAttachmentService AttachmentService { get; set; }
        private IVrfService VrfService { get; set; }
        private IContractBandwidthPoolService ContractBandwidthPoolService { get; set; }

        public async Task<Vif> GetByIDAsync(int id, bool? attachmentIsMultiPort = false)
        {
            if (attachmentIsMultiPort.GetValueOrDefault())
            {
                var dbResult = await UnitOfWork.MultiPortVlanRepository.GetAsync(q => q.MultiPortVlanID == id, includeProperties:
                    "MultiPort.Device.Location.SubRegion.Region,MultiPort.Device.Plane,MultiPort.Ports.Interface.InterfaceBandwidth,"
                    + "MultiPort.Ports.Interface.InterfaceVlans.Vrf,MultiPort.Ports.Interface.InterfaceVlans.ContractBandwidthPool.ContractBandwidth,"
                    + "Vrf.BgpPeers,Vrf.Device,ContractBandwidthPool.ContractBandwidth,Tenant",
                    AsTrackable: false);

                return Mapper.Map<Vif>(dbResult.SingleOrDefault());
            }
            else
            {
                var dbResult = await UnitOfWork.InterfaceVlanRepository.GetAsync(q => q.InterfaceVlanID == id, includeProperties:
                    "Interface,Interface.Port,Interface.Device.Location.SubRegion.Region,Interface.Tenant,Interface.InterfaceBandwidth,Interface.Device.Plane,"
                    + "Vrf.BgpPeers,Vrf.Device,ContractBandwidthPool.ContractBandwidth,Tenant,Interface.BundleInterfacePorts.Port",
                    AsTrackable: false);

                return Mapper.Map<Vif>(dbResult.SingleOrDefault());
            }
        }

        /// <summary>
        /// Return a VIF from the VRF which the VIF is associated with.
        /// </summary>
        /// <param name="vrfID"></param>
        /// <returns></returns>
        public async Task<Vif> GetByVrfIDAsync(int vrfID) 
        {
            var dbResult = await UnitOfWork.VrfRepository.GetAsync(q => q.VrfID == vrfID, includeProperties: "InterfaceVlans,MultiPortVlans");
            var vrf = dbResult.Single();

            if (vrf.MultiPortVlans.Count > 0)
            {
                var multiPortVlan = vrf.MultiPortVlans.Single();

                return await GetByIDAsync(multiPortVlan.MultiPortVlanID, true);
            }
            else if (vrf.InterfaceVlans.Count > 0)
            {
                var ifaceVlan = vrf.InterfaceVlans.Single();

                return await GetByIDAsync(ifaceVlan.InterfaceVlanID, false);
            }
            else
            {
                return null;
            }
        }

        public async Task<List<Vif>> GetAllByAttachmentIDAsync(int id, bool? attachmentIsMultiPort = false)
        {

            if (attachmentIsMultiPort.GetValueOrDefault())
            {
                var vifs = await UnitOfWork.MultiPortVlanRepository.GetAsync(q => q.MultiPortID == id,
                includeProperties: "MultiPort.Device.Location.SubRegion.Region,MultiPort.Device.Plane,MultiPort.Ports.Interface.InterfaceBandwidth," +
                    "Vrf.BgpPeers,ContractBandwidthPool.ContractBandwidth,Tenant",
                AsTrackable: false);

                return Mapper.Map<List<Vif>>(vifs);
            }
            else
            {
                var vifs = await UnitOfWork.InterfaceVlanRepository.GetAsync(q => q.InterfaceID == id,
                includeProperties: "Interface,Interface.Port,Interface.Device.Location.SubRegion.Region,Interface.Tenant,"
                + "Interface.InterfaceBandwidth,Interface.Device.Plane,Vrf.BgpPeers,Vrf.Device,ContractBandwidthPool.ContractBandwidth,"
                + "Tenant,Interface.BundleInterfacePorts.Port",
                AsTrackable: false);

                return Mapper.Map<List<Vif>>(vifs);
            }
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

        public async Task<List<Vif>> GetAsync(Expression<Func<MultiPortVlan, bool>> filter = null)
        {
            var ifaceVlans = await UnitOfWork.MultiPortVlanRepository.GetAsync(filter,
                includeProperties: "MultiPort.Device.Location.SubRegion.Region,Device.Plane,MultiPort.Ports.InterfaceInterfaceBandwidth," +
                    "MultiPort.Vrf.BgpPeers,MultiPort.Device,ContractBandwidthPool.ContractBandwidth,Tenant",
                AsTrackable: false);

            return Mapper.Map<List<Vif>>(ifaceVlans);
        }

        public async Task<ServiceResult> AddAsync(VifRequest request)
        {
            var result = new ServiceResult { IsSuccess = true };

            if (request.AttachmentIsMultiPort)
            {
                await AddVifToMultiPortAttachmentAsync(request, result);
            }
            else
            {
                await AddVifToAttachmentAsync(request, result);
            }

            return result;
        }

        public async Task<NetworkCheckSyncServiceResult> CheckNetworkSyncAsync(Vif vif)
        {
            if (vif.Attachment.IsMultiPort)
            {
                return await CheckNetworkSyncMultiPortVifAsync(vif);
            }
            else
            {
                return await CheckNetworkSyncAttachmentVifAsync(vif);
            };
        }

        public async Task<NetworkSyncServiceResult> SyncToNetworkAsync(Vif vif)
        {
            var result = new NetworkSyncServiceResult { IsSuccess = true };
            var serviceModelData = Mapper.Map<AttachmentServiceNetModel>(vif);

            result = await NetSync.SyncNetworkAsync(serviceModelData, $"/attachment/pe/{vif.Attachment.Device.Name}", new HttpMethod("PATCH"));

            await UpdateRequiresSyncAsync(vif.ID, !result.IsSuccess, true, vif.Attachment.IsMultiPort);

            return result;
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

            if (!syncResult.IsSuccess)
            {
                if (syncResult.NetworkHttpResponse == null || syncResult.NetworkHttpResponse.HttpStatusCode != HttpStatusCode.NotFound)
                {
                    result.IsSuccess = false;
                    result.AddRange(syncResult.GetMessageList());

                    return result;
                }
            }

            try
            {
                if (vif.VrfID != null)
                {
                    await UnitOfWork.VrfRepository.DeleteAsync(vif.VrfID);
                }

                if (vif.Attachment.IsMultiPort)
                {
                    var interfaceVlans = await this.UnitOfWork.InterfaceVlanRepository.GetAsync(q => q.Interface.Port.MultiPortID == vif.AttachmentID 
                        && q.VlanTag == vif.VlanTag);

                    var multiPortVlan = await this.UnitOfWork.MultiPortVlanRepository.GetByIDAsync(vif.ID);

                    foreach (var ifaceVlan in interfaceVlans) {

                        this.UnitOfWork.InterfaceVlanRepository.Delete(ifaceVlan);
                    }

                    this.UnitOfWork.MultiPortVlanRepository.Delete(multiPortVlan);
                }
                else
                {
                    await this.UnitOfWork.InterfaceVlanRepository.DeleteAsync(vif.ID);
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
            var result = new NetworkSyncServiceResult { IsSuccess = true };

            // Check for VPN Attachment Sets - if a VRF for the Attachment exists, which 
            // is to be deleted, and one or more VPNs are bound to the VRF, 
            // then quit and warn the user

            if (vif.VrfID != null)
            {
                var validationResult = await VrfService.ValidateDeleteAsync(vif.Vrf.VrfID);
                if (!validationResult.IsSuccess)
                {
                    result.AddRange(validationResult.GetMessageList());
                    result.IsSuccess = false;

                    return result;
                }
            }

            var tasks = new List<Task<NetworkSyncServiceResult>>();

            if (vif.Attachment.IsBundle)
            {
                tasks.Add(NetSync.DeleteFromNetworkAsync($"/attachment/pe/{vif.Attachment.Device.Name}/tagged-attachment-bundle-interface/"
                    + $"{vif.Attachment.BundleID}/vif/{vif.VlanTag}"));

            }
            else if (vif.Attachment.IsMultiPort)
            {
                foreach (var port in vif.Attachment.MultiPortMembers)
                {
                    // Delete vif from each member port

                    tasks.Add(NetSync.DeleteFromNetworkAsync($"/attachment/pe/{vif.Attachment.Device.Name}/tagged-attachment-multiport/{vif.Attachment.Name}"
                        + $"/multiport-member/{port.Type},{port.Name.Replace("/", "%2F")}/vif/{vif.VlanTag}"));
                }
            }
            else
            {
                tasks.Add(NetSync.DeleteFromNetworkAsync($"/attachment/pe/{vif.Attachment.Device.Name}/tagged-attachment-interface/"
                    + $"{vif.Attachment.InterfaceType},{vif.Attachment.InterfaceName.Replace("/", "%2F")}/vif/{vif.VlanTag}"));

            }

            // Execute network updates in parallel

            await Task.WhenAll(tasks);

            // Delete VRF from network

            var vrfResult = await NetSync.DeleteFromNetworkAsync($"/attachment/pe/{vif.Attachment.Device.Name }/vrf/{vif.Vrf.Name}");

            // Check the results 

            foreach (Task<NetworkSyncServiceResult> t in tasks)
            {
                var r = t.Result;
                if (!r.IsSuccess)
                {
                    result.AddRange(r.GetMessageList());
                    result.IsSuccess = false;
                }
            }

            if (!vrfResult.IsSuccess)
            {
                result.AddRange(vrfResult.GetMessageList());
                result.IsSuccess = false;
            }

            return result;
        }

        /// <summary>
        /// Validates a vif request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ServiceResult> ValidateAsync(VifRequest request)
        {
            if (request.AttachmentIsMultiPort)
            {
                return await ValidateMultiPortVifRequestAsync(request);
            }
            else
            {
                return await ValidateAttachmentVifRequestAsync(request);
            }
        }

        /// <summary>
        /// Update the RequiresSync property of an interfaceVlan record.
        /// </summary>
        /// <param name="ifaceVlan"></param>
        /// <param name="requiresSync"></param>
        /// <returns></returns>
        public async Task UpdateRequiresSyncAsync(InterfaceVlan ifaceVlan, bool requiresSync, bool saveChanges = true)
        {
            ifaceVlan.RequiresSync = requiresSync;
            UnitOfWork.InterfaceVlanRepository.Update(ifaceVlan);

            if (saveChanges)
            {
                await UnitOfWork.SaveAsync();
            }

            return;
        }

        /// Update the RequiresSync property of a multi-port vlan record.
        /// </summary>
        /// <param name="multiPortVlan"></param>
        /// <param name="requiresSync"></param>
        /// <returns></returns>
        public async Task UpdateRequiresSyncAsync(MultiPortVlan multiPortVlan, bool requiresSync, bool saveChanges = true)
        {
            multiPortVlan.RequiresSync = requiresSync;
            UnitOfWork.MultiPortVlanRepository.Update(multiPortVlan);

            if (saveChanges)
            {
                await UnitOfWork.SaveAsync();
            }

            return;
        }

        /// <summary>
        /// Update the RequiresSync property of an interfaceVlan record.
        /// </summary>
        /// <param name="ifaceVlan"></param>
        /// <param name="requiresSync"></param>
        /// <returns></returns>
        public async Task UpdateRequiresSyncAsync(int id, bool requiresSync, bool saveChanges = true, bool? attachmentIsMultiPort = false)
        {
            if (attachmentIsMultiPort.GetValueOrDefault())
            {

                var multiPortVlan = await UnitOfWork.MultiPortVlanRepository.GetByIDAsync(id);
                await UpdateRequiresSyncAsync(multiPortVlan, requiresSync, saveChanges);
            }
            else
            {

                var ifaceVlan = await UnitOfWork.InterfaceVlanRepository.GetByIDAsync(id);
                await UpdateRequiresSyncAsync(ifaceVlan, requiresSync, saveChanges);
            }

            return;
        }

        /// <summary>
        /// Validates a request for an Attachment or Bundle Attachment VIF
        /// </summary>
        /// <param name="request"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task<ServiceResult> ValidateAttachmentVifRequestAsync(VifRequest request)
        {

            var result = new ServiceResult { IsSuccess = true };

            var dbInterfaceResult = await UnitOfWork.InterfaceRepository.GetAsync(q => q.InterfaceID == request.AttachmentID,
                    includeProperties: "InterfaceBandwidth,Port");
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

            // Validate the requested Contract Bandwidth Pool

            var validateContractBandwidthPoolResult = await ContractBandwidthPoolService.ValidateAsync(request.ContractBandwidthPoolID.Value);
            if (!validateContractBandwidthPoolResult.IsSuccess)
            {
                result.IsSuccess = false;
                result.AddRange(validateContractBandwidthPoolResult.GetMessageList());

                return result;
            }

            var vifs = await GetAllByAttachmentIDAsync(iface.InterfaceID);
            var aggregateContractBandwidthMbps = vifs.Sum(q => q.ContractBandwidthPool.ContractBandwidth.BandwidthMbps);
            var contractBandwidthPoolResult = await UnitOfWork.ContractBandwidthPoolRepository.GetAsync(q => q.ContractBandwidthPoolID == request.ContractBandwidthPoolID, 
                includeProperties: "ContractBandwidth");
            var contractBandwidthPool = contractBandwidthPoolResult.Single();

            if ((aggregateContractBandwidthMbps + contractBandwidthPool.ContractBandwidth.BandwidthMbps) > iface.InterfaceBandwidth.BandwidthGbps * 1000)
            {
                result.Add("The selected contract bandwidth exceeds the remaining bandwidth of the attachment.");
                result.Add($"Remaining bandwidth : {(iface.InterfaceBandwidth.BandwidthGbps * 1000) - aggregateContractBandwidthMbps} Mbps.");
                result.Add($"Requested bandwidth : {contractBandwidthPool.ContractBandwidth.BandwidthMbps} Mbps.");

                result.IsSuccess = false;
            }

            return result;
        }

        /// <summary>
        /// Validates a request for a Multiport VIF
        /// </summary>
        /// <param name="request"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task<ServiceResult> ValidateMultiPortVifRequestAsync(VifRequest request)
        {
            var result = new ServiceResult { IsSuccess = true };

            var dbMultiPortResult = await UnitOfWork.MultiPortRepository.GetAsync(q => q.MultiPortID == request.AttachmentID, includeProperties: "InterfaceBandwidth");
            var multiPort = dbMultiPortResult.SingleOrDefault();

            if (multiPort == null)
            {
                result.Add("The multi-port was not found.");
                result.IsSuccess = false;

                return result;
            }

            if (!multiPort.IsTagged)
            {
                result.Add("A vif cannot be created for an untagged multiport.");
                result.IsSuccess = false;

                return result;
            }

            // Validate the requested Contract Bandwidth Pool

            var validateContractBandwidthPoolResult = await ContractBandwidthPoolService.ValidateAsync(request.ContractBandwidthPoolID.Value);
            if (!validateContractBandwidthPoolResult.IsSuccess)
            {
                result.IsSuccess = false;
                result.AddRange(validateContractBandwidthPoolResult.GetMessageList());

                return result;
            }

            var vifs = await GetAllByAttachmentIDAsync(multiPort.MultiPortID, true);
            var aggregateContractBandwidthMbps = vifs.Sum(q => q.ContractBandwidthPool.ContractBandwidth.BandwidthMbps);
            var contractBandwidthPoolResult = await UnitOfWork.ContractBandwidthPoolRepository.GetAsync(q => q.ContractBandwidthPoolID == request.ContractBandwidthPoolID,
               includeProperties: "ContractBandwidth");
            var contractBandwidthPool = contractBandwidthPoolResult.Single();

            if ((aggregateContractBandwidthMbps + contractBandwidthPool.ContractBandwidth.BandwidthMbps) > multiPort.InterfaceBandwidth.BandwidthGbps * 1000)
            {
                result.Add("The selected contract bandwidth exceeds the remaining bandwidth of the multi-port.");
                result.Add($"Remaining bandwidth : {(multiPort.InterfaceBandwidth.BandwidthGbps * 1000) - aggregateContractBandwidthMbps} Mbps.");
                result.Add($"Requested bandwidth : {contractBandwidthPool.ContractBandwidth.BandwidthMbps} Mbps.");

                result.IsSuccess = false;

                return result;
            }

            if (request.IsLayer3)
            {
                if (multiPort.InterfaceBandwidth.BandwidthGbps == 20)
                {
                    if (string.IsNullOrEmpty(request.IpAddress1) || string.IsNullOrEmpty(request.IpAddress2))
                    {
                        result.Add("Two IP addresses must be entered.");
                        result.IsSuccess = false;
                    }
                    if (string.IsNullOrEmpty(request.SubnetMask1) || string.IsNullOrEmpty(request.SubnetMask2))
                    {
                        result.Add("Two subnet masks must be entered.");
                        result.IsSuccess = false;
                    }
                }
                else if (multiPort.InterfaceBandwidth.BandwidthGbps == 40)
                {
                    if (string.IsNullOrEmpty(request.IpAddress1) || string.IsNullOrEmpty(request.IpAddress2)
                        || string.IsNullOrEmpty(request.IpAddress3) || string.IsNullOrEmpty(request.IpAddress4))
                    {
                        result.Add("Four IP addresses must be entered.");
                        result.IsSuccess = false;
                    }
                    if (string.IsNullOrEmpty(request.SubnetMask1) || string.IsNullOrEmpty(request.SubnetMask2)
                        || string.IsNullOrEmpty(request.SubnetMask3) || string.IsNullOrEmpty(request.SubnetMask4))
                    {
                        result.Add("Four subnet masks must be entered.");
                        result.IsSuccess = false;
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// Add a VIF to an Attachment
        /// </summary>
        /// <param name="request"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task AddVifToAttachmentAsync(VifRequest request, ServiceResult result)
        {
            await AllocateVlanTagAsync(request, result);

            if (!result.IsSuccess)
            {
                return;
            }

            var ifaceVlan = Mapper.Map<InterfaceVlan>(request);
            ifaceVlan.RequiresSync = true;

            Vrf vrf = null;
            if (request.IsLayer3)
            {
                var iface = await this.UnitOfWork.InterfaceRepository.GetByIDAsync(request.AttachmentID);
                vrf = Mapper.Map<Vrf>(request);
                vrf.DeviceID = iface.DeviceID;
                ifaceVlan.IpAddress = request.IpAddress1;
                ifaceVlan.SubnetMask = request.SubnetMask1;
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

                        return;
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
        }

        /// <summary>
        /// Add a VIF to a MultiPort Attachment
        /// </summary>
        /// <param name="request"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task AddVifToMultiPortAttachmentAsync(VifRequest request, ServiceResult result)
        {
            await AllocateVlanTagAsync(request, result);

            if (!result.IsSuccess)
            {
                return;
            }

            var multiPortResult = await this.UnitOfWork.MultiPortRepository.GetAsync(q => q.MultiPortID == request.AttachmentID, 
                includeProperties: "Ports.Interface");
            var multiPort = multiPortResult.Single();

            var multiPortVlan = Mapper.Map<MultiPortVlan>(request);
            multiPortVlan.RequiresSync = true;

            Vrf vrf = null;
            if (request.IsLayer3)
            {
                vrf = Mapper.Map<Vrf>(request);
                vrf.DeviceID = multiPort.DeviceID;
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

                        return;
                    }

                    multiPortVlan.VrfID = vrf.VrfID;
                }

                var ports = multiPort.Ports.ToList();
                var portsCount = ports.Count;

                for (var i = 1; i <= portsCount; i++)
                {
                    var ifaceVlan = Mapper.Map<InterfaceVlan>(request);
                    ifaceVlan.InterfaceID = ports[i - 1].Interface.InterfaceID;

                    if (request.IsLayer3)
                    {
                        ifaceVlan.VrfID = vrf.VrfID;

                        if (i == 1)
                        {
                            ifaceVlan.IpAddress = request.IpAddress1;
                            ifaceVlan.SubnetMask = request.SubnetMask1;
                        }
                        else if (i == 2)
                        {
                            ifaceVlan.IpAddress = request.IpAddress2;
                            ifaceVlan.SubnetMask = request.SubnetMask2;
                        }
                        else if (i == 3)
                        {
                            ifaceVlan.IpAddress = request.IpAddress3;
                            ifaceVlan.SubnetMask = request.SubnetMask3;
                        }
                        else if (i == 4)
                        {
                            ifaceVlan.IpAddress = request.IpAddress4;
                            ifaceVlan.SubnetMask = request.SubnetMask4;
                        }
                    }

                    this.UnitOfWork.InterfaceVlanRepository.Insert(ifaceVlan);
                }

                this.UnitOfWork.MultiPortVlanRepository.Insert(multiPortVlan);
               
                await this.UnitOfWork.SaveAsync();
            }

            catch (DbUpdateException /** ex **/)
            {
                // Add logging for the exception here
                result.Add("Something went wrong during the database update. The issue has been logged."
                   + "Please try again, and contact your system admin if the problem persists.");
                result.IsSuccess = false;
            }
        }

        /// <summary>
        /// Check network sync for an VIF associated with an Attachment.
        /// </summary>
        /// <param name="vif"></param>
        /// <returns></returns>
        private  async Task<NetworkCheckSyncServiceResult> CheckNetworkSyncAttachmentVifAsync(Vif vif)
        {
            var result = new NetworkCheckSyncServiceResult { InSync = true };

            if (vif.Attachment.IsBundle)
            {
                var serviceModelData = Mapper.Map<VifServiceNetModel>(vif);
                result = await NetSync.CheckNetworkSyncAsync(serviceModelData,
                    $"/attachment/pe/{vif.Attachment.Device.Name}/tagged-attachment-bundle-interface/{vif.Attachment.BundleID}/vif/{vif.VlanTag}");
            }

            else
            {
                var serviceModelData = Mapper.Map<VifServiceNetModel>(vif);
                result = await NetSync.CheckNetworkSyncAsync(serviceModelData,
                    $"/attachment/pe/{vif.Attachment.Device.Name}/tagged-attachment-interface/{vif.Attachment.InterfaceType},"
                    + $"{vif.Attachment.InterfaceName.Replace("/", "%2F")}/vif/{vif.VlanTag}");

            }
    
            await UpdateRequiresSyncAsync(vif.ID, !result.InSync, true, false);

            return result;
        }

        /// <summary>
        /// Check network sync for an VIF associated with a Multi-Port Attachment.
        /// </summary>
        /// <param name="vif"></param>
        /// <returns></returns>
        private async Task<NetworkCheckSyncServiceResult> CheckNetworkSyncMultiPortVifAsync(Vif vif)
        {
            var result = new NetworkCheckSyncServiceResult { InSync = true };
            var tasks = new List<Task<NetworkCheckSyncServiceResult>>();

            foreach (var port in vif.Attachment.MultiPortMembers)
            {
                // Create task to check the vif for each member port

                var ifaceVlan = port.Interface.InterfaceVlans.Where(q => q.VlanTag == vif.VlanTag).Single();
                var vifServiceModelData = Mapper.Map<VifServiceNetModel>(ifaceVlan);

                tasks.Add(NetSync.CheckNetworkSyncAsync(vifServiceModelData, $"/attachment/pe/{vif.Attachment.Device.Name}" 
                    + $"/tagged-attachment-multiport/{vif.Attachment.Name}"
                    + $"/multiport-member/{port.Type},{port.Name.Replace("/", "%2F")}/vif/{vif.VlanTag}"));
            }

            // Execute network checks in parallel

            await Task.WhenAll(tasks);

            // Check the results 

            foreach (Task<NetworkCheckSyncServiceResult> t in tasks)
            {
                var r = t.Result;
                if (!r.InSync)
                {
                    result.NetworkSyncServiceResult.AddRange(r.NetworkSyncServiceResult.GetMessageList());
                    result.InSync = false;
                }
            }

            await UpdateRequiresSyncAsync(vif.ID, !result.InSync, true, true);

            return result;
        }
     
        /// <summary>
        /// Allocate a vlan tag for a new VIF.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task AllocateVlanTagAsync(VifRequest request, ServiceResult result)
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

                IEnumerable<InterfaceVlan> currentVlans;
                if (request.AttachmentIsMultiPort)
                {
                    currentVlans = await UnitOfWork.InterfaceVlanRepository.GetAsync(q => q.Interface.Port.MultiPortID == request.AttachmentID);
                }
                else
                {
                    currentVlans = await UnitOfWork.InterfaceVlanRepository.GetAsync(q => q.InterfaceID == request.AttachmentID);
                }

                // Allocate a new unused vlan tag from the vlan tag range

                int? newVlanTag = Enumerable.Range(vlanTagRange.VlanTagRangeStart, vlanTagRange.VlanTagRangeCount)
                    .Except(currentVlans.Select(v => v.VlanTag)).FirstOrDefault();

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

                IEnumerable<InterfaceVlan> ifaceVlans;
                if (request.AttachmentIsMultiPort)
                {
                    ifaceVlans= await UnitOfWork.InterfaceVlanRepository.GetAsync(q => q.Interface.Port.MultiPortID == request.AttachmentID
                        && q.VlanTag == request.RequestedVlanTag);
                }
                else
                {
                    ifaceVlans = await UnitOfWork.InterfaceVlanRepository.GetAsync(q => q.InterfaceID == request.AttachmentID
                    && q.VlanTag == request.RequestedVlanTag);
                }

                if (ifaceVlans.Count() > 0)
                {
                    var attachment = await AttachmentService.GetByIDAsync(request.AttachmentID, request.AttachmentIsMultiPort);
                    result.Add($"The requested vlan tag is already in use for attachment '{attachment.Name}.");
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
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

        public async Task<Vif> GetByIDAsync(int id)
        {

            var dbResult = await UnitOfWork.VifRepository.GetAsync(q => q.VifID == id, 
                includeProperties: "Attachment.Tenant,"
                + "Attachment.Device.Location.SubRegion.Region,"
                + "Attachment.Device.Plane,"
                + "Attachment.Vrf.BgpPeers,"
                + "Attachment.Vrf.AttachmentSetVrfs.AttachmentSet,"
                + "Attachment.AttachmentBandwidth,"
                + "Attachment.ContractBandwidthPool.ContractBandwidth,"
                + "Attachment.Interfaces.Device,"
                + "Attachment.Interfaces.Ports.Device,"
                + "Attachment.Interfaces.Ports.PortBandwidth,"
                + "Attachment.Interfaces.Ports.Interface.Vlans.Vif,"
                + "Attachment.Vifs.Vrf.BgpPeers,"
                + "Attachment.Vifs.Vlans.Vif.ContractBandwidthPool,"
                + "Attachment.Vifs.ContractBandwidthPool.ContractBandwidth,"
                + "Vrf.BgpPeers,"
                + "Vlans,"
                + "ContractBandwidthPool.ContractBandwidth,"
                + "Tenant",
                AsTrackable: false);

            return dbResult.SingleOrDefault();
        }

        /// <summary>
        /// Return a VIF from the VRF which the VIF is associated with.
        /// </summary>
        /// <param name="vrfID"></param>
        /// <returns></returns>
        public async Task<Vif> GetByVrfIDAsync(int vrfID)
        {
            var dbResult = await UnitOfWork.VrfRepository.GetAsync(q => q.VrfID == vrfID, includeProperties: "Vifs");
            var vrf = dbResult.Single();

            return vrf.Vifs.SingleOrDefault();
        }

        public async Task<List<Vif>> GetAllByAttachmentIDAsync(int id)
        {
            var vifs = await UnitOfWork.VifRepository.GetAsync(q => q.AttachmentID == id,
                includeProperties: "Attachment.Tenant,"
                + "Attachment.Device.Location.SubRegion.Region,"
                + "Attachment.Device.Plane,"
                + "Attachment.Vrf.BgpPeers,"
                + "Attachment.Vrf.AttachmentSetVrfs.AttachmentSet,"
                + "Attachment.AttachmentBandwidth,"
                + "Attachment.ContractBandwidthPool.ContractBandwidth,"
                + "Attachment.Interfaces.Device,"
                + "Attachment.Interfaces.Ports.Device,"
                + "Attachment.Interfaces.Ports.PortBandwidth,"
                + "Attachment.Interfaces.Ports.Interface.Vlans.Vif,"
                + "Attachment.Vifs.Vrf.BgpPeers,"
                + "Attachment.Vifs.Vlans.Vif.ContractBandwidthPool,"
                + "Attachment.Vifs.ContractBandwidthPool.ContractBandwidth,"
                + "Vrf.BgpPeers,"
                + "Vlans,"
                + "ContractBandwidthPool.ContractBandwidth,"
                + "Tenant",
                AsTrackable: false);

            return vifs.ToList();
        }

        /// <summary>
        /// Return all vifs for a given vpn.
        /// </summary>
        /// <param name="attachmentID"></param>
        /// <returns></returns>
        public async Task<List<Vif>> GetAllByVpnIDAsync(int vpnID)
        {
            var result = await UnitOfWork.VifRepository
                .GetAsync(q => q.Vrf.AttachmentSetVrfs
                .SelectMany(r => r.AttachmentSet.VpnAttachmentSets)
                .Where(s => s.VpnID == vpnID)
                .Count() > 0,
                includeProperties: "Attachment.Tenant,"
                + "Attachment.Device.Location.SubRegion.Region,"
                + "Attachment.Device.Plane,"
                + "Attachment.Vrf.BgpPeers,"
                + "Attachment.Vrf.AttachmentSetVrfs.AttachmentSet,"
                + "Attachment.AttachmentBandwidth,"
                + "Attachment.ContractBandwidthPool.ContractBandwidth,"
                + "Attachment.Interfaces.Device,"
                + "Attachment.Interfaces.Ports.Device,"
                + "Attachment.Interfaces.Ports.PortBandwidth,"
                + "Attachment.Interfaces.Ports.Interface.Vlans.Vif,"
                + "Attachment.Vifs.Vrf.BgpPeers,"
                + "Attachment.Vifs.Vlans.Vif.ContractBandwidthPool,"
                + "Attachment.Vifs.ContractBandwidthPool.ContractBandwidth,"
                + "Vrf.BgpPeers,"
                + "Vlans,"
                + "ContractBandwidthPool.ContractBandwidth,"
                + "Tenant");

            return result.ToList();
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

        public async Task<ServiceResult> CheckNetworkSyncAsync(Vif vif)
        {
            var result = new ServiceResult();
            NetworkSyncServiceResult syncResult;

            if (vif.Attachment.IsMultiPort)
            {
                syncResult = await CheckNetworkSyncMultiPortVifAsync(vif);
            }
            else
            {
                syncResult = await CheckNetworkSyncAttachmentVifAsync(vif);
            }

            result.AddRange(syncResult.Messages);
            result.IsSuccess = syncResult.IsSuccess;

            return result; 
        }

        public async Task<ServiceResult> SyncToNetworkAsync(Vif vif)
        {
            var result = new ServiceResult();
            var serviceModelData = Mapper.Map<AttachmentServiceNetModel>(vif);

            var syncResult = await NetSync.SyncNetworkAsync(serviceModelData, $"/attachment/pe/{vif.Attachment.Device.Name}", new HttpMethod("PATCH"));

            result.AddRange(syncResult.Messages);
            result.IsSuccess = syncResult.IsSuccess;

            await UpdateRequiresSyncAsync(vif.VifID, !result.IsSuccess, true);

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

            try
            {
                UnitOfWork.VifRepository.Delete(vif);
                await UnitOfWork.VrfRepository.DeleteAsync(vif.VrfID);

                foreach (var vlan in vif.Vlans)
                {
                    await UnitOfWork.VlanRepository.DeleteAsync(vlan.VlanID);
                }

                // Check if the Contract Bandwidth Pool can be deleted

                var checkDeleteContractBandwidthPoolResult = await ContractBandwidthPoolService.ValidateDeleteAsync(vif);
                if (checkDeleteContractBandwidthPoolResult.IsSuccess)
                {
                    await UnitOfWork.ContractBandwidthPoolRepository.DeleteAsync(vif.ContractBandwidthPoolID);
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
        public async Task<ServiceResult> DeleteFromNetworkAsync(Vif vif)
        {
            var attachment = await AttachmentService.GetByIDAsync(vif.AttachmentID);
            var attachmentServiceData = Mapper.Map<AttachmentServiceNetModel>(attachment);
            var vifServiceData = Mapper.Map<AttachmentServiceNetModel>(vif);

            var taskResults = new List<NetworkSyncServiceResult>();
            var result = new ServiceResult { IsSuccess = true };

            // Check the attachment which the vif belongs to and process accordingly
            // The resources we need to delete vary by attachment type (bundle, multiport etc) in accordance
            // with the NSO service model definition

            try
            {
                if (vif.Attachment.IsBundle)
                {
                    // Delete the vif

                    var vifResource = $"/attachment/pe/{vif.Attachment.Device.Name}/tagged-attachment-bundle-interface/"
                        + $"{vif.Attachment.ID}";

                    taskResults.Add(await NetSync.DeleteFromNetworkAsync($"{vifResource}/vif/{vif.VlanTag}"));

                    // If the Contract Bandwidth Pool is referenced only by the vif we're about 
                    // to delete then the Contract Bandwidth Pool is not shared by any other vif and can be deleted

                    if (attachmentServiceData.TaggedAttachmentBundleInterfaces
                        .Single()
                        .Vifs
                        .Where(q => q.ContractBandwidthPoolName == vif.ContractBandwidthPool.Name)
                        .Count() == 1)
                    {
                        // Delete the Contract Bandwidth Pool resource

                        taskResults.Add(await NetSync.DeleteFromNetworkAsync($"{vifResource}/contract-bandwidth-pool/{vif.ContractBandwidthPool.Name}"));
                    }
                }
                else if (vif.Attachment.IsMultiPort)
                {

                    // If the Contract Bandwidth Pool is referenced only by the policy bandwidth of the vif we're about 
                    // to delete then the Contract Bandwidth Pool is not shared by any other vif and can be deleted

                    var deleteContractBandwidthPool = false;

                    if (attachmentServiceData.TaggedAttachmentMultiPorts
                        .Single()
                        .MultiPortMembers
                        .SelectMany(q => q.PolicyBandwidths)
                        .Where(q => q.ContractBandwidthPoolName == vif.ContractBandwidthPool.Name)
                        .Count() == vif.Attachment.Interfaces.Count())
                    {
                        // Flag for delete of the Contract Bandwidth Pool resource and the policy bandwidth resource for
                        // each member interface

                        deleteContractBandwidthPool = true;
                    }

                    var attachmentResource = $"/attachment/pe/{vif.Attachment.Device.Name}/tagged-attachment-multiport/{vif.Attachment.ID}";
                    var data = vifServiceData.TaggedAttachmentMultiPorts.Single();
                    
                    foreach (var port in data.MultiPortMembers)
                    {
                        var vifResource = $"{attachmentResource}/multiport-member/{port.InterfaceType},"
                            + $"{port.InterfaceName.Replace("/", "%2F")}";

                        // Delete vif from each member port

                        taskResults.Add(await NetSync.DeleteFromNetworkAsync($"{vifResource}/vif/{vif.VlanTag}"));

                        // Check to delete the policy bandwidth

                        if (deleteContractBandwidthPool)
                        {

                            var policyBandwidthName = vifServiceData.TaggedAttachmentMultiPorts
                                .Single().MultiPortMembers
                                .Single(q => q.InterfaceType == port.InterfaceType && q.InterfaceName == port.InterfaceName)
                                .PolicyBandwidths
                                .Single()
                                .Name;

                            // Delete the policy bandwidth

                            taskResults.Add(await NetSync.DeleteFromNetworkAsync($"{vifResource}/policy-bandwidth/{policyBandwidthName}"));
                        }
                    }

                    if (deleteContractBandwidthPool)
                    {
                        // Delete the Contract Bandwidth Pool resource

                        taskResults.Add(await NetSync.DeleteFromNetworkAsync($"{attachmentResource}/contract-bandwidth-pool/{vif.ContractBandwidthPool.Name}"));
                    }
                }
                else
                {
                    // Delete the vif

                    var data = vifServiceData.TaggedAttachmentInterfaces.Single();
                    var resource = $"/attachment/pe/{vif.Attachment.Device.Name}/tagged-attachment-interface/"
                          + $"{data.InterfaceType},{data.InterfaceName.Replace("/", "%2F")}";

                    taskResults.Add(await NetSync.DeleteFromNetworkAsync($"{resource}/vif/{vif.VlanTag}"));

                    // If the Contract Bandwidth Pool is referenced only by the vif we're about 
                    // to delete then the Contract Bandwidth Pool is not shared by any other vif and can be deleted

                    if (attachmentServiceData.TaggedAttachmentInterfaces
                        .Single()
                        .Vifs
                        .Where(q => q.ContractBandwidthPoolName == vif.ContractBandwidthPool.Name)
                        .Count() == 1)
                    {
                        // Delete the Contract Bandwidth Pool resource

                        taskResults.Add(await NetSync.DeleteFromNetworkAsync($"{resource}/contract-bandwidth-pool/{vif.ContractBandwidthPool.Name}"));
                    }
                }

                // Delete the vrf

                taskResults.Add(await NetSync.DeleteFromNetworkAsync($"/attachment/pe/{vif.Attachment.Device.Name }/vrf/{vif.Vrf.Name},{vif.Vrf.IsLayer3.ToString().ToLower()}"));

                // Compile results list

                foreach (var taskResult in taskResults) 
                {
                    result.NetworkSyncServiceResults.Add(taskResult);

                    if (!taskResult.IsSuccess)
                    {
                        result.IsSuccess = false;
                    }
                }

                await UpdateRequiresSyncAsync(vif, true, true);
            }

            catch ( Exception /** ex **/  )
            {
                // Add logging for the exception here
                result.Add("Something went wrong during the network update. The issue has been logged."
                   + "Please try again, and contact your system admin if the problem persists.");

                result.IsSuccess = false;
            }

            return result;
        }

        /// <summary>
        /// Validates a vif request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ServiceResult> ValidateNewAsync(VifRequest request)
        {
            var result = new ServiceResult { IsSuccess = true };
            var attachment = await AttachmentService.GetByIDAsync(request.AttachmentID);

            if (!attachment.IsTagged)
            {
                result.Add("A vif cannot be created for an untagged attachment interface.");
                result.IsSuccess = false;

                return result;
            }

            // Validate the requested Contract Bandwidth Pool

            var validateContractBandwidthPoolResult = await ContractBandwidthPoolService.ValidateNewAsync(request);
            if (!validateContractBandwidthPoolResult.IsSuccess)
            {
                result.IsSuccess = false;
                result.AddRange(validateContractBandwidthPoolResult.GetMessageList());

                return result;
            }

            if (request.AttachmentIsMultiPort && request.IsLayer3)
            {
                if (attachment.AttachmentBandwidth.BandwidthGbps == 20)
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
                else if (attachment.AttachmentBandwidth.BandwidthGbps == 40)
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

        public async Task<ServiceResult> ValidateAsync(Vpn vpn)
        {
            var result = new ServiceResult { IsSuccess = true };

            var vifs = await GetAllByVpnIDAsync(vpn.VpnID);
            var vifsRequireSync = vifs.Where(q => q.RequiresSync).ToList();

            if (vifsRequireSync.Count() > 0)
            {

                result.IsSuccess = false;
                result.Add("Vifs for the VPN require synchronisation with the network.");
                vifsRequireSync.ForEach(a => result.Add($"'{a.Name}' on device '{a.Attachment.Device.Name}' for tenant '{a.Tenant.Name}'."));
            }

            return result;
        }

        /// <summary>
        /// Update the RequiresSync property of a vif record.
        /// </summary>
        /// <returns></returns>
        public async Task UpdateRequiresSyncAsync(Vif vif, bool requiresSync, bool saveChanges = true)
        {
            vif.RequiresSync = requiresSync;
            UnitOfWork.VifRepository.Update(vif);

            if (saveChanges)
            {
                await UnitOfWork.SaveAsync();
            }

            return;
        }

        /// <summary>
        /// Update the RequiresSync property of a vif record.
        /// </summary>
        /// <returns></returns>
        public async Task UpdateRequiresSyncAsync(int id, bool requiresSync, bool saveChanges = true)
        {
            var vif = await UnitOfWork.VifRepository.GetByIDAsync(id);
            await UpdateRequiresSyncAsync(vif, requiresSync, saveChanges);

            return;
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

            var vif = Mapper.Map<Vif>(request);
            vif.RequiresSync = true;

            var attachment = await AttachmentService.GetByIDAsync(request.AttachmentID);
            request.DeviceID = attachment.DeviceID;

            var vlan = new Vlan();

            if (request.IsLayer3)
            {
                vlan.IpAddress = request.IpAddress1;
                vlan.SubnetMask = request.SubnetMask1;
            }

            try
            {
                var vrfResult = await VrfService.AddAsync(request);
                if (!vrfResult.IsSuccess)
                {
                    result.AddRange(vrfResult.GetMessageList());
                    result.IsSuccess = false;

                    return;
                }

                var vrf = (Vrf)vrfResult.Item;
                vif.VrfID = vrf.VrfID;

                if (request.ContractBandwidthID != null)
                {
                    // Create contract bandwidth pool for the vif

                    var contractBandwidthPoolResult = await ContractBandwidthPoolService.AddAsync(request);
                    if (!contractBandwidthPoolResult.IsSuccess)
                    {
                        result.AddRange(contractBandwidthPoolResult.GetMessageList());
                        result.IsSuccess = false;

                        return;
                    }

                    var contractBandwidthPool = (ContractBandwidthPool)contractBandwidthPoolResult.Item;
                    vif.ContractBandwidthPoolID = contractBandwidthPool.ContractBandwidthPoolID;
                }

                UnitOfWork.VifRepository.Insert(vif);
                await UnitOfWork.SaveAsync();
                var iface = attachment.Interfaces.Single();
                vlan.InterfaceID = iface.InterfaceID;
                vlan.VifID = vif.VifID;
                UnitOfWork.VlanRepository.Insert(vlan);
                await UnitOfWork.SaveAsync();
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

            var attachment = await AttachmentService.GetByIDAsync(request.AttachmentID);
            request.DeviceID = attachment.DeviceID;

            var vif = Mapper.Map<Vif>(request);
            vif.RequiresSync = true;

            try
            {
                var vrfResult = await VrfService.AddAsync(request);
                if (!vrfResult.IsSuccess)
                {
                    result.AddRange(vrfResult.GetMessageList());
                    result.IsSuccess = false;

                    return;
                }

                var vrf = (Vrf)vrfResult.Item;
                vif.VrfID = vrf.VrfID;

                if (request.ContractBandwidthID != null)
                {
                    // Create contract bandwidth pool for the vif

                    var contractBandwidthPoolResult = await ContractBandwidthPoolService.AddAsync(request);
                    if (!contractBandwidthPoolResult.IsSuccess)
                    {
                        result.AddRange(contractBandwidthPoolResult.GetMessageList());
                        result.IsSuccess = false;

                        return;
                    }

                    var contractBandwidthPool = (ContractBandwidthPool)contractBandwidthPoolResult.Item;
                    vif.ContractBandwidthPoolID = contractBandwidthPool.ContractBandwidthPoolID;
                }

                UnitOfWork.VifRepository.Insert(vif);
                await this.UnitOfWork.SaveAsync();

                var ifaces = attachment.Interfaces.ToList();
                var ifaceCount = ifaces.Count;

                for (var i = 1; i <= ifaceCount; i++)
                {
                    var vlan = new Vlan();
                    vlan.InterfaceID = ifaces[i - 1].InterfaceID;
                    vlan.VifID = vif.VifID;
                    
                    if (request.IsLayer3)
                    {
                        if (i == 1)
                        {
                            vlan.IpAddress = request.IpAddress1;
                            vlan.SubnetMask = request.SubnetMask1;
                        }
                        else if (i == 2)
                        {
                            vlan.IpAddress = request.IpAddress2;
                            vlan.SubnetMask = request.SubnetMask2;
                        }
                        else if (i == 3)
                        {
                            vlan.IpAddress = request.IpAddress3;
                            vlan.SubnetMask = request.SubnetMask3;
                        }
                        else if (i == 4)
                        {
                            vlan.IpAddress = request.IpAddress4;
                            vlan.SubnetMask = request.SubnetMask4;
                        }
                    }

                    this.UnitOfWork.VlanRepository.Insert(vlan);
                }
               
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
        /// Check network sync for a VIF associated with an Attachment.
        /// </summary>
        /// <param name="vif"></param>
        /// <returns></returns>
        private  async Task<NetworkSyncServiceResult> CheckNetworkSyncAttachmentVifAsync(Vif vif)
        {
            var result = new NetworkSyncServiceResult { IsSuccess = true };
            var vifServiceModelData = Mapper.Map<AttachmentVifServiceNetModel>(vif.Vlans.Single());

            if (vif.Attachment.IsBundle)
            {
                result = await NetSync.CheckNetworkSyncAsync(vifServiceModelData,
                    $"/attachment/pe/{vif.Attachment.Device.Name}/tagged-attachment-bundle-interface/{vif.Attachment.ID}/vif/{vif.VlanTag}");
            }

            else
            {
                var attachmentServiceModelData = Mapper.Map<AttachmentServiceNetModel>(vif);
                var data = attachmentServiceModelData.TaggedAttachmentInterfaces.Single();

                result = await NetSync.CheckNetworkSyncAsync(vifServiceModelData,
                    $"/attachment/pe/{vif.Attachment.Device.Name}/tagged-attachment-interface/{data.InterfaceType},"
                    + $"{data.InterfaceName.Replace("/", "%2F")}/vif/{vif.VlanTag}");
            }
    
            await UpdateRequiresSyncAsync(vif, !result.IsSuccess, true);

            return result;
        }

        /// <summary>
        /// Check network sync for an VIF associated with a Multi-Port Attachment.
        /// </summary>
        /// <param name="vif"></param>
        /// <returns></returns>
        private async Task<NetworkSyncServiceResult> CheckNetworkSyncMultiPortVifAsync(Vif vif)
        {
            var result = new NetworkSyncServiceResult { IsSuccess = true };
            var tasks = new List<Task<NetworkSyncServiceResult>>();

            foreach (var vlan in vif.Vlans)
            {
                // Create async task to check each vlan 

                var vifServiceModelData = Mapper.Map<MultiPortVifServiceNetModel>(vlan);
                var port = vlan.Interface.Ports.Single();

                tasks.Add(NetSync.CheckNetworkSyncAsync(vifServiceModelData, $"/attachment/pe/{vif.Attachment.Device.Name}" 
                    + $"/tagged-attachment-multiport/{vif.Attachment.ID}"
                    + $"/multiport-member/{port.Type},"
                    + $"{port.Name.Replace("/", "%2F")}/vif/{vif.VlanTag}"));
            }

            // Await for all network checks to complete

            await Task.WhenAll(tasks);

            // Check the results 

            foreach (Task<NetworkSyncServiceResult> t in tasks)
            {
                var r = t.Result;
                if (!r.IsSuccess)
                {
                    result.Messages.AddRange(r.Messages);
                    result.IsSuccess = false;
                }
            }

            await UpdateRequiresSyncAsync(vif, !result.IsSuccess, true);

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

            var attachment = await AttachmentService.GetByIDAsync(request.AttachmentID);
            var currentVifs = attachment.Vifs;

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

                if (currentVifs.Where(q => q.VlanTag == request.RequestedVlanTag).Count() > 0)
                {
                    result.Add($"The requested vlan tag is already assigned.");
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
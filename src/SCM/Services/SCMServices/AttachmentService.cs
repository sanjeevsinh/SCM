﻿using System;
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
using System.Linq.Expressions;
using System.Net.Http;

namespace SCM.Services.SCMServices
{
    public class AttachmentService : BaseService, IAttachmentService
    {

        public AttachmentService(IUnitOfWork unitOfWork, IMapper mapper,
            INetworkSyncService netSync, IVrfService vrfService, IContractBandwidthPoolService contractBandwidthPoolService) : base(unitOfWork, mapper, netSync)
        {
            VrfService = vrfService;
            ContractBandwidthPoolService = contractBandwidthPoolService;
        }
        private IVrfService VrfService { get; set; }
        private IContractBandwidthPoolService ContractBandwidthPoolService { get; set; }

        public async Task<Attachment> GetByIDAsync(int id)
        {
            var dbResult = await UnitOfWork.AttachmentRepository.GetAsync(q => q.AttachmentID == id,
                includeProperties: "Tenant,"
                + "Device.Location.SubRegion.Region,"
                + "Device.Plane,"
                + "Vrf.BgpPeers,"
                + "Vrf.AttachmentSetVrfs.AttachmentSet,"
                + "AttachmentBandwidth,"
                + "ContractBandwidthPool.ContractBandwidth,"
                + "Interfaces.Device,"
                + "Interfaces.Ports.Device,"
                + "Interfaces.Ports.PortBandwidth,"
                + "Interfaces.Ports.Interface.Vlans.Vif,"
                + "Vifs.Vrf.BgpPeers,"
                + "Vifs.Vlans.Vif.ContractBandwidthPool,"
                + "Vifs.ContractBandwidthPool.ContractBandwidth",
                AsTrackable: false);

            return dbResult.SingleOrDefault();
        }

        /// <summary>
        /// Return an Attachment from the VRF which the Attachment is associated with.
        /// </summary>
        /// <param name="vrfID"></param>
        /// <returns></returns>
        public async Task<Attachment> GetByVrfIDAsync(int vrfID)
        {
            var dbResult = await UnitOfWork.AttachmentRepository.GetAsync(q => q.VrfID == vrfID,
                includeProperties: "Tenant,"
                + "Device.Location.SubRegion.Region,"
                + "Device.Plane,"
                + "Vrf.BgpPeers,"
                + "Vrf.AttachmentSetVrfs.AttachmentSet,"
                + "AttachmentBandwidth,"
                + "ContractBandwidthPool.ContractBandwidth,"
                + "Interfaces.Device,"
                + "Interfaces.Ports.Device,"
                + "Interfaces.Ports.PortBandwidth,"
                + "Interfaces.Ports.Interface.Vlans.Vif,"
                + "Vifs.Vrf.BgpPeers,"
                + "Vifs.Vlans.Vif.ContractBandwidthPool,"
                + "Vifs.ContractBandwidthPool.ContractBandwidth", AsTrackable: false);

            return dbResult.SingleOrDefault();
        }

        public async Task<IEnumerable<Attachment>> GetAllByTenantIDAsync(int tenantID)
        {
            var dbresult = await UnitOfWork.TenantRepository.GetAsync(q => q.TenantID == tenantID, AsTrackable: false);
            var tenant = dbresult.SingleOrDefault();

            if (tenant != null)
            {
                return await GetAllByTenantAsync(tenant);
            }
            else
            {
                return Enumerable.Empty<Attachment>();
            }
        }

        public async Task<IEnumerable<Attachment>> GetAllByTenantAsync(Tenant tenant)
        {
            var attachments = await UnitOfWork.AttachmentRepository.GetAsync(q => q.TenantID == tenant.TenantID,
                includeProperties: "Tenant,"
                + "Device.Location.SubRegion.Region,"
                + "Device.Plane,"
                + "Vrf.BgpPeers,"
                + "Vrf.AttachmentSetVrfs.AttachmentSet,"
                + "AttachmentBandwidth,"
                + "ContractBandwidthPool.ContractBandwidth,"
                + "Interfaces.Device,"
                + "Interfaces.Ports.Device,"
                + "Interfaces.Ports.PortBandwidth,"
                + "Interfaces.Ports.Interface.Vlans.Vif,"
                + "Vifs.Vrf.BgpPeers,"
                + "Vifs.Vlans.Vif.ContractBandwidthPool,"
                + "Vifs.ContractBandwidthPool.ContractBandwidth",
                AsTrackable: false);

            return attachments.ToList();
        }

        /// <summary>
        /// Return all Attachments for a given vpn.
        /// </summary>
        /// <param name="attachmentID"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Attachment>> GetAllByVpnIDAsync(int vpnID)
        {
            var result = await UnitOfWork.AttachmentRepository
                .GetAsync(q => q.Vrf.AttachmentSetVrfs
                .SelectMany(r => r.AttachmentSet.VpnAttachmentSets)
                .Where(s => s.VpnID == vpnID)
                .Count() > 0,
                includeProperties: "Tenant,"
                + "Device.Location.SubRegion.Region,"
                + "Device.Plane,"
                + "Vrf.BgpPeers,"
                + "Vrf.AttachmentSetVrfs.AttachmentSet,"
                + "AttachmentBandwidth,"
                + "ContractBandwidthPool.ContractBandwidth,"
                + "Interfaces.Device,"
                + "Interfaces.Ports.Device,"
                + "Interfaces.Ports.PortBandwidth,"
                + "Interfaces.Ports.Interface.Vlans.Vif,"
                + "Vifs.Vrf.BgpPeers,"
                + "Vifs.Vlans.Vif.ContractBandwidthPool,"
                + "Vifs.ContractBandwidthPool.ContractBandwidth",
                AsTrackable: false);

            return result.ToList();
        }

        public async Task<ServiceResult> AddAsync(AttachmentRequest request)
        {
            // Request some ports

            request = await RequestPortsAsync(request);

            if (request.Ports.Count() == 0)
            {
                var result = new ServiceResult();
                result.AddRange(request.Errors);

                return result;
            }

            // Hand off to method to generate the attachment from the allocated ports

            if (request.BundleRequired)
            {
                return await AddBundleAttachmentAsync(request);
            }
            else if (request.MultiPortRequired)
            {
                return await AddMultiPortAsync(request);
            }
            else
            {
                return await AddAttachmentAsync(request);
            }
        }

        public async Task<int> UpdateAsync(Attachment attachment)
        {
            this.UnitOfWork.AttachmentRepository.Update(attachment);

            return await this.UnitOfWork.SaveAsync();
        }

        /// <summary>
        /// Perform shallow check of network sync state of a collection
        /// of attachments by checking the 'RequiresSync' property.
        /// </summary>
        /// <param name="attachments"></param>
        /// <returns></returns>
        public ServiceResult ShallowCheckNetworkSync(IEnumerable<Attachment> attachments)
        {
            var result = new ServiceResult { IsSuccess = true };

            var attachmentsRequireSync = attachments.Where(q => q.RequiresSync);
            if (attachmentsRequireSync.Count() > 0)
            {
                result.IsSuccess = false;
                result.Add("The following attachments require synchronisation with the network:.");
                attachmentsRequireSync.ToList().ForEach(f => result.Add($"'{f.Name}'."));
            }

            return result;
        }

        public async Task<ServiceResult> CheckNetworkSyncAsync(Attachment attachment)
        {
            var result = new ServiceResult
            {
                IsSuccess = true,
                Item = attachment,
                Context = attachment.Tenant
            };

            NetworkSyncServiceResult syncResult;

            if (attachment.IsTagged)
            {
                if (attachment.IsBundle)
                {
                    var data = Mapper.Map<TaggedAttachmentBundleInterfaceServiceNetModel>(attachment);
                    syncResult = await NetSync.CheckNetworkSyncAsync(data,
                        $"/attachment/pe/{attachment.Device.Name}/tagged-attachment-bundle-interface/{data.BundleID}");
                }

                else if (attachment.IsMultiPort)
                {
                    var data = Mapper.Map<TaggedAttachmentMultiPortServiceNetModel>(attachment);
                    syncResult = await NetSync.CheckNetworkSyncAsync(data,
                       $"/attachment/pe/{attachment.Device.Name}/tagged-attachment-multiport/{data.Name}");
                }
                else
                {
                    var data = Mapper.Map<TaggedAttachmentInterfaceServiceNetModel>(attachment);
                    syncResult = await NetSync.CheckNetworkSyncAsync(data,
                        $"/attachment/pe/{attachment.Device.Name}/tagged-attachment-interface/{data.InterfaceType},"
                        + data.InterfaceName.Replace("/", "%2F"));
                }
            }
            else
            {
                if (attachment.IsBundle)
                {
                    var data = Mapper.Map<UntaggedAttachmentBundleInterfaceServiceNetModel>(attachment);
                    syncResult = await NetSync.CheckNetworkSyncAsync(data,
                            $"/attachment/pe/{attachment.Device.Name}/untagged-attachment-bundle-interface/{data.BundleID}");
                }
                else if (attachment.IsMultiPort)
                {
                    var data = Mapper.Map<UntaggedAttachmentMultiPortServiceNetModel>(attachment);
                    syncResult = await NetSync.CheckNetworkSyncAsync(data,
                        $"/attachment/pe/{attachment.Device.Name}/untagged-attachment-multiport/{data.Name}");
                }
                else
                {
                    var data = Mapper.Map<UntaggedAttachmentInterfaceServiceNetModel>(attachment);
                    syncResult = await NetSync.CheckNetworkSyncAsync(data,
                        $"/attachment/pe/{attachment.Device.Name}/untagged-attachment-interface/{data.InterfaceType},"
                        + data.InterfaceName.Replace("/", "%2F"));
                }
            }

            result.NetworkSyncServiceResults.Add(syncResult);

            if (!syncResult.IsSuccess)
            {
                result.IsSuccess = false;

                if (syncResult.StatusCode == NetworkSyncStatusCode.Success)
                {
                    // Request was successfully executed and the Attachment was tested for sync with the network

                    result.Add($"Attachment '{attachment.Name}' is not synchronised with the network.");
                }
                else
                {
                    // Request failed to execute for some reason - e.g server down, no network etc

                    result.Add($"There was an error checking status for attachment '{attachment.Name}'.");
                }
            }

            return result;
        }

        public async Task<IEnumerable<ServiceResult>> CheckNetworkSyncAsync(IEnumerable<Attachment> attachments,
            IProgress<ServiceResult> progress)
        {
            List<Task<ServiceResult>> tasks = (from attachment in attachments select SyncToNetworkAsync(attachment)).ToList();
            var results = new List<ServiceResult>();

            while (tasks.Count() > 0)
            {
                Task<ServiceResult> task = await Task.WhenAny(tasks);
                results.Add(task.Result);
                tasks.Remove(task);

                // Update caller with progress

                progress.Report(task.Result);
            }

            await Task.WhenAll(tasks);

            return results;
        }

        public async Task<ServiceResult> SyncToNetworkAsync(Attachment attachment)
        {
            var result = new ServiceResult
            {
                IsSuccess = true,
                Item = attachment,
                Context = attachment.Tenant
            };

            var serviceModelData = Mapper.Map<AttachmentServiceNetModel>(attachment);
            var syncResult = await NetSync.SyncNetworkAsync(serviceModelData, $"/attachment/pe/{attachment.Device.Name}", new HttpMethod("PATCH"));

            result.NetworkSyncServiceResults.Add(syncResult);

            if (!syncResult.IsSuccess)
            {
                result.IsSuccess = false;

                if (syncResult.StatusCode == NetworkSyncStatusCode.Success)
                {
                    // Request was successfully executed but synchronisation failed

                    result.Add($"Failed to synchronise attachmnet '{attachment.Name}' with the network.");
                }
                else
                {
                    // Request failed to execute for some reason - e.g server down, no network etc

                    result.Add($"There was an error synchronising attachment '{attachment.Name}' with the network.");
                }
            }

            return result;
        }

        public async Task<IEnumerable<ServiceResult>> SyncToNetworkAsync(IEnumerable<Attachment> attachments,
            IProgress<ServiceResult> progress)
        {
            List<Task<ServiceResult>> tasks = (from attachment in attachments select SyncToNetworkAsync(attachment)).ToList();
            var results = new List<ServiceResult>();

            while (tasks.Count() > 0)
            {
                Task<ServiceResult> task = await Task.WhenAny(tasks);
                results.Add(task.Result);
                tasks.Remove(task);

                // Update caller with progress

                progress.Report(task.Result);
            }

            await Task.WhenAll(tasks);

            return results;
        }

        /// <summary>
        /// Delete an attachment from the network and from the inventory
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns></returns>
        public async Task<ServiceResult> DeleteAsync(Attachment attachment)
        {
            if (attachment.IsBundle)
            {
                return await DeleteBundleAttachmentAsync(attachment);
            }
            else if (attachment.IsMultiPort)
            {
                return await DeleteMultiPortAttachmentAsync(attachment);
            }
            else
            {
                return await DeleteAttachmentAsync(attachment);
            }
        }

        public async Task<ServiceResult> DeleteFromNetworkAsync(Attachment attachment)
        {
            var networkSyncServiceResults = new List<NetworkSyncServiceResult>();
            var result = new ServiceResult { IsSuccess = true };
            var taskResult = new NetworkSyncServiceResult();

            var serviceModelData = Mapper.Map<AttachmentServiceNetModel>(attachment);

            if (serviceModelData.TaggedAttachmentBundleInterfaces.Count > 0)
            {
                var data = serviceModelData.TaggedAttachmentBundleInterfaces.Single();
                taskResult = await NetSync.DeleteFromNetworkAsync($"/attachment/pe/{attachment.Device.Name}"
                    + $"/tagged-attachment-bundle-interface/{data.BundleID}");
            }
            else if (serviceModelData.TaggedAttachmentInterfaces.Count > 0)
            {
                var data = serviceModelData.TaggedAttachmentInterfaces.Single();
                taskResult = await NetSync.DeleteFromNetworkAsync($"/attachment/pe/{attachment.Device.Name}"
                   + $"/tagged-attachment-interface/{data.InterfaceType},"
                   + data.InterfaceName.Replace("/", "%2F"));
            }
            else if (serviceModelData.TaggedAttachmentMultiPorts.Count > 0)
            {
                var data = serviceModelData.TaggedAttachmentMultiPorts.Single();
                taskResult = await NetSync.DeleteFromNetworkAsync($"/attachment/pe/{attachment.Device.Name}"
                  + $"/tagged-attachment-multiport/{data.Name}");
            }
            else if (serviceModelData.UntaggedAttachmentBundleInterfaces.Count > 0)
            {
                var data = serviceModelData.UntaggedAttachmentBundleInterfaces.Single();
                taskResult = await NetSync.DeleteFromNetworkAsync($"/attachment/pe/{attachment.Device.Name}"
                   + $"/untagged-attachment-bundle-interface/{data.BundleID}");
            }
            else if (serviceModelData.UntaggedAttachmentInterfaces.Count > 0)
            {
                var data = serviceModelData.UntaggedAttachmentInterfaces.Single();
                taskResult = await NetSync.DeleteFromNetworkAsync($"/attachment/pe/{attachment.Device.Name}"
                   + $"/untagged-attachment-interface/{data.InterfaceType},"
                   + data.InterfaceName.Replace("/", "%2F"));
            }
            else if (serviceModelData.UntaggedAttachmentMultiPorts.Count > 0)
            {
                var data = serviceModelData.UntaggedAttachmentMultiPorts.Single();
                taskResult = await NetSync.DeleteFromNetworkAsync($"/attachment/pe/{attachment.Device.Name}"
                    + $"/untagged-attachment-multiport/{data.Name}");
            }

            var vrfTasks = new List<Task<NetworkSyncServiceResult>>();
            if (serviceModelData.Vrfs.Count > 0)
            {
                vrfTasks.AddRange(serviceModelData.Vrfs.Select(q => NetSync.DeleteFromNetworkAsync($"/attachment/pe/{attachment.Device.Name}"
                        + $"/vrf/{q.VrfName},{q.EnableLayer3.ToString().ToLower()}")));
            }

            result.IsSuccess = taskResult.IsSuccess;
            result.AddRange(taskResult.Messages);

            // Wait for deletion of vrfs from the network to complete

            await Task.WhenAll(vrfTasks);

            // Check results

            foreach (var t in vrfTasks)
            {
                result.NetworkSyncServiceResults.Add(t.Result);

                if (!t.Result.IsSuccess)
                {
                    result.IsSuccess = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Validates an attachment request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ServiceResult> ValidateNewAsync(AttachmentRequest request)
        {
            var result = new ServiceResult { IsSuccess = true };

            var bandwidth = await UnitOfWork.AttachmentBandwidthRepository.GetByIDAsync(request.BandwidthID);
            request.Bandwidth = bandwidth;

            if (request.BundleRequired)
            {
                if (!request.Bandwidth.SupportedByBundle)
                {
                    result.Add($"The requested bandwidth ({request.Bandwidth.BandwidthGbps} Gbps) is not supported by a bundle attachment. "
                        + "Uncheck the bundle option to request this bandwidth.");
                    result.IsSuccess = false;

                    return result;
                }
            }
            else if (request.MultiPortRequired)
            {
                if (!request.Bandwidth.SupportedByMultiPort)
                {
                    result.Add($"The requested bandwidth ({request.Bandwidth.BandwidthGbps} Gbps) is not supported by a multi-port attachment. "
                        + "Uncheck the multi-port option to request this bandwidth.");
                    result.IsSuccess = false;

                    return result;
                }
            }
            else
            {
                if (request.Bandwidth.MustBeBundleOrMultiPort)
                {
                    result.Add($"The requested bandwidth ({request.Bandwidth.BandwidthGbps} Gbps) is ONLY supported by a bundle or multi-port attachment. "
                       + "Check the bundle or multi-port option to request this bandwidth.");

                    result.IsSuccess = false;

                    return result;
                }
            }

            if (request.IsLayer3)
            {
                if (request.MultiPortRequired)
                {
                    if (request.Bandwidth.BandwidthGbps == 20)
                    {
                        if (string.IsNullOrEmpty(request.IpAddress1) || string.IsNullOrEmpty(request.IpAddress2))
                        {
                            result.Add("Two IP addresses must be entered.");
                            result.IsSuccess = false;

                            return result;
                        }
                        if (string.IsNullOrEmpty(request.SubnetMask1) || string.IsNullOrEmpty(request.SubnetMask2))
                        {
                            result.Add("Two subnet masks must be entered.");
                            result.IsSuccess = false;

                            return result;
                        }
                    }
                    else if (request.Bandwidth.BandwidthGbps == 40)
                    {
                        if (string.IsNullOrEmpty(request.IpAddress1) || string.IsNullOrEmpty(request.IpAddress2)
                            || string.IsNullOrEmpty(request.IpAddress3) || string.IsNullOrEmpty(request.IpAddress4))
                        {
                            result.Add("Four IP addresses must be entered.");
                            result.IsSuccess = false;

                            return result;
                        }
                        if (string.IsNullOrEmpty(request.SubnetMask1) || string.IsNullOrEmpty(request.SubnetMask2)
                            || string.IsNullOrEmpty(request.SubnetMask3) || string.IsNullOrEmpty(request.SubnetMask4))
                        {
                            result.Add("Four subnet masks must be entered.");
                            result.IsSuccess = false;

                            return result;
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(request.IpAddress1))
                    {
                        result.Add("An IP address must be specified for layer 3 attachments.");
                        result.IsSuccess = false;

                        return result;
                    }
                    if (string.IsNullOrEmpty(request.SubnetMask1))
                    {
                        result.Add("A subnet mask must be specified for layer 3 attachments.");
                        result.IsSuccess = false;

                        return result;
                    }
                }
            }

            if (!request.IsTagged)
            {
                // Validate the requested Contract Bandwidth Pool

                var validateContractBandwidthPoolResult = await ContractBandwidthPoolService.ValidateNewAsync(request);
                if (!validateContractBandwidthPoolResult.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.AddRange(validateContractBandwidthPoolResult.GetMessageList());

                    return result;
                }
            }

            return result;
        }

        public async Task<ServiceResult> ValidateAsync(Vpn vpn)
        {
            var result = new ServiceResult { IsSuccess = true };

            var attachments = await GetAllByVpnIDAsync(vpn.VpnID);
            var attachmentsRequireSync = attachments.Where(q => q.RequiresSync).ToList();

            if (attachmentsRequireSync.Count() > 0) {

                result.IsSuccess = false;
                result.Add("The following attachments for the VPN require synchronisation with the network:");
                attachmentsRequireSync.ForEach(a => result.Add($"'{a.Name}' on device '{a.Device.Name}' for tenant '{a.Tenant.Name}'."));
            }

            return result;
        }

        /// <summary>
        /// Update the RequiresSync property of an Attachment record.
        /// </summary>
        /// <param name="iface"></param>
        /// <param name="requiresSync"></param>
        /// <returns></returns>
        public async Task UpdateRequiresSyncAsync(Attachment attachment, bool requiresSync, bool saveChanges = true)
        {
            attachment.RequiresSync = requiresSync;
            UnitOfWork.AttachmentRepository.Update(attachment);
            if (saveChanges)
            {
                await UnitOfWork.SaveAsync();
            }

            return;
        }

        /// <summary>
        /// Update the RequiresSync property of an Attachment record.
        /// </summary>
        /// <returns></returns>
        public async Task UpdateRequiresSyncAsync(int id, bool requiresSync, bool saveChanges = true)
        {
            var attachment = await UnitOfWork.AttachmentRepository.GetByIDAsync(id);
            await UpdateRequiresSyncAsync(attachment, requiresSync, saveChanges);

            return;
        }

        /// <summary>
        /// Helper to request some ports for a new attachment
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<AttachmentRequest> RequestPortsAsync(AttachmentRequest request) {

            // Work out the number of ports we need to allocate and 
            // the port bandwidth required

            if (request.BundleRequired || request.MultiPortRequired)
            {
                // For bundles and multiport requests we need at least 2 ports. Work out the number of ports required from 
                // the request data based upon the required bandwidth of the attachment (e.g. 20Gbp/s) and the 
                // per port bandwidth needed to satisfy the required bandwidth (e.g. 10Gb/s)

                request.NumPortsRequired = request.Bandwidth.BandwidthGbps / request.Bandwidth.BundleOrMultiPortMemberBandwidthGbps.Value;
                request.PortBandwidthRequired = request.Bandwidth.BandwidthGbps / request.NumPortsRequired;
            }
            else
            {
                // The request is not for a bundle or multiport so the number of ports required must be 1

                request.NumPortsRequired = 1;
                request.PortBandwidthRequired = request.Bandwidth.BandwidthGbps;
            }

            // Try and find some ports which satisfy the request

            request = await FindPortsAsync(request);

            // Do we have some ports? If not quit. 

            if (request.Ports.Count() == 0)
            {
                return request;
            }

            return request;
        }

        private async Task<AttachmentRequest> FindPortsAsync(AttachmentRequest request)
        {
            request = await FindDeviceAsync(request);

            if (request.Device == null)
            {
                return request;
            }

            var ports = request.Device.Ports.Where(q => q.TenantID == null && q.PortBandwidth.BandwidthGbps == request.PortBandwidthRequired);

            if (ports.Count() == 0)
            {
                request.Errors.Add("No ports matching the requested bandwidth parameter could not be found. "
                    + "Please change your request and try again, or contact your system adminstrator and report this issue.");
            }

            if (ports.Count() < request.NumPortsRequired)
            {
                request.Errors.Add($"The number of ports available ({ports.Count()}) is less than the number required ({request.NumPortsRequired}). "
                    + "Please change your request and try again, or contact your system adminstrator and report this issue.");
            }

            request.Ports = ports.Take(request.NumPortsRequired);

            return request;
        }

        private async Task<AttachmentRequest> FindDeviceAsync(AttachmentRequest request)
        {
            // Find all devices in the requested location

            var devices = await UnitOfWork.DeviceRepository.GetAsync(q => q.LocationID == request.LocationID,
                includeProperties: "Ports.PortBandwidth,Ports.Device,Attachments");

            // Filter devices collection to include only those devices which belong to the requested plane (if specified)

            if (request.PlaneID != null)
            {
                devices = devices.Where(q => q.PlaneID == request.PlaneID).ToList();
            }

            // Filter devices collection to only those devices which have the required number of free ports
            // of the required bandwidth.
            // Free ports are not already assigned to a tenant.

            devices = devices.Where(q => q.Ports.Where(p => p.TenantID == null
                && p.PortBandwidth.BandwidthGbps == request.PortBandwidthRequired).Count() >= request.NumPortsRequired).ToList();

            if (devices.Count == 0)
            {
                request.Errors.Add("Ports matching the requested location and "
                    + "bandwidth parameters could not be found. "
                    + "Please change the input parameters and try again, "
                    + "or contact your system adminstrator to report this issue.");

                return request;
            }
            else if (devices.Count > 1)
            {
                // Get device with the least number of tenant-assigned ports.

                request.Device = devices.Aggregate((current, x) =>
                (x.Ports.Where(p => p.TenantID != null).Count() < current.Ports.Where(p => p.TenantID != null).Count() ? x : current));
            }
            else
            {
                request.Device = devices.Single();
            }

            return request;
        }

        /// <summary>
        /// Add an attachment to the inventory
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<ServiceResult> AddAttachmentAsync(AttachmentRequest request)
        {
            var result = new ServiceResult
            {
                IsSuccess = true
            };

            var attachment = Mapper.Map<Attachment>(request);
            attachment.AttachmentBandwidthID = request.BandwidthID;
            attachment.RequiresSync = true;
            attachment.Created = true;

            var iface = new Interface();
            iface.DeviceID = request.Device.ID;

            if (request.IsLayer3)
            {
                iface.IpAddress = request.IpAddress1;
                iface.SubnetMask = request.SubnetMask1;
            }

            // Need to implement Transaction Scope here when available in dotnet core

            try
            {
                if (!request.IsTagged)
                {
                    var vrfResult = await VrfService.AddAsync(request);
                    if (!vrfResult.IsSuccess)
                    {
                        result.AddRange(vrfResult.GetMessageList());
                        result.IsSuccess = false;

                        return result;
                    }

                    var vrf = (Vrf)vrfResult.Item;
                    attachment.VrfID = vrf.VrfID;

                    var contractBandwidthPoolResult = await ContractBandwidthPoolService.AddAsync(request);
                    if (!contractBandwidthPoolResult.IsSuccess)
                    {
                        result.AddRange(contractBandwidthPoolResult.GetMessageList());
                        result.IsSuccess = false;

                        return result;
                    }

                    var contractBandwidthPool = (ContractBandwidthPool)contractBandwidthPoolResult.Item;
                    attachment.ContractBandwidthPoolID = contractBandwidthPool.ContractBandwidthPoolID;
                }

                UnitOfWork.AttachmentRepository.Insert(attachment);
                await UnitOfWork.SaveAsync();
                iface.AttachmentID = attachment.AttachmentID;
                UnitOfWork.InterfaceRepository.Insert(iface);

                await UnitOfWork.SaveAsync();

                var port = request.Ports.Single();
                port.TenantID = request.TenantID;
                port.InterfaceID = iface.InterfaceID;
                UnitOfWork.PortRepository.Update(port);

                await UnitOfWork.SaveAsync();

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

        /// <summary>
        /// Add a bundle attachment to the inventory
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<ServiceResult> AddBundleAttachmentAsync(AttachmentRequest request)
        {
            var result = new ServiceResult
            {
                IsSuccess = true
            };

            var attachment = Mapper.Map<Attachment>(request);
            attachment.AttachmentBandwidthID = request.BandwidthID;
            attachment.RequiresSync = true;
            attachment.Created = true;

            var bundleIface = new Interface();

            if (request.IsLayer3)
            {
                bundleIface.IpAddress = request.IpAddress1;
                bundleIface.SubnetMask = request.SubnetMask1;
            }

            var port = request.Ports.First();
            bundleIface.DeviceID = port.DeviceID;

            var usedBundleIDs = port.Device.Attachments.Where(q => q.IsBundle).Select(q => q.ID).Where(q => q != null);

            // Find the first un-used bundle ID in the range 1, 65535 (this is the Cisco IOS-XR allowable range for bundle IDs).

            attachment.ID = Enumerable.Range(1, 65535).Except(usedBundleIDs.Select(q => q.Value)).First();
           
            // Need to implement Transaction Scope here when available in dotnet core

            try
            {
                if (!request.IsTagged)
                {
                    var vrfResult = await VrfService.AddAsync(request);
                    if (!vrfResult.IsSuccess)
                    {
                        result.AddRange(vrfResult.GetMessageList());
                        result.IsSuccess = false;

                        return result;
                    }

                    var vrf = (Vrf)vrfResult.Item;
                    attachment.VrfID = vrf.VrfID;

                    var contractBandwidthPoolResult = await ContractBandwidthPoolService.AddAsync(request);
                    if (!contractBandwidthPoolResult.IsSuccess)
                    {
                        result.AddRange(contractBandwidthPoolResult.GetMessageList());
                        result.IsSuccess = false;

                        return result;
                    }

                    var contractBandwidthPool = (ContractBandwidthPool)contractBandwidthPoolResult.Item;
                    attachment.ContractBandwidthPoolID = contractBandwidthPool.ContractBandwidthPoolID;
                }

                UnitOfWork.AttachmentRepository.Insert(attachment);
                await UnitOfWork.SaveAsync();
                bundleIface.AttachmentID = attachment.AttachmentID;
                UnitOfWork.InterfaceRepository.Insert(bundleIface);

                await this.UnitOfWork.SaveAsync();

                foreach (var p in request.Ports)
                {
                    p.TenantID = request.TenantID;
                    p.InterfaceID = bundleIface.InterfaceID;
                    UnitOfWork.PortRepository.Update(p);
                }

                await UnitOfWork.SaveAsync();
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

        /// <summary>
        /// Add a multiport attachment to the inventory
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<ServiceResult> AddMultiPortAsync(AttachmentRequest request)
        {
            var result = new ServiceResult
            {
                IsSuccess = true
            };

            var port = request.Ports.First();
            var attachment = Mapper.Map<Attachment>(request);

            var usedIds = port.Device.Attachments.Where(q => q.IsMultiPort).Select(q => q.ID).Where(q => q != null);

            // Find the first un-used identifier in the range 1, 65535 (this range is fairly arbitrary - the identifer is not used
            // for network configuration of a multiport. It serves only to create a unique multiport ID which is used
            // in the NSO service models).

            attachment.ID = Enumerable.Range(1, 65535).Except(usedIds.Select(q => q.Value)).First();

            attachment.DeviceID = request.Device.ID;
            attachment.AttachmentBandwidthID = request.BandwidthID;
            attachment.RequiresSync = true;
            attachment.Created = true;

            try
            {
                // Need to implement Transaction Scope here when available in dotnet core

                Vrf vrf = null;
                if (!request.IsTagged)
                {
                    var vrfResult = await VrfService.AddAsync(request);
                    if (!vrfResult.IsSuccess)
                    {
                        result.AddRange(vrfResult.GetMessageList());
                        result.IsSuccess = false;

                        return result;
                    }

                    vrf = (Vrf)vrfResult.Item;
                    attachment.VrfID = vrf.VrfID;

                    // Create contract bandwidth pool

                    var contractBandwidthPoolResult = await ContractBandwidthPoolService.AddAsync(request);
                    if (!contractBandwidthPoolResult.IsSuccess)
                    {
                        result.AddRange(contractBandwidthPoolResult.GetMessageList());
                        result.IsSuccess = false;

                        return result;
                    }

                    var contractBandwidthPool = (ContractBandwidthPool)contractBandwidthPoolResult.Item;
                    attachment.ContractBandwidthPoolID = contractBandwidthPool.ContractBandwidthPoolID;
                }

                // Generate key for the multiport attachment record

                UnitOfWork.AttachmentRepository.Insert(attachment);
                await UnitOfWork.SaveAsync();

                // Create interface records, one for each member interface 
                // of the multiport

                var ports = request.Ports.ToList();
                var portCount = ports.Count();

                for (var i = 1; i <= portCount; i++)
                {
                    var iface = new Interface();
                    iface.DeviceID = request.Device.ID;
                    iface.AttachmentID = attachment.AttachmentID;

                    if (request.IsLayer3)
                    {
                        if (i == 1)
                        {
                            iface.IpAddress = request.IpAddress1;
                            iface.SubnetMask = request.SubnetMask1;
                        }
                        else if (i == 2)
                        {
                            iface.IpAddress = request.IpAddress2;
                            iface.SubnetMask = request.SubnetMask2;
                        }
                        else if (i == 3)
                        {
                            iface.IpAddress = request.IpAddress3;
                            iface.SubnetMask = request.SubnetMask3;
                        }
                        else if (i == 4)
                        {
                            iface.IpAddress = request.IpAddress4;
                            iface.SubnetMask = request.SubnetMask4;
                        }
                    }

                    UnitOfWork.InterfaceRepository.Insert(iface);

                    // Assign the port to the tenant and make it a member
                    // of the multiport

                    var p = ports[i - 1];
                    p.TenantID = request.TenantID;
                    p.InterfaceID = iface.InterfaceID;
                    UnitOfWork.PortRepository.Update(p);
                }

                await UnitOfWork.SaveAsync();
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

        /// <summary>
        /// Delete an attachment from inventory
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns></returns>
        private async Task<ServiceResult> DeleteAttachmentAsync(Attachment attachment)
        {
            var result = new ServiceResult
            {
                IsSuccess = true
            };

            try
            {
                if (attachment.IsTagged)
                {
                    // Delete VIFs

                    foreach (var vif in attachment.Vifs)
                    {

                        foreach (var vlan in vif.Vlans)
                        {
                            await UnitOfWork.VlanRepository.DeleteAsync(vlan.VlanID);
                        }

                        await UnitOfWork.VifRepository.DeleteAsync(vif.VifID);
                        await UnitOfWork.VrfRepository.DeleteAsync(vif.VrfID);

                        // Clean up Contract Bandwidth Pool

                        await UnitOfWork.ContractBandwidthPoolRepository.DeleteAsync(vif.ContractBandwidthPoolID);
                    }
                }
                else
                {
                    // Delete VRF and Contract Bandwidth Pool for an untagged attachment

                    await UnitOfWork.VrfRepository.DeleteAsync(attachment.VrfID);
                    await UnitOfWork.ContractBandwidthPoolRepository.DeleteAsync(attachment.ContractBandwidthPoolID);
                }


                var iface = attachment.Interfaces.Single();
                var port = await UnitOfWork.PortRepository.GetByIDAsync(iface.Ports.Single().ID);
                port.TenantID = null;
                port.InterfaceID = null;

                UnitOfWork.PortRepository.Update(port);
                await UnitOfWork.InterfaceRepository.DeleteAsync(iface.InterfaceID);
                await UnitOfWork.AttachmentRepository.DeleteAsync(attachment.AttachmentID);

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
        /// Delete a bundle attachment from inventory
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns></returns>
        private async Task<ServiceResult> DeleteBundleAttachmentAsync(Attachment attachment)
        {
            var result = new ServiceResult { IsSuccess = true };

            var iface = attachment.Interfaces.Single();
            var ports = await UnitOfWork.PortRepository.GetAsync(q => q.InterfaceID == iface.InterfaceID);

            try
            {
                foreach (var port in ports)
                {
                    port.TenantID = null;
                    port.InterfaceID = null;
                    UnitOfWork.PortRepository.Update(port);
                }

                if (attachment.IsTagged)
                {
                    foreach (var vif in attachment.Vifs)
                    {
                        foreach (var vlan in vif.Vlans)
                        {
                            await UnitOfWork.VlanRepository.DeleteAsync(vlan.VlanID);
                        }

                        await UnitOfWork.VifRepository.DeleteAsync(vif.VifID);
                        await UnitOfWork.VrfRepository.DeleteAsync(vif.VrfID);

                        // Clean up Contract Bandwidth Pool

                        await UnitOfWork.ContractBandwidthPoolRepository.DeleteAsync(vif.ContractBandwidthPoolID);
                    }
                }
                else { 

                    // Delete VRF and Contract Bandwidth Pool for an untagged attachment

                    await UnitOfWork.VrfRepository.DeleteAsync(attachment.VrfID);
                    await UnitOfWork.ContractBandwidthPoolRepository.DeleteAsync(attachment.ContractBandwidthPoolID);
                }

                await UnitOfWork.InterfaceRepository.DeleteAsync(iface.InterfaceID);
                await UnitOfWork.AttachmentRepository.DeleteAsync(attachment.AttachmentID);

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
        /// Delete a multiport attachment from inventory
        /// </summary>
        /// <returns></returns>
        private async Task<ServiceResult> DeleteMultiPortAttachmentAsync(Attachment attachment)
        {
            var result = new ServiceResult { IsSuccess = true };

            var interfaces = attachment.Interfaces;
            var ports = await UnitOfWork.PortRepository.GetAsync(q => q.Interface.AttachmentID == attachment.AttachmentID);

            try
            {
                foreach (var port in ports)
                {
                    port.TenantID = null;
                    port.InterfaceID = null;
                    UnitOfWork.PortRepository.Update(port);
                }

                foreach (var iface in interfaces)
                {
                    await UnitOfWork.InterfaceRepository.DeleteAsync(iface.InterfaceID);
                }

                if (attachment.IsTagged)
                {

                    // Delete vifs of a tagged attachment

                    foreach (var vif in attachment.Vifs)
                    {
                        foreach (var vlan in vif.Vlans)
                        {
                            await UnitOfWork.VlanRepository.DeleteAsync(vlan.VlanID);
                        }

                        await UnitOfWork.VifRepository.DeleteAsync(vif.VifID);
                        await UnitOfWork.VrfRepository.DeleteAsync(vif.VrfID);

                        // Clean up Contract Bandwidth Pool

                        await UnitOfWork.ContractBandwidthPoolRepository.DeleteAsync(vif.ContractBandwidthPoolID);
                    }
                } 
                else
                { 
                    // Delete VRF and Contract Bandwidth Pool for an untagged attachment

                    await UnitOfWork.VrfRepository.DeleteAsync(attachment.VrfID);
                    await UnitOfWork.ContractBandwidthPoolRepository.DeleteAsync(attachment.ContractBandwidthPoolID);
                }

                await UnitOfWork.AttachmentRepository.DeleteAsync(attachment.AttachmentID);
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
    }
}
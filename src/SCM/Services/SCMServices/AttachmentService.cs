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
using System.Linq.Expressions;

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

        public async Task<Attachment> GetByIDAsync(int id)
        {

            var dbResult = await UnitOfWork.InterfaceRepository.GetAsync(q => q.InterfaceID == id, includeProperties:
                "Device.Location,Device.Plane,Vrf.BgpPeers,InterfaceBandwidth,ContractBandwidthPool.ContractBandwidth,"
                + "Tenant,Port,BundleInterfacePorts.Port,InterfaceVlans.Vrf.BgpPeers,"
                + "InterfaceVlans.ContractBandwidthPool.ContractBandwidth",
                AsTrackable: false);

            return Mapper.Map<Attachment>(dbResult.SingleOrDefault());
        }

        public async Task<List<Attachment>> GetAllByTenantAsync(Tenant tenant)
        {
            var ifaces = await UnitOfWork.InterfaceRepository.GetAsync(q => q.TenantID == tenant.TenantID,
                includeProperties: "Port,Device.Location.SubRegion.Region,Device.Plane,"
                + "Vrf.Device,InterfaceBandwidth,ContractBandwidthPool,BundleInterfacePorts,"
                + "InterfaceVlans.Vrf.BgpPeers,InterfaceVlans.ContractBandwidthPool.ContractBandwidth",
                AsTrackable: false);

            return Mapper.Map<List<Attachment>>(ifaces);
        }

        public async Task<List<Attachment>> GetAsync(Expression<Func<Interface, bool>> filter = null)
        {
            var ifaces = await UnitOfWork.InterfaceRepository.GetAsync(filter,
                includeProperties: "Port,Device.Location.SubRegion.Region,Device.Plane,"
                + "Vrf.Device,InterfaceBandwidth,ContractBandwidthPool,BundleInterfacePorts,"
                + "InterfaceVlans.Vrf.BgpPeers,InterfaceVlans.ContractBandwidthPool.ContractBandwidth",
                AsTrackable: false);

            return Mapper.Map<List<Attachment>>(ifaces);
        }

        public async Task<ServiceResult> AddAsync(AttachmentRequest request)
        {
            var result = new ServiceResult { IsSuccess = true };

            if (request.BundleRequired)
            {
                request.NumPortsRequired = request.Bandwidth.BandwidthGbps / request.Bandwidth.BundleOrMultiPortMemberBandwidthGbps.Value;
                request.PortBandwidthRequired = request.Bandwidth.BandwidthGbps / request.NumPortsRequired;

                var ports = await FindPortsAsync(request, result);
                if (ports.Count() == 0)
                {
                    return result;
                }

                await AddBundleAttachmentAsync(request, ports, result);
            }
            else
            {
                request.NumPortsRequired = 1;
                request.PortBandwidthRequired = request.Bandwidth.BandwidthGbps;

                var ports = await FindPortsAsync(request, result);
                if (ports.Count() == 0)
                {
                    return result;
                }

                await AddAttachmentAsync(request, ports.First(), result);
            }

            return result;
        }

        public async Task<NetworkCheckSyncServiceResult> CheckNetworkSyncAsync(Attachment attachment)
        {

            var iface = await UnitOfWork.InterfaceRepository.GetByIDAsync(attachment.ID);

            if (iface == null)
            {
                var result = new NetworkCheckSyncServiceResult();
                result.NetworkSyncServiceResult.Add("The attachment resource was not found.");

                return result;
            }

            if (attachment.IsBundle)
            {
                if (attachment.IsTagged)
                {
                    var taggedAttachmentBundleServiceModelData = Mapper.Map<TaggedAttachmentBundleInterfaceServiceNetModel>(attachment);
                    var attachmentCheckSyncResult = await NetSync.CheckNetworkSyncAsync(taggedAttachmentBundleServiceModelData,
                        $"/attachment/pe/{attachment.Device.Name}/tagged-attachment-bundle-interface/{taggedAttachmentBundleServiceModelData.BundleID}");

                    await UpdateRequiresSyncAsync(iface, !attachmentCheckSyncResult.InSync, true);

                    return attachmentCheckSyncResult;
                }
                else
                {
                    if (attachment.IsLayer3)
                    {
                        var vrfServiceModelData = Mapper.Map<VrfServiceNetModel>(attachment);
                        var vrfCheckSyncResult = await NetSync.CheckNetworkSyncAsync(vrfServiceModelData,
                            $"/attachment/pe/{attachment.Device.Name }/vrf/{vrfServiceModelData.VrfName}");

                        if (!vrfCheckSyncResult.InSync)
                        {
                            return vrfCheckSyncResult;
                        }
                    }

                    var untaggedAttachmentBundleServiceModelData = Mapper.Map<UntaggedAttachmentBundleInterfaceServiceNetModel>(attachment);
                    var attachmentCheckSyncResult = await NetSync.CheckNetworkSyncAsync(untaggedAttachmentBundleServiceModelData,
                        $"/attachment/pe/{attachment.Device.Name}/untagged-attachment-bundle-interface/{untaggedAttachmentBundleServiceModelData.BundleID}");

                    await UpdateRequiresSyncAsync(iface, !attachmentCheckSyncResult.InSync);

                    return attachmentCheckSyncResult;
                }
            }
            else
            {
                if (attachment.IsTagged)
                {
                    var taggedAttachmentServiceModelData = Mapper.Map<TaggedAttachmentInterfaceServiceNetModel>(attachment);
                    var attachmentCheckSyncResult = await NetSync.CheckNetworkSyncAsync(taggedAttachmentServiceModelData,
                        $"/attachment/pe/{attachment.Device.Name}/tagged-attachment-interface/{taggedAttachmentServiceModelData.InterfaceType},"
                        + taggedAttachmentServiceModelData.InterfaceID.Replace("/", "%2F"));

                    await UpdateRequiresSyncAsync(iface, !attachmentCheckSyncResult.InSync);

                    return attachmentCheckSyncResult;
                }
                else
                {
                    if (attachment.IsLayer3)
                    {
                        var vrfServiceModelData = Mapper.Map<VrfServiceNetModel>(attachment);
                        var vrfCheckSyncResult = await NetSync.CheckNetworkSyncAsync(vrfServiceModelData,
                            $"/attachment/pe/{attachment.Device.Name}/vrf/{vrfServiceModelData.VrfName}");

                        if (!vrfCheckSyncResult.InSync)
                        {
                            return vrfCheckSyncResult;
                        }
                    }

                    var untaggedAttachmentServiceModelData = Mapper.Map<UntaggedAttachmentInterfaceServiceNetModel>(attachment);
                    var attachmentCheckSyncResult = await NetSync.CheckNetworkSyncAsync(untaggedAttachmentServiceModelData,
                        $"/attachment/pe/{attachment.Device.Name}/untagged-attachment-interface/{untaggedAttachmentServiceModelData.InterfaceType},"
                        + untaggedAttachmentServiceModelData.InterfaceID.Replace("/", "%2F"));

                    await UpdateRequiresSyncAsync(iface, !attachmentCheckSyncResult.InSync);

                    return attachmentCheckSyncResult;
                }
            }
        }

        public async Task<NetworkSyncServiceResult> SyncToNetworkAsync(Attachment attachment)
        {

            var iface = await UnitOfWork.InterfaceRepository.GetByIDAsync(attachment.ID);

            if (iface == null)
            {
                var result = new NetworkSyncServiceResult();
                result.Add("The attachment resource was not found.");

                return result;
            }

            if (attachment.IsBundle)
            {
                if (attachment.IsTagged)
                {
                    var taggedAttachmentBundleServiceModelData = Mapper.Map<TaggedAttachmentBundleInterfaceServiceNetModel>(attachment);
                    var attachmentSyncResult = await NetSync.SyncNetworkAsync(taggedAttachmentBundleServiceModelData,
                        $"/attachment/pe/{attachment.Device.Name}/tagged-attachment-bundle-interface/{taggedAttachmentBundleServiceModelData.BundleID}");

                    await UpdateRequiresSyncAsync(iface, !attachmentSyncResult.IsSuccess);

                    return attachmentSyncResult;
                }
                else
                {
                    if (attachment.IsLayer3)
                    {
                        var vrfServiceModelData = Mapper.Map<VrfServiceNetModel>(attachment);
                        var vrfSyncResult = await NetSync.SyncNetworkAsync(vrfServiceModelData,
                            $"/attachment/pe/{attachment.Device.Name}/vrf/{vrfServiceModelData.VrfName}");

                        if (!vrfSyncResult.IsSuccess)
                        {
                            await UpdateRequiresSyncAsync(iface, true);
                            return vrfSyncResult;
                        }
                    }

                    var untaggedAttachmentBundleServiceModelData = Mapper.Map<UntaggedAttachmentBundleInterfaceServiceNetModel>(attachment);
                    var attachmentSyncResult = await NetSync.SyncNetworkAsync(untaggedAttachmentBundleServiceModelData,
                        $"/attachment/pe/{attachment.Device.Name}/untagged-attachment-bundle-interface/{untaggedAttachmentBundleServiceModelData.BundleID}");

                    await UpdateRequiresSyncAsync(iface, !attachmentSyncResult.IsSuccess);

                    return attachmentSyncResult;
                }
            }
            else
            {
                if (attachment.IsTagged)
                {
                    var taggedAttachmentServiceModelData = Mapper.Map<TaggedAttachmentInterfaceServiceNetModel>(attachment);
                    var attachmentSyncResult = await NetSync.SyncNetworkAsync(taggedAttachmentServiceModelData,
                        $"/attachment/pe/{attachment.Device.Name}/tagged-attachment-interface/{taggedAttachmentServiceModelData.InterfaceType},"
                        + taggedAttachmentServiceModelData.InterfaceID.Replace("/", "%2F"));

                    await UpdateRequiresSyncAsync(iface, !attachmentSyncResult.IsSuccess);

                    return attachmentSyncResult;
                }
                else
                {
                    if (attachment.IsLayer3)
                    {
                        var vrfServiceModelData = Mapper.Map<VrfServiceNetModel>(attachment);
                        var vrfSyncResult = await NetSync.SyncNetworkAsync(vrfServiceModelData,
                            $"/attachment/pe/{attachment.Device.Name}/vrf/{vrfServiceModelData.VrfName}");

                        if (!vrfSyncResult.IsSuccess)
                        {
                            await UpdateRequiresSyncAsync(iface, true);
                            return vrfSyncResult;
                        }
                    }

                    var untaggedAttachmentServiceModelData = Mapper.Map<UntaggedAttachmentInterfaceServiceNetModel>(attachment);
                    var attachmentSyncResult = await NetSync.SyncNetworkAsync(untaggedAttachmentServiceModelData,
                        $"/attachment/pe/{attachment.Device.Name}/untagged-attachment-interface/{untaggedAttachmentServiceModelData.InterfaceType},"
                        + untaggedAttachmentServiceModelData.InterfaceID.Replace("/", "%2F"));

                    await UpdateRequiresSyncAsync(iface, !attachmentSyncResult.IsSuccess);

                    return attachmentSyncResult;
                }
            }
        }

        /// <summary>
        /// Delete an attachment from the network and from the inventory
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns></returns>
        public async Task<ServiceResult> DeleteAsync(Attachment attachment)
        {

            var result = new ServiceResult { IsSuccess = true };

            // Validate the attachment can be deleted - if not quit 

            await ValidateDeleteAsync(attachment, result);
            if (!result.IsSuccess)
            {
                return result;
            }

            var syncResult = await DeleteFromNetworkAsync(attachment);

            // Delete from network may return IsSuccess false if the resource was not found - this should be ignored

            if (!syncResult.IsSuccess && syncResult.NetworkHttpResponse.HttpStatusCode != HttpStatusCode.NotFound)
            {
                result.IsSuccess = false;
                result.AddRange(syncResult.GetMessageList());

                return result;
            }

            if (attachment.IsBundle)
            {
                await DeleteBundleAttachmentAsync(attachment, result);
            }
            else
            {
                await DeleteAttachmentAsync(attachment, result);
            }

            return result;
        }

        /// <summary>
        /// Delete an attachment from the network only.
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns></returns>
        public async Task<NetworkSyncServiceResult> DeleteFromNetworkAsync(Attachment attachment)
        {
            var syncResult = new NetworkSyncServiceResult();

            var iface = await UnitOfWork.InterfaceRepository.GetByIDAsync(attachment.ID);

            if (iface == null)
            {
                syncResult.Add("The attachment resource was not found.");

                return syncResult;
            }

            // Validate the attachment can be deleted - if not quit 

            var result = new ServiceResult { IsSuccess = true };
            await ValidateDeleteAsync(attachment, result);
            if (!result.IsSuccess)
            {
                syncResult.AddRange(result.GetMessageList());

                return syncResult;
            }

            if (attachment.IsBundle)
            {
                if (attachment.IsTagged)
                {
                    var taggedAttachmentBundleServiceModelData = Mapper.Map<TaggedAttachmentBundleInterfaceServiceNetModel>(attachment);
                    var attachmentSyncResult = await NetSync.DeleteFromNetworkAsync($"/attachment/pe/{attachment.Device.Name}"
                       + $"/tagged-attachment-bundle-interface/{taggedAttachmentBundleServiceModelData.BundleID}");

                    await UpdateRequiresSyncAsync(iface, true);

                    return attachmentSyncResult;
                }
                else
                {
                    if (attachment.IsLayer3)
                    {
                        var vrfServiceModelData = Mapper.Map<VrfServiceNetModel>(attachment);
                        var vrfSyncResult = await NetSync.DeleteFromNetworkAsync($"/attachment/pe/{attachment.Device.Name}"
                            + $"/vrf/{vrfServiceModelData.VrfName}");

                        if (!vrfSyncResult.IsSuccess)
                        {
                            return vrfSyncResult;
                        }
                    }

                    var untaggedAttachmentBundleServiceModelData = Mapper.Map<UntaggedAttachmentBundleInterfaceServiceNetModel>(attachment);
                    var attachmentSyncResult = await NetSync.DeleteFromNetworkAsync($"/attachment/pe/{attachment.Device.Name}"
                        + $"/untagged-attachment-bundle-interface/{untaggedAttachmentBundleServiceModelData.BundleID}");

                    await UpdateRequiresSyncAsync(iface, true);

                    return attachmentSyncResult;

                }
            }
            else
            {
                if (attachment.IsTagged)
                {
                    var taggedAttachmentServiceModelData = Mapper.Map<TaggedAttachmentInterfaceServiceNetModel>(attachment);
                    var attachmentSyncResult = await NetSync.DeleteFromNetworkAsync($"/attachment/pe/{attachment.Device.Name}"
                        + $"/tagged-attachment-interface/{taggedAttachmentServiceModelData.InterfaceType},"
                        + taggedAttachmentServiceModelData.InterfaceID.Replace("/", "%2F"));

                    await UpdateRequiresSyncAsync(iface, true);

                    return attachmentSyncResult;
                }
                else
                {
                    if (attachment.IsLayer3)
                    {
                        var vrfServiceModelData = Mapper.Map<VrfServiceNetModel>(attachment);
                        var vrfSyncResult = await NetSync.DeleteFromNetworkAsync($"/attachment/pe/{attachment.Device.Name}/vrf/{vrfServiceModelData.VrfName}");

                        if (!vrfSyncResult.IsSuccess)
                        {
                            return vrfSyncResult;
                        }
                    }

                    var untaggedAttachmentServiceModelData = Mapper.Map<UntaggedAttachmentInterfaceServiceNetModel>(attachment);
                    var attachmentSyncResult = await NetSync.DeleteFromNetworkAsync($"/attachment/pe/{attachment.Device.Name}"
                        + $"/untagged-attachment-interface/{untaggedAttachmentServiceModelData.InterfaceType},"
                        + untaggedAttachmentServiceModelData.InterfaceID.Replace("/", "%2F"));

                    await UpdateRequiresSyncAsync(iface, true);

                    return attachmentSyncResult;
                }
            }
        }

        /// <summary>
        /// Validates an attachment request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ServiceResult> ValidateAsync(AttachmentRequest request)
        {
            var result = new ServiceResult { IsSuccess = true };

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

            if (!request.IsTagged)
            {
                var dbResult = await UnitOfWork.ContractBandwidthPoolRepository.GetAsync(q =>
                    q.ContractBandwidthPoolID == request.ContractBandwidthPoolID, includeProperties: "ContractBandwidth,Interfaces.Port,InterfaceVlans.Interface.Port");

                var contractBandwidthPool = dbResult.SingleOrDefault();
                if (contractBandwidthPool == null)
                {
                    result.Add("The requested contract bandwidth pool was not found.");
                    result.IsSuccess = false;

                    return result;
                }

                if (contractBandwidthPool.Interfaces.Count > 0)
                {
                    var iface = contractBandwidthPool.Interfaces.Single();
                    if (iface.IsBundle)
                    {
                        result.Add($"The selected contract bandwidth pool is in-use for attachment bundle {iface.BundleID}. "
                            + "Select another contract bandwidth pool.");
                    }
                    else
                    {
                        result.Add($"The selected contract bandwidth pool is in-use for attachment {iface.Port.Type}{iface.Port.Name}. "
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
                        result.Add($"The selected contract bandwidth pool is in-use for attachment "
                            + $"{ifaceVlan.Interface.Port.Type}{ifaceVlan.Interface.Port.Name}.{ifaceVlan.VlanTag}. "
                            + "Select another contract bandwidth pool.");
                    }

                    result.IsSuccess = false;

                    return result;
                }

                if (contractBandwidthPool.ContractBandwidth.BandwidthMbps > request.Bandwidth.BandwidthGbps * 1000)
                {
                    result.Add($"The requested contract bandwidth of {contractBandwidthPool.ContractBandwidth.BandwidthMbps} Mbps exceeds the "
                        + $"attachment bandwidth of {request.Bandwidth.BandwidthGbps} Gbps.");
                    result.Add("Select a lower contract bandwidth or request a higher bandwidth attachment.");
                    result.IsSuccess = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Helper to validate if an attachment can be deleted.
        /// </summary>
        /// <param name="attachment"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task ValidateDeleteAsync(Attachment attachment, ServiceResult result)
        {
            // Check if attachment has a vrf. If it does, check if the VRF participates in any services

            if (attachment.VrfID != null)
            {
                var vrfValidationResult = await VrfService.ValidateDeleteAsync(attachment.VrfID.Value);
                if (!vrfValidationResult.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.AddRange(vrfValidationResult.GetMessageList());

                    return;
                }
            }

            // If the attachment is tagged, check if the attachment has any vifs, then check if each vif has a vrf.
            // If a vrf exists, check if it participates in an services

            if (attachment.IsTagged)
            {
                var ifaceVlans = await UnitOfWork.InterfaceVlanRepository.GetAsync(q => q.InterfaceID == attachment.ID);
                if (ifaceVlans.Count > 0)
                {
                    foreach (InterfaceVlan ifaceVlan in ifaceVlans)
                    {
                        if (ifaceVlan.VrfID != null)
                        {
                            var vrfValidationResult = await VrfService.ValidateDeleteAsync(ifaceVlan.VrfID.Value);
                            if (!vrfValidationResult.IsSuccess)
                            {
                                result.IsSuccess = false;
                                result.AddRange(vrfValidationResult.GetMessageList());
                            }
                        }   
                    }
                }
            }
        }

        private async Task<IEnumerable<Port>> FindPortsAsync(AttachmentRequest request, ServiceResult result)
        {
            var device = await FindDeviceAsync(request, result);

            if (device == null)
            {
                return Enumerable.Empty<Port>();
            }

            var ports = device.Ports.Where(q => q.TenantID == null && q.PortBandwidth.BandwidthGbps == request.PortBandwidthRequired);

            if (ports.Count() == 0)
            {
                result.Add("No ports matching the requested bandwidth parameter could not be found. "
                    + "Please change your request and try again, or contact your system adminstrator and report this issue.");

                result.IsSuccess = false;
            }

            if (ports.Count() < request.NumPortsRequired)
            {
                result.Add($"The number of ports available ({ports.Count()}) is less than the number required ({request.NumPortsRequired}). "
                    + "Please change your request and try again, or contact your system adminstrator and report this issue.");

                result.IsSuccess = false;
            }

            ports = ports.Take(request.NumPortsRequired);

            return ports;
        }

        private async Task<Device> FindDeviceAsync(AttachmentRequest request, ServiceResult result)
        {
            // Find all devices in the requested location

            var devices = await UnitOfWork.DeviceRepository.GetAsync(q => q.LocationID == request.LocationID,
                includeProperties: "Ports.PortBandwidth,Ports.Device,Interfaces");

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

            Device device = null;

            if (devices.Count == 0)
            {
                result.Add("Ports matching the requested location and "
                    + "bandwidth parameters could not be found. "
                    + "Please change the input parameters and try again, "
                    + "or contact your system adminstrator to report this issue.");

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

        /// <summary>
        /// Add an attachment to the inventory
        /// </summary>
        /// <param name="request"></param>
        /// <param name="port"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task AddAttachmentAsync(AttachmentRequest request, Port port, ServiceResult result)
        {
            port.TenantID = request.TenantID;

            var iface = Mapper.Map<Interface>(request);
            iface.DeviceID = port.DeviceID;
            iface.PortID = port.ID;
            iface.RequiresSync = true;

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
                    var vrfResult = await VrfService.AddAsync(vrf);
                    if (!vrfResult.IsSuccess)
                    {
                        result.AddRange(vrfResult.GetMessageList());
                        result.IsSuccess = false;

                        return;
                    }

                    iface.VrfID = vrf.VrfID;
                }

                UnitOfWork.InterfaceRepository.Insert(iface);
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
        /// Add a bundle attachment to the inventory
        /// </summary>
        /// <param name="request"></param>
        /// <param name="ports"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task AddBundleAttachmentAsync(AttachmentRequest request, IEnumerable<Port> ports, ServiceResult result)
        {
            Vrf vrf = null;
            if (request.IsLayer3)
            {
                vrf = Mapper.Map<Vrf>(request);
                vrf.DeviceID = ports.First().Device.ID;
            }

            var bundleIface = Mapper.Map<Interface>(request);
            var port = ports.First();
            bundleIface.DeviceID = port.Device.ID;
            bundleIface.RequiresSync = true;
            var usedBundleIDs = port.Device.Interfaces.Select(q => q.BundleID).Where(q => q != null);

            // Find the first un-used bundle ID in the range 1, 65535 (this is the Cisco IOS-XR allowable range for bundle IDs).

            bundleIface.BundleID = Enumerable.Range(1, 65535).Except(usedBundleIDs.Select(q => q.Value)).First();

            // Need to implement Transaction Scope here when available in dotnet core

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

                    bundleIface.VrfID = vrf.VrfID;
                }

                UnitOfWork.InterfaceRepository.Insert(bundleIface);
                await this.UnitOfWork.SaveAsync();

                foreach (var p in ports)
                {
                    p.TenantID = request.TenantID;
                    UnitOfWork.PortRepository.Update(p);

                    var bundleIfacePort = new BundleInterfacePort { PortID = p.ID, InterfaceID = bundleIface.InterfaceID };
                    UnitOfWork.BundleInterfacePortRepository.Insert(bundleIfacePort);
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
        }

        /// <summary>
        /// Delete an attachment from inventory
        /// </summary>
        /// <param name="attachment"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task DeleteAttachmentAsync(Attachment attachment, ServiceResult result)
        {
            var port = await UnitOfWork.PortRepository.GetByIDAsync(attachment.Port.ID);
            port.TenantID = null;

            try
            {
                UnitOfWork.PortRepository.Update(port);
                await UnitOfWork.InterfaceRepository.DeleteAsync(attachment.ID);
                if (attachment.VrfID != null)
                {
                    var vrf = await VrfService.GetByIDAsync(attachment.VrfID.Value);
                    await VrfService.DeleteAsync(vrf);
                }

                await UnitOfWork.SaveAsync();
            }

            catch (DbUpdateException  /** ex **/ )
            {
                result.Add("The delete operation failed. The error has been logged. "
                   + "Please try again and contact your system administrator if the problem persists.");

                result.IsSuccess = false;
            }
        }
        /// <summary>
        /// Delete a bundle attachment from inventory
        /// </summary>
        /// <param name="attachment"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task DeleteBundleAttachmentAsync(Attachment attachment, ServiceResult result)
        {
            var bundlePorts = await UnitOfWork.BundleInterfacePortRepository.GetAsync(q => q.InterfaceID == attachment.ID,
                includeProperties: "Port");

            var ports = bundlePorts.Select(q => q.Port);

            try
            {
                foreach (var port in ports)
                {
                    port.TenantID = null;
                    UnitOfWork.PortRepository.Update(port);
                }

                foreach (var bundlePort in bundlePorts)
                {
                    UnitOfWork.BundleInterfacePortRepository.Delete(bundlePort);
                }

                await UnitOfWork.InterfaceRepository.DeleteAsync(attachment.ID);
                if (attachment.VrfID != null)
                {
                    var vrf = await VrfService.GetByIDAsync(attachment.VrfID.Value);
                    await VrfService.DeleteAsync(vrf);
                }

                await UnitOfWork.SaveAsync();
            }

            catch (DbUpdateException  /** ex **/ )
            {
                result.Add("The delete operation failed. The error has been logged. "
                   + "Please try again and contact your system administrator if the problem persists.");

                result.IsSuccess = false;
            }
        }

        /// <summary>
        /// Helper to update the RequiresSync property of an interface record.
        /// </summary>
        /// <param name="iface"></param>
        /// <param name="requiresSync"></param>
        /// <returns></returns>
        public async Task UpdateRequiresSyncAsync(Interface iface, bool requiresSync, bool saveChanges = true)
        {
            iface.RequiresSync = requiresSync;
            UnitOfWork.InterfaceRepository.Update(iface);
            if (saveChanges)
            {
                await UnitOfWork.SaveAsync();
            }

            return;
        }
        /// <summary>
        /// Helper to update the RequiresSync property of an interface record.
        /// </summary>
        /// <param name="iface"></param>
        /// <param name="requiresSync"></param>
        /// <returns></returns>
        public async Task UpdateRequiresSyncAsync(int ifaceID, bool requiresSync, bool saveChanges = true)
        {
            var iface = await UnitOfWork.InterfaceRepository.GetByIDAsync(ifaceID);
            await UpdateRequiresSyncAsync(iface, requiresSync, saveChanges);

            return;
        }
    }
}
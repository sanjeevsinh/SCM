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

        public async Task<AttachmentAndVifs> GetByIDAsync(int id, bool? multiPort = false)
        {
            if (multiPort.GetValueOrDefault())
            {
                var dbResult = await UnitOfWork.MultiPortRepository.GetAsync(q => q.MultiPortID == id,
                              includeProperties: "Device.Location.SubRegion.Region,Device.Plane,"
                              + "Vrf.Device,InterfaceBandwidth,ContractBandwidthPool,Ports.Interface.InterfaceBandwidth,"
                              + "Ports.Interface.InterfaceVlans.Vrf.BgpPeers," 
                              + "Ports.Interface.InterfaceVlans.ContractBandwidthPool.ContractBandwidth",
                              AsTrackable: false);

                return Mapper.Map<AttachmentAndVifs>(dbResult.SingleOrDefault());
            }

            else
            {
                var dbResult = await UnitOfWork.InterfaceRepository.GetAsync(q => q.InterfaceID == id && q.IsMultiPort == false,
                    includeProperties: "Device.Location,Device.Plane,Vrf.BgpPeers,InterfaceBandwidth,ContractBandwidthPool.ContractBandwidth,"
                    + "Tenant,Port.MultiPort,BundleInterfacePorts.Port,InterfaceVlans.Vrf.BgpPeers,"
                    + "InterfaceVlans.ContractBandwidthPool.ContractBandwidth",
                    AsTrackable: false);

                return Mapper.Map<AttachmentAndVifs>(dbResult.SingleOrDefault());
            }
        }

        /// <summary>
        /// Return a VIF from the VRF which the VIF is associated with.
        /// </summary>
        /// <param name="vrfID"></param>
        /// <returns></returns>
        public async Task<AttachmentAndVifs> GetByVrfIDAsync(int vrfID)
        {
            var dbResult = await UnitOfWork.VrfRepository.GetAsync(q => q.VrfID == vrfID, includeProperties: "Interfaces,MultiPorts");
            var vrf = dbResult.Single();

            if (vrf.MultiPorts.Count > 0)
            {
                var multiPorts = vrf.MultiPorts.Single();

                return await GetByIDAsync(multiPorts.MultiPortID, true);
            }
            else if (vrf.Interfaces.Count > 0)
            {
                var iface = vrf.Interfaces.Single();

                return await GetByIDAsync(iface.InterfaceID, false);
            }
            else
            {
                return null;
            }
        }

        public async Task<List<AttachmentAndVifs>> GetAllByTenantAsync(Tenant tenant)
        {
            var ifaces = await UnitOfWork.InterfaceRepository.GetAsync(q => q.TenantID == tenant.TenantID && !q.IsMultiPort,
                includeProperties: "Port,Device.Location.SubRegion.Region,Device.Plane,"
                + "Vrf.Device,InterfaceBandwidth,ContractBandwidthPool,BundleInterfacePorts,"
                + "InterfaceVlans.Vrf.BgpPeers,InterfaceVlans.ContractBandwidthPool.ContractBandwidth",
                AsTrackable: false);

            var multiPorts = await UnitOfWork.MultiPortRepository.GetAsync(q => q.TenantID == tenant.TenantID,
                includeProperties: "Device.Location.SubRegion.Region,Device.Plane,"
                + "Vrf.Device,InterfaceBandwidth,ContractBandwidthPool,Ports.Interface.InterfaceBandwidth,"
                + "Ports.Interface.InterfaceVlans.Vrf.BgpPeers,"
                + "Ports.Interface.InterfaceVlans.ContractBandwidthPool.ContractBandwidth",
                AsTrackable: false);

            return Mapper.Map<List<AttachmentAndVifs>>(ifaces).ToList().Concat(Mapper.Map<List<AttachmentAndVifs>>(multiPorts)).ToList();
        }

        public async Task<List<AttachmentAndVifs>> GetAsync(Expression<Func<Interface, bool>> filter = null, bool? multiPort = false)
        {
            if (multiPort.Value)
            {
                return null;
            }
            else
            {
                var ifaces = await UnitOfWork.InterfaceRepository.GetAsync(filter,
                    includeProperties: "Port.MultiPort,Device.Location.SubRegion.Region,Device.Plane,"
                    + "Vrf.Device,InterfaceBandwidth,ContractBandwidthPool,BundleInterfacePorts,"
                    + "InterfaceVlans.Vrf.BgpPeers,InterfaceVlans.ContractBandwidthPool.ContractBandwidth",
                    AsTrackable: false);

                return Mapper.Map<List<AttachmentAndVifs>>(ifaces);
            }
        }

        public async Task<ServiceResult> AddAsync(AttachmentRequest request)
        {
            var result = new ServiceResult { IsSuccess = true };
            IEnumerable<Port> ports = Enumerable.Empty<Port>();

            if (request.BundleRequired || request.MultiPortRequired)
            {
                request.NumPortsRequired = request.Bandwidth.BandwidthGbps / request.Bandwidth.BundleOrMultiPortMemberBandwidthGbps.Value;
                request.PortBandwidthRequired = request.Bandwidth.BandwidthGbps / request.NumPortsRequired;

                ports = await FindPortsAsync(request, result);
                if (ports.Count() == 0)
                {
                    return result;
                }
            }
            if (request.BundleRequired)
            {
                await AddBundleAttachmentAsync(request, ports.ToList(), result);
            }
            else if (request.MultiPortRequired)
            {
                await AddMultiPortAsync(request, ports.ToList(), result);
            }
            else
            {
                request.NumPortsRequired = 1;
                request.PortBandwidthRequired = request.Bandwidth.BandwidthGbps;

                ports = await FindPortsAsync(request, result);
                if (ports.Count() == 0)
                {
                    return result;
                }

                await AddAttachmentAsync(request, ports.First(), result);
            }

            return result;
        }

        public async Task<NetworkCheckSyncServiceResult> CheckNetworkSyncAsync(AttachmentAndVifs attachment)
        {
            var result = new NetworkCheckSyncServiceResult { InSync = true };

            if (attachment.IsTagged)
            {
                if (attachment.IsBundle)
                {
                    var data = Mapper.Map<TaggedAttachmentBundleInterfaceServiceNetModel>(attachment);
                    result = await NetSync.CheckNetworkSyncAsync(data,
                        $"/attachment/pe/{attachment.Device.Name}/tagged-attachment-bundle-interface/{attachment.BundleID}");
                }

                else if (attachment.IsMultiPort)
                {
                    var data = Mapper.Map<TaggedAttachmentMultiPortServiceNetModel>(attachment);
                    result = await NetSync.CheckNetworkSyncAsync(data,
                       $"/attachment/pe/{attachment.Device.Name}/tagged-attachment-multiport/{attachment.Name}");
                }
                else

                {
                    var data = Mapper.Map<TaggedAttachmentInterfaceServiceNetModel>(attachment);
                    result = await NetSync.CheckNetworkSyncAsync(data,
                        $"/attachment/pe/{attachment.Device.Name}/tagged-attachment-interface/{attachment.InterfaceType},"
                        + attachment.InterfaceName.Replace("/", "%2F"));
                }
            }
            else
            {
                if (attachment.IsBundle)
                {
                    var data = Mapper.Map<UntaggedAttachmentBundleInterfaceServiceNetModel>(attachment);
                    result = await NetSync.CheckNetworkSyncAsync(data,
                            $"/attachment/pe/{attachment.Device.Name}/untagged-attachment-bundle-interface/{attachment.BundleID}");
                }
                else if (attachment.IsMultiPort)
                {
                    var data = Mapper.Map<UntaggedAttachmentMultiPortServiceNetModel>(attachment);
                    result = await NetSync.CheckNetworkSyncAsync(data,
                        $"/attachment/pe/{attachment.Device.Name}/untagged-attachment-multiport/{attachment.Name}");
                }
                else
                {
                    var data = Mapper.Map<UntaggedAttachmentInterfaceServiceNetModel>(attachment);
                    result = await NetSync.CheckNetworkSyncAsync(data,
                        $"/attachment/pe/{attachment.Device.Name}/untagged-attachment-interface/{attachment.InterfaceType},"
                        + attachment.InterfaceName.Replace("/", "%2F"));
                }
            }

            await UpdateRequiresSyncAsync(attachment.ID, !result.InSync, true, attachment.IsMultiPort);

            return result;
        }

        public async Task<NetworkSyncServiceResult> SyncToNetworkAsync(AttachmentAndVifs attachment)
        {
            var result = new NetworkSyncServiceResult { IsSuccess = true };
            var serviceModelData = Mapper.Map<AttachmentServiceNetModel>(attachment);

            result = await NetSync.SyncNetworkAsync(serviceModelData, $"/attachment/pe/{attachment.Device.Name}", new HttpMethod("PATCH"));

            await UpdateRequiresSyncAsync(attachment.ID, !result.IsSuccess,true, attachment.IsMultiPort);

            return result;
        }

        /// <summary>
        /// Delete an attachment from the network and from the inventory
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns></returns>
        public async Task<ServiceResult> DeleteAsync(AttachmentAndVifs attachment)
        {
            // Validate the attachment can be deleted - if not quit 

            var validationResult = await ValidateDeleteAsync(attachment);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            var syncResult = await DeleteFromNetworkAsync(attachment);

            // Delete from network may return IsSuccess false if the resource was not found - this should be ignored

            if (!syncResult.IsSuccess)
            {
                if (syncResult.NetworkHttpResponse == null || syncResult.NetworkHttpResponse.HttpStatusCode != HttpStatusCode.NotFound)
                {
                    var result = new ServiceResult();
                    result.AddRange(syncResult.GetMessageList());

                    return result;
                }
            }

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

        public async Task<NetworkSyncServiceResult> DeleteFromNetworkAsync(AttachmentAndVifs attachment)
        {
            var result = new NetworkSyncServiceResult { IsSuccess = true };

            // Validate the attachment can be deleted - if not quit 

            var validationResult = await ValidateDeleteAsync(attachment);
            if (!validationResult.IsSuccess)
            {
                result.AddRange(validationResult.GetMessageList());
                result.IsSuccess = false;

                return result;
            }

            var tasks = new List<Task<NetworkSyncServiceResult>>();
            var serviceModelData = Mapper.Map<AttachmentServiceNetModel>(attachment);

            if (serviceModelData.TaggedAttachmentBundleInterfaces.Count > 0)
            {
                tasks.Add(NetSync.DeleteFromNetworkAsync($"/attachment/pe/{attachment.Device.Name}"
                    + $"/tagged-attachment-bundle-interface/{attachment.BundleID}"));
            }
            else if (serviceModelData.TaggedAttachmentInterfaces.Count > 0)
            {
                tasks.Add(NetSync.DeleteFromNetworkAsync($"/attachment/pe/{attachment.Device.Name}"
                    + $"/tagged-attachment-interface/{attachment.InterfaceType},"
                    + attachment.InterfaceName.Replace("/", "%2F")));
            }
            else if (serviceModelData.TaggedAttachmentMultiPorts.Count > 0)
            {
                tasks.Add(NetSync.DeleteFromNetworkAsync($"/attachment/pe/{attachment.Device.Name}"
                   + $"/tagged-attachment-multiport/{attachment.Name}"));
            }
            else if (serviceModelData.UntaggedAttachmentBundleInterfaces.Count > 0)
            {
                tasks.Add(NetSync.DeleteFromNetworkAsync($"/attachment/pe/{attachment.Device.Name}"
                   + $"/untagged-attachment-bundle-interface/{attachment.BundleID}"));
            }
            else if (serviceModelData.UntaggedAttachmentInterfaces.Count > 0)
            {
                tasks.Add(NetSync.DeleteFromNetworkAsync($"/attachment/pe/{attachment.Device.Name}"
                   + $"/untagged-attachment-interface/{attachment.InterfaceType},"
                   + attachment.InterfaceName.Replace("/", "%2F")));
            }
            else if (serviceModelData.UntaggedAttachmentMultiPorts.Count > 0)
            {
                tasks.Add(NetSync.DeleteFromNetworkAsync($"/attachment/pe/{attachment.Device.Name}"
                    + $"/untagged-attachment-multiport/{attachment.Name}"));
            }

            var vrfTasks = new List<Task<NetworkSyncServiceResult>>();
            if (serviceModelData.Vrfs.Count > 0)
            {
                foreach (var vrf in serviceModelData.Vrfs)
                {
                    vrfTasks.Add(NetSync.DeleteFromNetworkAsync($"/attachment/pe/{attachment.Device.Name}"
                        + $"/vrf/{vrf.VrfName}"));
                }
            }

            // Remove attachments first, then when complete remove vrfs.
            // This is important because the server will throw an error if an attempt is made
            // to delete a vrf which is referenced by an attachment.

            await Task.WhenAll(tasks);
            await Task.WhenAll(vrfTasks);

            // Check results

            foreach (var t in tasks.Concat(vrfTasks))
            {
                var r = t.Result;
                if (!r.IsSuccess)
                {
                    result.AddRange(r.GetMessageList());
                    result.IsSuccess = false;
                }
            }

            await UpdateRequiresSyncAsync(attachment.ID, true, true, attachment.IsMultiPort);

            return result;
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

                var validateContractBandwidthPoolResult = await ContractBandwidthPoolService.ValidateAsync(request.ContractBandwidthPoolID.Value);
                if (!validateContractBandwidthPoolResult.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.AddRange(validateContractBandwidthPoolResult.GetMessageList());

                    return result;
                }

                var contractBandwidthPoolResult = await UnitOfWork.ContractBandwidthPoolRepository.GetAsync(q => q.ContractBandwidthPoolID == request.ContractBandwidthPoolID,
                     includeProperties: "ContractBandwidth");
                var contractBandwidthPool = contractBandwidthPoolResult.Single();

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
        /// Update the RequiresSync property of an interface record.
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
        public async Task UpdateRequiresSyncAsync(int id, bool requiresSync, bool saveChanges = true, bool? isMultiPort = false)
        {
            if (isMultiPort.GetValueOrDefault())
            {
                var multiPort = await UnitOfWork.MultiPortRepository.GetByIDAsync(id);
                await UpdateRequiresSyncAsync(multiPort, requiresSync, saveChanges);
            }
            else
            {
                var iface = await UnitOfWork.InterfaceRepository.GetByIDAsync(id);
                await UpdateRequiresSyncAsync(iface, requiresSync, saveChanges);
            }

            return;
        }

        /// <summary>
        /// Helper to update the RequiresSync property of a multiport record.
        /// </summary>
        /// <param name="multiPort"></param>
        /// <param name="requiresSync"></param>
        /// <returns></returns>
        public async Task UpdateRequiresSyncAsync(MultiPort multiPort, bool requiresSync, bool saveChanges = true)
        {
            multiPort.RequiresSync = requiresSync;
            UnitOfWork.MultiPortRepository.Update(multiPort);
            if (saveChanges)
            {
                await UnitOfWork.SaveAsync();
            }

            return;
        }

        /// <summary>
        /// Helper to validate if an attachment can be deleted.
        /// </summary>
        /// <param name="attachment"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task<ServiceResult> ValidateDeleteAsync(AttachmentAndVifs attachment)
        {
            var result = new ServiceResult { IsSuccess = true };

            // Check if attachment has a vrf. If it does, check if the VRF participates in any services

            if (attachment.VrfID != null)
            {
                var vrfValidationResult = await VrfService.ValidateDeleteAsync(attachment.VrfID.Value);
                if (!vrfValidationResult.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.AddRange(vrfValidationResult.GetMessageList());

                    return result;
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

            return result;
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
                includeProperties: "Ports.PortBandwidth,Ports.Device,Interfaces,Ports.Device.MultiPorts");

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
            iface.InterfaceBandwidthID = request.BandwidthID;
            iface.RequiresSync = true;

            Vrf vrf = null;
            if (request.IsLayer3)
            {
                vrf = Mapper.Map<Vrf>(request);
                vrf.DeviceID = port.Device.ID;
                iface.IpAddress = request.IpAddress1;
                iface.SubnetMask = request.SubnetMask1;
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
        private async Task AddBundleAttachmentAsync(AttachmentRequest request, IList<Port> ports, ServiceResult result)
        {
            Vrf vrf = null;
            var bundleIface = Mapper.Map<Interface>(request);

            if (request.IsLayer3)
            {
                vrf = Mapper.Map<Vrf>(request);
                vrf.DeviceID = ports.First().Device.ID;
                bundleIface.IpAddress = request.IpAddress1;
                bundleIface.SubnetMask = request.SubnetMask1;
            }

            var port = ports.First();
            bundleIface.DeviceID = port.Device.ID;
            bundleIface.InterfaceBandwidthID = request.BandwidthID;
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
        /// Add a multiport attachment to the inventory
        /// </summary>
        /// <param name="request"></param>
        /// <param name="port"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task AddMultiPortAsync(AttachmentRequest request, IList<Port> ports, ServiceResult result)
        {
            Vrf vrf = null;
            var port = ports.First();
            var deviceID = port.DeviceID;

            // Check to create a vrf

            if (request.IsLayer3)
            {
                vrf = Mapper.Map<Vrf>(request);
                vrf.DeviceID = deviceID;
            }

            var usedIds = port.Device.MultiPorts.Select(q => q.Identifier);

            // Find the first un-used identifier in the range 1, 65535 (this range is fairly arbitrary - the identifer is not used
            // for network configuration of a multiport. It serves only to create an ID which is unique to a device and which is used
            // in the NSO service models).

            var id = Enumerable.Range(1, 65535).Except(usedIds).First();

            // Create a new multiport

            var multiPort = Mapper.Map<MultiPort>(request);
            multiPort.Identifier = id;
            multiPort.DeviceID = deviceID;
            multiPort.InterfaceBandwidthID = request.BandwidthID;
            multiPort.LocalFailureDetectionIpAddress = "1.1.1.1";
            multiPort.RemoteFailureDetectionIpAddress = "1.1.1.2";
            multiPort.RequiresSync = true;

            try
            {
                // Need to implement Transaction Scope here when available in dotnet core

                if (vrf != null)
                {
                    var vrfResult = await VrfService.AddAsync(vrf);
                    if (vrfResult.IsSuccess)
                    {
                        multiPort.VrfID = vrf.VrfID;
                    }
                    else
                    { 
                        result.AddRange(vrfResult.GetMessageList());
                        result.IsSuccess = false;

                        return;
                    }
                }

                // Generate key for the multiport record

                UnitOfWork.MultiPortRepository.Insert(multiPort);
                await UnitOfWork.SaveAsync();

                // Create interface records, one for each member interface 
                // of the multiport

                var interfaceBandwidthsQuery = await UnitOfWork.InterfaceBandwidthRepository.GetAsync(q => 
                    q.BandwidthGbps == request.PortBandwidthRequired);

                var interfaceBandwidth = interfaceBandwidthsQuery.Single();

                var portCount = ports.Count();

                for (var i = 1; i <= portCount; i++)
                {
                    var iface = Mapper.Map<Interface>(request);
                    var p = ports[i - 1];
                    iface.DeviceID = deviceID;
                    iface.PortID = p.ID;
                    iface.InterfaceBandwidthID = interfaceBandwidth.InterfaceBandwidthID;
                    iface.RequiresSync = true;

                    if (request.IsLayer3)
                    {
                        iface.VrfID = vrf.VrfID;

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

                    p.TenantID = request.TenantID;
                    p.MultiPortID = multiPort.MultiPortID;
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
        }

        /// <summary>
        /// Delete an attachment from inventory
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns></returns>
        private async Task<ServiceResult> DeleteAttachmentAsync(AttachmentAndVifs attachment)
        {
            var result = new ServiceResult { IsSuccess = true };

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

            return result;
        }

        /// <summary>
        /// Delete a bundle attachment from inventory
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns></returns>
        private async Task<ServiceResult> DeleteBundleAttachmentAsync(AttachmentAndVifs attachment)
        {
            var result = new ServiceResult { IsSuccess = true };

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

            return result;
        }

        /// <summary>
        /// Delete a multiport attachment from inventory
        /// </summary>
        /// <param name="multiport"></param>
        /// <returns></returns>
        private async Task<ServiceResult> DeleteMultiPortAttachmentAsync(AttachmentAndVifs attachment)
        {
            var result = new ServiceResult { IsSuccess = true };

            var ports = await UnitOfWork.PortRepository.GetAsync(q => q.MultiPortID == attachment.ID,
                includeProperties: "Interface");

            var interfaces = ports.Select(q => q.Interface);

            try
            {
                foreach (var port in ports)
                {
                    port.TenantID = null;
                    port.MultiPortID = null;
                    UnitOfWork.PortRepository.Update(port);
                }

                foreach (var iface in interfaces)
                {
                    UnitOfWork.InterfaceRepository.Delete(iface);
                }

                if (attachment.VrfID != null)
                {
                    var vrf = await VrfService.GetByIDAsync(attachment.VrfID.Value);
                    await VrfService.DeleteAsync(vrf);
                }

                await this.UnitOfWork.MultiPortRepository.DeleteAsync(attachment.ID);
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
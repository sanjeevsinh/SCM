using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using SCM.Models.NetModels.IpVpnNetModels;
using System.Threading.Tasks;
using AutoMapper;
using System.Net;
using SCM.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace SCM.Services.SCMServices
{
    public class VpnService : BaseService, IVpnService
    {
        public VpnService(IUnitOfWork unitOfWork, 
            IMapper mapper,
            IRouteTargetService routeTargetService,
            INetworkSyncService netSync) : base(unitOfWork, mapper, netSync)
        {
            RouteTargetService = routeTargetService;
        }

        private IRouteTargetService RouteTargetService { get; set; }

        public async Task<IEnumerable<Vpn>> GetAllAsync()
        {
            return await this.UnitOfWork.VpnRepository.GetAsync(includeProperties:
                "Region,"
                + "Plane,"
                + "VpnTenancyType,"
                + "VpnTopologyType.VpnProtocolType,"
                + "Tenant,"
                + "VpnAttachmentSets.VpnTenantNetworks.TenantNetwork,"
                + "VpnAttachmentSets.VpnTenantCommunities.TenantCommunity,"
                + "VpnAttachmentSets.AttachmentSet.AttachmentSetVrfs.Vrf.Device.Location.SubRegion.Region,"
                + "RouteTargets",AsTrackable:false);
        }

        public async Task<Vpn> GetByIDAsync(int id)
        {
            var dbResult = await this.UnitOfWork.VpnRepository.GetAsync(q => q.VpnID == id, 
                includeProperties:
                "Region,"
                + "Plane,"
                + "VpnTenancyType,"
                + "VpnTopologyType.VpnProtocolType,"
                + "Tenant,"
                + "VpnAttachmentSets.VpnTenantNetworks.TenantNetwork,"
                + "VpnAttachmentSets.VpnTenantCommunities.TenantCommunity,"
                + "VpnAttachmentSets.AttachmentSet.AttachmentSetVrfs.Vrf.Device.Location.SubRegion.Region,"
                + "RouteTargets", 
                AsTrackable: false);

            return dbResult.SingleOrDefault();
        }


        public async Task<IEnumerable<Vpn>> GetAllByVrfIDAsync(int id)
        {
            var dbResult = await this.UnitOfWork.VpnRepository.GetAsync(q => q.VpnAttachmentSets
                    .SelectMany(r => r.AttachmentSet.AttachmentSetVrfs)
                    .Where(s => s.VrfID == id)
                    .Count() > 0,
                includeProperties: 
                "Region,"
                + "Plane,"
                + "VpnTenancyType,"
                + "VpnTopologyType.VpnProtocolType,"
                + "Tenant,"
                + "VpnAttachmentSets.VpnTenantNetworks.TenantNetwork,"
                + "VpnAttachmentSets.VpnTenantCommunities.TenantCommunity,"
                + "VpnAttachmentSets.AttachmentSet.AttachmentSetVrfs.Vrf.Device.Location.SubRegion.Region,"
                + "RouteTargets", 
                AsTrackable: false);

            return dbResult.GroupBy(q => q.VpnID).Select(r => r.First());
        }

        public async Task<IEnumerable<Vpn>> GetAllByAttachmentSetIDAsync(int id)
        {
            var dbResult = await this.UnitOfWork.VpnRepository.GetAsync(q => q.VpnAttachmentSets
                    .Where(r => r.AttachmentSetID == id)
                    .Count() > 0,
                includeProperties: "Region,"
                + "Plane,"
                + "VpnTenancyType,"
                + "VpnTopologyType.VpnProtocolType,"
                + "Tenant,"
                + "VpnAttachmentSets.VpnTenantNetworks.TenantNetwork,"
                + "VpnAttachmentSets.VpnTenantCommunities.TenantCommunity,"
                + "VpnAttachmentSets.AttachmentSet.AttachmentSetVrfs.Vrf.Device.Location.SubRegion.Region,"
                + "RouteTargets", AsTrackable: false);

            return dbResult.GroupBy(q => q.VpnID).Select(r => r.First());
        }

        public async Task<IEnumerable<Vpn>> GetAllByTenantNetworkIDAsync(int id)
        {
            var dbResult = await this.UnitOfWork.VpnRepository.GetAsync(q => q.VpnAttachmentSets
                    .SelectMany(r => r.VpnTenantNetworks)
                    .Where(s => s.TenantNetworkID == id)
                    .Count() > 0,
                includeProperties: "Region,"
                + "Plane,"
                + "VpnTenancyType,"
                + "VpnTopologyType.VpnProtocolType,"
                + "Tenant,"
                + "VpnAttachmentSets.VpnTenantNetworks.TenantNetwork,"
                + "VpnAttachmentSets.VpnTenantCommunities.TenantCommunity,"
                + "VpnAttachmentSets.AttachmentSet.AttachmentSetVrfs.Vrf.Device.Location.SubRegion.Region,"
                + "RouteTargets", AsTrackable: false);

            return dbResult.GroupBy(q => q.VpnID).Select(r => r.First());
        }

        public async Task<IEnumerable<Vpn>> GetAllByTenantCommunityIDAsync(int id)
        {
            var dbResult = await this.UnitOfWork.VpnRepository.GetAsync(q => q.VpnAttachmentSets
                    .SelectMany(r => r.VpnTenantCommunities)
                    .Where(s => s.TenantCommunityID == id)
                    .Count() > 0,
                includeProperties: "Region,"
                + "Plane,"
                + "VpnTenancyType,"
                + "VpnTopologyType.VpnProtocolType,"
                + "Tenant,"
                + "VpnAttachmentSets.VpnTenantNetworks.TenantNetwork,"
                + "VpnAttachmentSets.VpnTenantCommunities.TenantCommunity,"
                + "VpnAttachmentSets.AttachmentSet.AttachmentSetVrfs.Vrf.Device.Location.SubRegion.Region,"
                + "RouteTargets", AsTrackable: false);

            return dbResult.GroupBy(q => q.VpnID).Select(r => r.First());
        }

        public async Task<ServiceResult> AddAsync(Vpn vpn)
        {
            var result = new ServiceResult { IsSuccess = true };
            var vpnTopologyType = await UnitOfWork.VpnTopologyTypeRepository.GetByIDAsync(vpn.VpnTopologyTypeID);

            // Get some route targets

            var routeTargets = await RouteTargetService.AllocateAllVpnRouteTargetsAsync(vpnTopologyType.TopologyType, result);

            // Quit if failed to allocate route targets

            if (!result.IsSuccess)
            {
                return result;
            }

            try
            {
                // Implement transactionScope here when available in dotnet core

                vpn.RequiresSync = true;
                vpn.Created = true;

                this.UnitOfWork.VpnRepository.Insert(vpn);

                // Save in order to generat a new vpn ID

                await this.UnitOfWork.SaveAsync();

                foreach (RouteTarget rt in routeTargets)
                {
                    rt.VpnID = vpn.VpnID;
                    this.UnitOfWork.RouteTargetRepository.Insert(rt);
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

            return result;
        }

        public async Task<int> UpdateAsync(Vpn vpn)
        {
            this.UnitOfWork.VpnRepository.Update(vpn);

            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<ServiceResult> DeleteAsync(Vpn vpn)
        {
            var serviceResult = new ServiceResult { IsSuccess = true };

            this.UnitOfWork.VpnRepository.Delete(vpn);
            await this.UnitOfWork.SaveAsync();

            return serviceResult;
        }

        /// <summary>
        /// Validate a new VPN request
        /// </summary>
        /// <param name="vpn"></param>
        /// <returns></returns>
        public async Task<ServiceResult> ValidateNewAsync(Vpn vpn)
        {
            var validationResult = new ServiceResult { IsSuccess = true };

            var topologyType = await UnitOfWork.VpnTopologyTypeRepository.GetByIDAsync(vpn.VpnTopologyTypeID);

            if (vpn.IsExtranet)
            {
                if (topologyType.TopologyType != "Hub-and-Spoke")
                {
                    validationResult.Add("Extranet is supported only for Hub-and-Spoke IP VPN topologies.");
                    validationResult.IsSuccess = false;
                }
            }

            return validationResult;
        }

        /// <summary>
        /// Validate change requests to a VPN
        /// </summary>
        /// <param name="vpn"></param>
        /// <param name="currentVpn"></param>
        /// <returns></returns>
        public async Task<ServiceResult> ValidateChangesAsync(Vpn vpn, Vpn currentVpn)
        {
            var validationResult = new ServiceResult { IsSuccess = true };

            var vpnTenancyType = await UnitOfWork.VpnTenancyTypeRepository.GetByIDAsync(vpn.VpnTenancyTypeID);
            if (vpnTenancyType.TenancyType == "Single" && currentVpn.VpnTenancyType.TenancyType == "Multi")
            {
                // The tenancy type can be narrowed only if the only tenant of the VPN is the owner.

                var tenants = currentVpn.VpnAttachmentSets.Select(s => s.AttachmentSet.Tenant);
                var ownerCount = tenants.Count(s => s.Name == currentVpn.Tenant.Name);

                if (ownerCount != tenants.Count())
                {
                    validationResult.Add("The Tenancy Type cannot be changed from Multi to Single because tenants other "
                        + "than the owner are attached to the VPN.");
                    validationResult.IsSuccess = false;
                }
            }

            // Check if Region has been narrowed to a specific region, or changed from one region to another

            if (vpn.RegionID != null && (currentVpn.RegionID == null || vpn.RegionID != currentVpn.RegionID))
            {
                // Region can be narrowed or changed only if the only devices with VRFs which participate in the VPN are in the 
                // selected region.

                var regions = currentVpn.VpnAttachmentSets.SelectMany(q =>
                q.AttachmentSet.AttachmentSetVrfs.Select(r => r.Vrf.Device.Location.SubRegion.Region));

                var distinctRegions = regions.GroupBy(q => q.RegionID).Select(group => group.First());
                if (distinctRegions.Count() == 1)
                {
                    var region = distinctRegions.Single();
                    if (region.RegionID != vpn.RegionID)
                    {
                        validationResult.Add("The Region setting cannot be narrowed to a specific region because all of the tenants "
                            + "of the VPN exist in the " + region.Name + " region.");
                        validationResult.IsSuccess = false;
                    }
                }
                else if (distinctRegions.Count() > 1)
                {
                    validationResult.Add("The Region setting cannot be narrowed to a specific region because tenants of the VPN "
                        + "exist in more than one region.");
                    validationResult.IsSuccess = false;
                }
            }

            if (vpn.IsExtranet)
            {
                var VpnTopologyType = await UnitOfWork.VpnTopologyTypeRepository.GetByIDAsync(vpn.VpnTopologyTypeID);
                if (VpnTopologyType.TopologyType != "Hub-and-Spoke")
                {
                    validationResult.Add("The Extranet attribute can only be set for VPNs with the 'Hub-and-Spoke' topology.");
                    validationResult.IsSuccess = false;
                }
            }

            return validationResult;
        }

        /// <summary>
        /// Perform shallow check of network sync state of a collection
        /// of vpns by checking the 'RequiresSync' property.
        /// </summary>
        /// <param name="vpns"></param>
        /// <returns></returns>
        public ServiceResult ShallowCheckNetworkSync(IEnumerable<Vpn> vpns)
        {
            var result = new ServiceResult { IsSuccess = true };

            var vpnsRequireSync = vpns.Where(q => q.RequiresSync);
            if (vpnsRequireSync.Count() > 0)
            {
                result.IsSuccess = false;
                result.Add("The following VPNs require synchronisation with the network:.");
                vpnsRequireSync.ToList().ForEach(f => result.Add($"'{f.Name}'."));
            }

            return result;
        }

        public async Task<IEnumerable<ServiceResult>> CheckNetworkSyncAsync(IEnumerable<Vpn> vpns,
           IProgress<ServiceResult> progress)
        {
            List<Task<ServiceResult>> tasks = (from vpn in vpns select CheckNetworkSyncAsync(vpn)).ToList();
            return await ExecuteTasksAsync(tasks, progress);
        }

        public async Task<IEnumerable<ServiceResult>> CheckNetworkSyncAsync(IEnumerable<Vpn> vpns, 
            AttachmentSet attachmentSetContext, 
            IProgress<ServiceResult> progress)
        {
            List<Task<ServiceResult>> tasks = (from vpn in vpns select CheckNetworkSyncAsync(vpn, attachmentSetContext)).ToList();
            return await ExecuteTasksAsync(tasks, progress);
        }

        public async Task<ServiceResult> CheckNetworkSyncAsync(Vpn vpn)
        {
            var result = new ServiceResult {
                IsSuccess = true,
                Item = vpn
            };

            var vpnServiceModelData = Mapper.Map<IpVpnServiceNetModel>(vpn);
            var syncResult = await NetSync.CheckNetworkSyncAsync(vpnServiceModelData, "/ip-vpn/vpn/" + vpn.Name);

            result.NetworkSyncServiceResults.Add(syncResult);

            if (!syncResult.IsSuccess)
            {
                result.IsSuccess = false;
                vpn.RequiresSync = true;

                if (syncResult.StatusCode == NetworkSyncStatusCode.Success)
                {
                    // Request was successfully executed and the VPN was tested for sync with the network

                    result.Add($"VPN '{vpn.Name}' is not synchronised with the network.");
                }
                else
                {
                    // Request failed to execute for some reason - e.g server down, no network etc

                    result.Add($"There was an error checking status for VPN '{vpn.Name}'.");
                }
            }
            else
            {
                vpn.RequiresSync = false;
            }

            return result;
        }

        public async Task<ServiceResult> CheckNetworkSyncAsync(Vpn vpn, AttachmentSet attachmentSetContext)
        {
            var result = await CheckNetworkSyncAsync(vpn);
            result.Context = attachmentSetContext;

            return result;
        }

        public async Task<IEnumerable<ServiceResult>> SyncToNetworkAsync(IEnumerable<Vpn> vpns, 
            AttachmentSet attachmentSetContext, 
            IProgress<ServiceResult> progress)
        {
            List<Task<ServiceResult>> tasks = (from vpn in vpns select SyncToNetworkAsync(vpn, attachmentSetContext)).ToList();
            return await ExecuteTasksAsync(tasks, progress);
        }

        public async Task<ServiceResult> SyncToNetworkAsync(Vpn vpn)
        {
            var result = new ServiceResult
            {
                IsSuccess = true,
                Item = vpn
            };

            var vpnServiceModelData = Mapper.Map<IpVpnServiceNetModel>(vpn);
            var syncResult = await NetSync.SyncNetworkAsync(vpnServiceModelData, "/ip-vpn/vpn/" + vpn.Name);

            result.NetworkSyncServiceResults.Add(syncResult);

            if (!syncResult.IsSuccess)
            {
                result.IsSuccess = false;
                vpn.RequiresSync = true;

                if (syncResult.StatusCode == NetworkSyncStatusCode.Success)
                {
                    // Request was successfully executed but synchronisation failed

                    result.Add($"Failed to synchronise VPN '{vpn.Name}' with the network.");
                }
                else
                {
                    // Request failed to execute for some reason - e.g server down, no network etc

                    result.Add($"There was an error synchronising VPN '{vpn.Name}' with the network.");
                }
            }
            else
            {
                vpn.RequiresSync = false;
            }

            return result;
        }
    
        public async Task<ServiceResult> SyncToNetworkAsync(Vpn vpn, AttachmentSet attachmentSetContext)
        {
            var result = await SyncToNetworkAsync(vpn);
            result.Context = attachmentSetContext;

            return result;
        }

        public async Task<ServiceResult> DeleteFromNetworkAsync(Vpn vpn)
        {
            var result = new ServiceResult();

            var syncResult = await NetSync.DeleteFromNetworkAsync("/ip-vpn/vpn/" + vpn.Name);
            result.NetworkSyncServiceResults.Add(syncResult);
            result.IsSuccess = syncResult.IsSuccess;

            return result;
        }

        /// <summary>
        /// Update the RequiresSync property of a vpn record.
        /// </summary>
        /// <param name="vpn"></param>
        /// <param name="requiresSync"></param>
        /// <returns></returns>
        public async Task UpdateRequiresSyncAsync(Vpn vpn, bool requiresSync, bool saveChanges = true)
        {
            vpn.RequiresSync = requiresSync;
            UnitOfWork.VpnRepository.Update(vpn);
            if (saveChanges)
            {
                await UnitOfWork.SaveAsync();
            }

            return;
        }

        /// <summary>
        /// Update the RequiresSync property of a vpn record.
        /// </summary>
        /// <param name="vpn"></param>
        /// <param name="requiresSync"></param>
        /// <returns></returns>
        public async Task UpdateRequiresSyncAsync(int vpnID, bool requiresSync, bool saveChanges = true)
        {
            var vpn = await UnitOfWork.VpnRepository.GetByIDAsync(vpnID);
            await UpdateRequiresSyncAsync(vpn, requiresSync, saveChanges);

            return;
        }

        /// <summary>
        /// Update the RequiresSync property of a collecton of vpn records.
        /// </summary>
        /// <param name="vpns"></param>
        /// <param name="requiresSync"></param>
        /// <returns></returns>
        public async Task UpdateRequiresSyncAsync(IEnumerable<Vpn> vpns, bool requiresSync, bool saveChanges = true)
        {
            var tasks = new List<Task>();
            foreach (var vpn in vpns)
            {
                await UpdateRequiresSyncAsync(vpn.VpnID, requiresSync, saveChanges);
            }

            return;
        }

        private async Task<IEnumerable<ServiceResult>> ExecuteTasksAsync(IList<Task<ServiceResult>> tasks,
            IProgress<ServiceResult> progress)
        {
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
    }
}
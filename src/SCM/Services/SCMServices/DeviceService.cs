using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Data;
using SCM.Models.NetModels.AttachmentNetModels;
using AutoMapper;
using System.Net;

namespace SCM.Services.SCMServices
{
    public class DeviceService : BaseService, IDeviceService
    {
        public DeviceService(IUnitOfWork unitOfWork, IMapper mapper, INetworkSyncService netsync) : base(unitOfWork, mapper, netsync)
        {
        }

        public async Task<IEnumerable<Device>> GetAllAsync()
        {
            return await this.UnitOfWork.DeviceRepository.GetAsync(includeProperties:"Location,Plane");
        }

        public async Task<Device> GetByIDAsync(int id)
        {
            var result = await this.UnitOfWork.DeviceRepository.GetAsync(d => d.ID == id, includeProperties: "Location,Plane");
            return result.SingleOrDefault();
        }

        public async Task<int> AddAsync(Device device)
        {
            this.UnitOfWork.DeviceRepository.Insert(device);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> UpdateAsync(Device device)
        {
            this.UnitOfWork.DeviceRepository.Update(device);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<ServiceResult> DeleteAsync(Device device)
        {
            var serviceResult = new ServiceResult();
            serviceResult.IsSuccess = true;

            // Delete from the network first

            var syncResult = await DeleteFromNetworkAsync(device.ID);

            if (!syncResult.IsSuccess && syncResult.NetworkHttpResponse.HttpStatusCode != HttpStatusCode.NotFound)
            {
                serviceResult.IsSuccess = false;
                serviceResult.Add(syncResult.GetAllMessages());
                return serviceResult;
            }


            // Now update the inventory

            this.UnitOfWork.DeviceRepository.Delete(device);
            await this.UnitOfWork.SaveAsync();

            return serviceResult;
        }

        public async Task<NetworkCheckSyncServiceResult> CheckNetworkSyncAsync(int deviceID)
        {
            var checkSyncResult = new NetworkCheckSyncServiceResult();

            var deviceDbResult = await UnitOfWork.DeviceRepository.GetAsync(q => q.ID == deviceID,
                           includeProperties: "Vrfs,Interfaces.Port,Interfaces.InterfaceBandwidth,"
                               + "Interfaces.Vrf.BgpPeers,"
                               + "Interfaces.InterfaceVlans.Vrf.BgpPeers,"
                               + "Interfaces.BundleInterfacePorts.Port");

            var device = deviceDbResult.SingleOrDefault();
            if (device == null)
            {
                checkSyncResult.NetworkSyncServiceResult.Add("The Device was not found.");
            }
            else
            {
                var attachmentServiceModelData = Mapper.Map<AttachmentServiceNetModel>(device);
                checkSyncResult = await NetSync.CheckNetworkSyncAsync(attachmentServiceModelData, "/attachment/pe/" + device.Name);
            }

            await UpdateDeviceRequiresSyncAsync(device, !checkSyncResult.InSync);

            return checkSyncResult;
        }

        public async Task<NetworkSyncServiceResult> SyncToNetworkAsync(int deviceID)
        {
            var syncResult = new NetworkSyncServiceResult();
            syncResult.IsSuccess = true;

            var deviceDbResult = await UnitOfWork.DeviceRepository.GetAsync(q => q.ID == deviceID, 
                includeProperties: "Vrfs,Interfaces.Port,Interfaces.InterfaceBandwidth,"
                               + "Interfaces.Vrf.BgpPeers,"
                               + "Interfaces.InterfaceVlans.Vrf.BgpPeers,"
                               + "Interfaces.BundleInterfacePorts.Port,"
                               + "Interfaces.ContractBandwidthPool.ContractBandwidth,"
                               + "Interfaces.InterfaceVlans.ContractBandwidthPool.ContractBandwidth");

            var device = deviceDbResult.SingleOrDefault();
            if (device == null)
            {
                syncResult.Add("The Device was not found.");
                syncResult.IsSuccess = false;
            }
            else
            {
                var attachmentServiceModelData = Mapper.Map<AttachmentServiceNetModel>(device);
                syncResult = await NetSync.SyncNetworkAsync(attachmentServiceModelData, "/attachment/pe/" + device.Name);
            }

            await UpdateDeviceRequiresSyncAsync(device, !syncResult.IsSuccess);

            return syncResult;
        }
        public async Task<NetworkSyncServiceResult> DeleteFromNetworkAsync(int deviceID)
        {
            var syncResult = new NetworkSyncServiceResult();
            syncResult.IsSuccess = true;

            var dbResult = await UnitOfWork.DeviceRepository.GetAsync(q => q.ID == deviceID, AsTrackable: false);
            var device = dbResult.SingleOrDefault();

            if (device == null)
            {
                syncResult.Add("The Device was not found.");
                syncResult.IsSuccess = false;
            }
            else
            {
                syncResult = await NetSync.DeleteFromNetworkAsync("/attachment/pe/" + device.Name);
            }

            await UpdateDeviceRequiresSyncAsync(device, true);

            return syncResult;
        }

        /// <summary>
        /// Helper to update the RequiresSync property of a vpn record.
        /// </summary>
        /// <param name="vpn"></param>
        /// <param name="requiresSync"></param>
        /// <returns></returns>
        private async Task<int> UpdateDeviceRequiresSyncAsync(Device device, bool requiresSync)
        {
            device.RequiresSync = requiresSync;
            UnitOfWork.DeviceRepository.Update(device);

            return await UnitOfWork.SaveAsync();
        }
    }
}

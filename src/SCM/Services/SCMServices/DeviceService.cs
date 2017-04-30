using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
            return await this.UnitOfWork.DeviceRepository.GetAsync(includeProperties: "Vrfs,"
                               + "Interfaces.Ports,"
                               + "Attachments.Interfaces.Ports,"
                               + "Attachments.AttachmentBandwidth,"
                               + "Attachments.Vrf.BgpPeers,"
                               + "Attachments.ContractBandwidthPool.ContractBandwidth,"
                               + "Attachments.Vifs.Vlans,"
                               + "Attachments.Vifs.Vrf.BgpPeers,"
                               + "Attachments.Vifs.ContractBandwidthPool.ContractBandwidth", AsTrackable: false);
        }

        public async Task<Device> GetByIDAsync(int id)
        {
            var result = await this.UnitOfWork.DeviceRepository.GetAsync(d => d.ID == id,
                includeProperties: "Vrfs,"
                               + "Interfaces.Ports,"
                               + "Attachments.Interfaces.Ports,"
                               + "Attachments.AttachmentBandwidth,"
                               + "Attachments.Vrf.BgpPeers,"
                               + "Attachments.ContractBandwidthPool.ContractBandwidth,"
                               + "Attachments.Vifs.Vlans,"
                               + "Attachments.Vifs.Vrf.BgpPeers,"
                               + "Attachments.Vifs.ContractBandwidthPool.ContractBandwidth", AsTrackable: false);

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
            var result = new ServiceResult { IsSuccess = true };
       
            // Delete from the network first

            var syncResult = await DeleteFromNetworkAsync(device.ID);

            // Delete from network may return IsSuccess false if the resource was not found - this should be ignored

            if (!syncResult.IsSuccess)
            {
                foreach (var r in syncResult.NetworkSyncServiceResults)
                {
                    if (r.HttpStatusCode != HttpStatusCode.NotFound)
                    {
                        result.IsSuccess = false;

                        return result;
                    }
                }
            }

            // Now update the inventory

            this.UnitOfWork.DeviceRepository.Delete(device);
            await this.UnitOfWork.SaveAsync();

            return result;
        }

        public async Task<ServiceResult> CheckNetworkSyncAsync(int deviceID)
        {
            var result = new ServiceResult();
            var device = await GetByIDAsync(deviceID);

            if (device == null)
            {
                result.Add("The device was not found.");
            }
            else
            {
                var attachmentServiceModelData = Mapper.Map<AttachmentServiceNetModel>(device);
                var syncResult = await NetSync.CheckNetworkSyncAsync(attachmentServiceModelData, "/attachment/pe/" + device.Name);

                result.AddRange(syncResult.Messages);
                result.IsSuccess = syncResult.IsSuccess;
            }

            await UpdateDeviceRequiresSyncAsync(device, !result.IsSuccess);

            return result;
        }

        public async Task<ServiceResult> SyncToNetworkAsync(int deviceID)
        {
            var result = new ServiceResult();
            var device = await GetByIDAsync(deviceID);

            if (device == null)
            {
                result.Add("The device was not found.");
            }
            else
            {
                var attachmentServiceModelData = Mapper.Map<AttachmentServiceNetModel>(device);
                var syncResult = await NetSync.SyncNetworkAsync(attachmentServiceModelData, "/attachment/pe/" + device.Name);

                result.AddRange(syncResult.Messages);
                result.IsSuccess = syncResult.IsSuccess;
            }

            await UpdateDeviceRequiresSyncAsync(device, !result.IsSuccess);

            return result;
        }

        public async Task<ServiceResult> DeleteFromNetworkAsync(int deviceID)
        {
            var result = new ServiceResult();
            var device = await GetByIDAsync(deviceID);

            if (device == null)
            {
                result.Add("The Device was not found.");
            }
            else
            {
                var syncResult = await NetSync.DeleteFromNetworkAsync("/attachment/pe/" + device.Name);

                result.AddRange(syncResult.Messages);
                result.IsSuccess = syncResult.IsSuccess;
            }

            await UpdateDeviceRequiresSyncAsync(device, true);

            return result;
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

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
                               + "Attachments.Interfaces.Ports.Interface.Vlans,"
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
                               + "Attachments.Interfaces.Ports.Interface.Vlans,"
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
       
            this.UnitOfWork.DeviceRepository.Delete(device);
            await this.UnitOfWork.SaveAsync();

            return result;
        }

        /// <summary>
        /// Perform shallow check of network sync state of a collection
        /// of devices by checking the 'RequiresSync' property.
        /// </summary>
        /// <param name="devices"></param>
        /// <returns></returns>
        public ServiceResult ShallowCheckNetworkSync(IEnumerable<Device> devices)
        {
            var result = new ServiceResult { IsSuccess = true };

            var devicesRequireSync = devices.Where(q => q.RequiresSync);
            if (devicesRequireSync.Count() > 0)
            {
                result.IsSuccess = false;
                result.Add("The following devices require synchronisation with the network:");
                devicesRequireSync.ToList().ForEach(f => result.Add($"'{f.Name}'"));
            }

            return result;
        }


        public async Task<ServiceResult> CheckNetworkSyncAsync(Device device)
        {
            var result = new ServiceResult();
            var attachmentServiceModelData = Mapper.Map<AttachmentServiceNetModel>(device);
            var syncResult = await NetSync.CheckNetworkSyncAsync(attachmentServiceModelData, "/attachment/pe/" + device.Name);

            result.AddRange(syncResult.Messages);
            result.IsSuccess = syncResult.IsSuccess;
                    
            await UpdateDeviceRequiresSyncAsync(device, !result.IsSuccess);

            return result;
        }

        public async Task<ServiceResult> SyncToNetworkAsync(Device device)
        {
            var result = new ServiceResult();
            var attachmentServiceModelData = Mapper.Map<AttachmentServiceNetModel>(device);
            var syncResult = await NetSync.SyncNetworkAsync(attachmentServiceModelData, "/attachment/pe/" + device.Name);

            result.AddRange(syncResult.Messages);
            result.IsSuccess = syncResult.IsSuccess;
        
            await UpdateDeviceRequiresSyncAsync(device, !result.IsSuccess);

            return result;
        }

        public async Task<ServiceResult> DeleteFromNetworkAsync(Device device)
        {
            var result = new ServiceResult();

            var syncResult = await NetSync.DeleteFromNetworkAsync("/attachment/pe/" + device.Name);

            result.AddRange(syncResult.Messages);
            result.IsSuccess = syncResult.IsSuccess;

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

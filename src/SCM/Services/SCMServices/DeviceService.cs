using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Data;
using SCM.Models.NetModels;
using AutoMapper;

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

        public async Task<int> DeleteAsync(Device device)
        {
            this.UnitOfWork.DeviceRepository.Delete(device);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<NetworkSyncServiceResult> SyncToNetwork(int deviceID)
        {
            var syncResult = new NetworkSyncServiceResult();
            syncResult.IsSuccess = true;

            var deviceDbResult = await UnitOfWork.DeviceRepository.GetAsync(q => q.ID == deviceID, 
                includeProperties: "Ports.Interface.InterfaceBandwidth," 
                    + "Ports.Interface.InterfaceVlans.Vrf.BgpPeers,Ports.Interface.Vrf.BgpPeers,BundleInterfaces.Vrf.BgpPeers,"
                    + "BundleInterfaces.InterfaceBandwidth,BundleInterfaces.BundleInterfacePorts," 
                    + "BundleInterfaces.BundleInterfaceVlans.Vrf.BgpPeers");

            var device = deviceDbResult.SingleOrDefault();
            if (device == null)
            {
                syncResult.Add("The Device was not found.");
                syncResult.IsSuccess = false;
            }
            else
            {
                var attachmentServiceModelData = new AttachmentServiceNetModel();
                attachmentServiceModelData.PEs = Mapper.Map<List<PeAttachmentNetModel>>(deviceDbResult);

                syncResult = await NetSync.SyncToNetwork(attachmentServiceModelData, "/attachment");
            }

            return syncResult;
        }
    }
}

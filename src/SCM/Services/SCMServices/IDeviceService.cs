using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Models.NetModels;
using SCM.Data;

namespace SCM.Services.SCMServices
{
    public interface IDeviceService
    {
        IUnitOfWork UnitOfWork { get; }
        Task<IEnumerable<Device>> GetAllAsync();
        Task<Device> GetByIDAsync(int id);
        Task<int> AddAsync(Device device);
        Task<int> UpdateAsync(Device device);
        Task<ServiceResult> DeleteAsync(Device device);
        Task<NetworkSyncServiceResult> CheckNetworkSyncAsync(int deviceID);
        Task<NetworkSyncServiceResult> SyncToNetworkAsync(int deviceID);
        Task<NetworkSyncServiceResult> DeleteFromNetworkAsync(int deviceID);
    }
}

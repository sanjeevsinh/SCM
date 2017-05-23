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
        Task<ServiceResult> AddAsync(Device device);
        Task<int> UpdateAsync(Device device);
        Task<ServiceResult> DeleteAsync(Device device);
        ServiceResult ShallowCheckNetworkSync(IEnumerable<Device> devices);
        Task<IEnumerable<ServiceResult>> CheckNetworkSyncAsync(IEnumerable<Device> devices, IProgress<ServiceResult> progress);
        Task<ServiceResult> CheckNetworkSyncAsync(Device device);
        Task<IEnumerable<ServiceResult>> SyncToNetworkAsync(IEnumerable<Device> devices, IProgress<ServiceResult> progress);
        Task<ServiceResult> SyncToNetworkAsync(Device device);
        Task<ServiceResult> DeleteFromNetworkAsync(Device device);
        Task UpdateRequiresSyncAsync(int deviceID, bool requiresSync, bool saveChanges = true);
        Task UpdateRequiresSyncAsync(Device device, bool requiresSync, bool saveChanges = true);
    }
}

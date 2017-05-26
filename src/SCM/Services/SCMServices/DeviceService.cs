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
                               + "Ports.PortBandwidth,"
                               + "Interfaces.Ports,"
                               + "Attachments.Interfaces.Ports.Interface.Vlans,"
                               + "Attachments.AttachmentBandwidth,"
                               + "Attachments.Vrf.BgpPeers,"
                               + "Attachments.ContractBandwidthPool.ContractBandwidth,"
                               + "Attachments.Vifs.Vlans,"
                               + "Attachments.Vifs.Vrf.BgpPeers,"
                               + "Attachments.Vifs.ContractBandwidthPool.ContractBandwidth,"
                               + "Plane,"
                               + "Location", AsTrackable: false);
        }

        public async Task<Device> GetByIDAsync(int id)
        {
            var result = await this.UnitOfWork.DeviceRepository.GetAsync(d => d.ID == id,
                includeProperties: "Vrfs,"
                               + "Ports.PortBandwidth,"
                               + "Interfaces.Ports,"
                               + "Attachments.Interfaces.Ports.Interface.Vlans,"
                               + "Attachments.AttachmentBandwidth,"
                               + "Attachments.Vrf.BgpPeers,"
                               + "Attachments.ContractBandwidthPool.ContractBandwidth,"
                               + "Attachments.Vifs.Vlans,"
                               + "Attachments.Vifs.Vrf.BgpPeers,"
                               + "Attachments.Vifs.ContractBandwidthPool.ContractBandwidth,"
                               + "Plane,"
                               + "Location", AsTrackable: false);

            return result.SingleOrDefault();
        }

        public async Task<ServiceResult> AddAsync(Device device)
        {
            var result = new ServiceResult
            {
                IsSuccess = true
            };

            device.Created = true;
            device.RequiresSync = true;
            this.UnitOfWork.DeviceRepository.Insert(device);
            await this.UnitOfWork.SaveAsync();

            return result;
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
                result.Add("The following devices require synchronisation with the network:.");
                devicesRequireSync.ToList().ForEach(f => result.Add($"'{f.Name}'."));
            }

            return result;
        }
        public async Task<IEnumerable<ServiceResult>> CheckNetworkSyncAsync(IEnumerable<Device> devices,
            IProgress<ServiceResult> progress)
        {
            List<Task<ServiceResult>> tasks = (from device in devices select CheckNetworkSyncAsync(device)).ToList();
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

        public async Task<ServiceResult> CheckNetworkSyncAsync(Device device)
        {
            var result = new ServiceResult
            {
                IsSuccess = true,
                Item = device
            };

            try
            {

                var attachmentServiceModelData = Mapper.Map<AttachmentServiceNetModel>(device);
                var syncResult = await NetSync.CheckNetworkSyncAsync(attachmentServiceModelData, "/attachment/pe/" + device.Name);

                result.NetworkSyncServiceResults.Add(syncResult);

                if (!syncResult.IsSuccess)
                {
                    result.IsSuccess = false;
                    device.RequiresSync = true;

                    if (syncResult.StatusCode == NetworkSyncStatusCode.Success)
                    {
                        // Request was successfully executed and the device was tested for sync with the network

                        result.Add($"Device '{device.Name}' is not synchronised with the network.");
                    }
                    else
                    {
                        // Request failed to execute for some reason - e.g server down, no network etc

                        result.Add($"There was an error checking status for device '{device.Name}'.");
                    }
                }
                else
                {
                    device.RequiresSync = false;
                }

            }

            catch (Exception ex)
            {
                return result;
            }

            return result;
        }

        public async Task<IEnumerable<ServiceResult>> SyncToNetworkAsync(IEnumerable<Device> devices,
           IProgress<ServiceResult> progress)
        {
            List<Task<ServiceResult>> tasks = (from device in devices select SyncToNetworkAsync(device)).ToList();
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

        public async Task<ServiceResult> SyncToNetworkAsync(Device device)
        {
            var result = new ServiceResult
            {
                IsSuccess = true,
                Item = device
            };

            var attachmentServiceModelData = Mapper.Map<AttachmentServiceNetModel>(device);
            var syncResult = await NetSync.SyncNetworkAsync(attachmentServiceModelData, "/attachment/pe/" + device.Name);

            result.NetworkSyncServiceResults.Add(syncResult);

            if (!syncResult.IsSuccess)
            {
                result.IsSuccess = false;
                device.RequiresSync = true;

                if (syncResult.StatusCode == NetworkSyncStatusCode.Success)
                {
                    // Request was successfully executed but synchronisation failed

                    result.Add($"Failed to synchronise device '{device.Name}' with the network.");
                }
                else
                {
                    // Request failed to execute for some reason - e.g server down, no network etc

                    result.Add($"There was an error synchronising device '{device.Name}' with the network.");
                }
            }
            else
            {
                device.RequiresSync = false;
            }

            return result;
        }

        public async Task<ServiceResult> DeleteFromNetworkAsync(Device device)
        {
            var result = new ServiceResult();

            var syncResult = await NetSync.DeleteFromNetworkAsync("/attachment/pe/" + device.Name);

            result.AddRange(syncResult.Messages);
            result.IsSuccess = syncResult.IsSuccess;

            return result;
        }

        /// <summary>
        /// Helper to update the RequiresSync property of a device record.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="requiresSync"></param>
        /// <returns></returns>
        public async Task UpdateRequiresSyncAsync(Device device, bool requiresSync, bool saveChanges = true)
        {
            device.RequiresSync = requiresSync;
            UnitOfWork.DeviceRepository.Update(device);
            if (saveChanges)
            {
                await UnitOfWork.SaveAsync();
            }

            return;
        }

        /// <summary>
        /// Helper to update the RequiresSync property of a device record.
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="requiresSync"></param>
        /// <returns></returns>
        public async Task UpdateRequiresSyncAsync(int deviceID, bool requiresSync, bool saveChanges = true)
        {
            var device = await GetByIDAsync(deviceID);
            await UpdateRequiresSyncAsync(device, requiresSync, saveChanges);

            return;
        }
    }
}

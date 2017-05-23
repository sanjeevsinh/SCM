using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SCM.Models.ServiceModels;
using SCM.Services.SCMServices;
using AutoMapper;
using SCM.Models.ViewModels;
using SCM.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using SCM.Hubs;
using System.Net;
using Microsoft.AspNetCore.Http;
using System.Net.Http;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace SCM.Api.Controllers
{
    [Route("api")]
    public class DeviceApiController : BaseApiController
    {
        public DeviceApiController(IDeviceService deviceService,
            IMapper mapper,
            IConnectionManager signalRConnectionManager) : base(mapper)
        {
            DeviceService = deviceService;
            HubContext = signalRConnectionManager.GetHubContext<NetworkSyncHub>();
        }

        private IDeviceService DeviceService { get; set; }
        private IHubContext HubContext { get; set; }

        // GET: api/devices
        [HttpGet("devices")]
        public async Task<IEnumerable<DeviceViewModel>> GetAll(int id)
        {
            var devices = await DeviceService.GetAllAsync();
            return Mapper.Map<List<DeviceViewModel>>(devices);
        }

        // GET api/devices/1
        [HttpGet("devices/{id}")]
        public async Task<DeviceViewModel> Get(int id)
        {
            var device = await DeviceService.GetByIDAsync(id);
            return Mapper.Map<DeviceViewModel>(device);
        }

        // POST api/devices
        [HttpPost("devices")]
        public async Task<IActionResult> Create(DeviceViewModel device)
        {
            var mappedRequest = Mapper.Map<Device>(device);

            var result = await DeviceService.AddAsync(mappedRequest);
            if (!result.IsSuccess)
            {
                result.GetMessageList().ForEach(message => ModelState.AddModelError(string.Empty, message));
                return BadRequest(ModelState);
            }
            else
            {
                var deviceItem = Mapper.Map<Device>(result.Item);
                return CreatedAtRoute("GetDevice", new { id = deviceItem.ID }, deviceItem);
            }
        }

        // PUT api/devices/5
        [HttpPut("devices/{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/devices/5
        [HttpDelete("devices/{id}")]
        public void Delete(int id)
        {
        }

        [HttpPost("devices/{id}/checksync")]
        public async Task<ActionResult> CheckSync(int id)
        {
            var item = await DeviceService.GetByIDAsync(id);
            if (item == null)
            {
                return BadRequest("The device was not found.");
            }

            var result = await DeviceService.CheckNetworkSyncAsync(item);
            var mappedItem = Mapper.Map<DeviceViewModel>(item);

            await DeviceService.UpdateRequiresSyncAsync(item, !result.IsSuccess, true);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    Success = true,
                    message = $"Device {item.Name} has been checked and is synchronised with the network."
                });
            }
            else
            {
                if (result.NetworkSyncServiceResults.Single().StatusCode == NetworkSyncStatusCode.Success)
                {
                    return Ok(new
                    {
                        Success = false,
                        Message = $"Device {item.Name} has been checked and is NOT synchronised with the network."
                        + "Press the 'Sync' button to update the network."
                    });
                }
                else
                {
                    return Ok(new
                    {
                        Success = false,
                        Message = result.GetMessage()
                    });
                }
            }
        }

        [HttpPost("devices/checksync")]
        public async Task<IActionResult> CheckSyncAll()
        {
            var devices = await DeviceService.GetAllAsync();
            if (devices.Count() == 0)
            {
                return Ok(new
                {
                    Success = true,
                    Message = "No devices were found."
                });
            }

            var progress = new Progress<ServiceResult>(UpdateClientProgress);
            var results = await DeviceService.CheckNetworkSyncAsync(devices, progress);

            foreach (var r in results)
            {
                var item = (Device)r.Item;
                await DeviceService.UpdateRequiresSyncAsync(item, !r.IsSuccess, true);
            }

            if (results.Where(q => q.IsSuccess).Count() == results.Count())
            {
                return Ok(new
                {
                    Success = true,
                    Message = "All devices have been checked and are synchronised with the network."
                });
            }
            else
            {
                var message = string.Empty;
                results.ToList().ForEach(q => message += q.GetMessage());

                return Ok(new
                {
                    Success = false,
                    Message = message
                });
            }
        }

        [HttpPost("devices/{id}/sync")]
        public async Task<IActionResult> Sync(int id)
        {
            var item = await DeviceService.GetByIDAsync(id);
            if (item == null)
            {
                return BadRequest("The device was not found.");
            }

            var result = await DeviceService.SyncToNetworkAsync(item);
            item.RequiresSync = !result.IsSuccess;
            item.Created = false;
            await DeviceService.UpdateAsync(item);

            var mappedItem = Mapper.Map<DeviceViewModel>(item);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    Success = true,
                    Message = $"Device {item.Name} is synchronised with the network."
                });
            }
            else
            {
                return Ok(new
                {
                    Success = false,
                    Message = result.GetMessage()
                });
            }
        }

        [HttpPost("devices/sync")]
        public async Task<IActionResult> SyncAll()
        {
            var devices = await DeviceService.GetAllAsync();
            if (devices.Count() == 0)
            {
                return Ok(new
                {
                    Success = true,
                    Message = "No devices were found."
                });
            }

            var progress = new Progress<ServiceResult>(UpdateClientProgress);
            var message = string.Empty;

            var results = await DeviceService.SyncToNetworkAsync(devices, progress);

            foreach (var r in results)
            {
                var item = (Device)r.Item;
                item.RequiresSync = !r.IsSuccess;
                item.Created = false;
                await DeviceService.UpdateAsync(item);
            }

            if (results.Where(q => q.IsSuccess).Count() == results.Count())
            {
                return Ok(new
                {
                    Success = true,
                    Message = "All devices are synchronised with the network."
                });
            }
            else
            {
                results.ToList().ForEach(q => message += q.GetMessage());
                return Ok(new
                {
                    Success = false,
                    Message = message
                });
            }
        }

        /// <summary>
        /// Delegate method which is called when sync or checksync of an 
        /// individual device has completed.
        /// </summary>
        /// <param name="result"></param>
        private void UpdateClientProgress(ServiceResult result)
        {
            var device = (Device)result.Item;

            // Update all clients which are subscribed to the attachment context
            // supplied in the result object

            HubContext.Clients.Group($"Devices_{device.ID}")
                .onSingleComplete(Mapper.Map<DeviceViewModel>(device), result.IsSuccess);
        }
    }
}
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
    public class VpnApiController : BaseApiController
    {
        public VpnApiController(IVpnService vifService,
            IAttachmentSetService attachmentSetService,
            IMapper mapper,
            IConnectionManager signalRConnectionManager) : base(mapper)
        {
            VpnService = vifService;
            AttachmentSetService = attachmentSetService;
            HubContext = signalRConnectionManager.GetHubContext<NetworkSyncHub>();
        }

        private IVpnService VpnService { get; set; }
        private IAttachmentSetService AttachmentSetService { get; set; }
        private IHubContext HubContext { get; set; }

        // GET: api/vpns
        [HttpGet("vpns")]
        public async Task<IEnumerable<VpnViewModel>> GetAll(int id)
        {
            var vpns = await VpnService.GetAllAsync();
            return Mapper.Map<List<VpnViewModel>>(vpns);
        }

        // GET api/vpns/1
        [HttpGet("vpns/{id}")]
        public async Task<VpnViewModel> Get(int id)
        {
            var vpn = await VpnService.GetByIDAsync(id);
            return Mapper.Map<VpnViewModel>(vpn);
        }

        // POST api/vpns
        [HttpPost("vpns")]
        public async Task<IActionResult> Create(VpnViewModel vpn)
        { 
            var mappedRequest = Mapper.Map<Vpn>(vpn);
            var validationResult = await VpnService.ValidateNewAsync(mappedRequest);

            if (!validationResult.IsSuccess)
            {
                validationResult.GetMessageList().ForEach(message => ModelState.AddModelError(string.Empty, message));
                return BadRequest(ModelState);
            }
            else
            {
                var result = await VpnService.AddAsync(mappedRequest);
                if (!result.IsSuccess)
                {
                    result.GetMessageList().ForEach(message => ModelState.AddModelError(string.Empty, message));
                    return BadRequest(ModelState);
                }
                else
                {
                    var vpnItem = Mapper.Map<Vpn>(result.Item);
                    return CreatedAtRoute("GetVpn", new { id = vpnItem.VpnID }, vpnItem);
                }
            }
        }

        // PUT api/vpns/5
        [HttpPut("vpns/{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/vpns/5
        [HttpDelete("vpns/{id}")]
        public void Delete(int id)
        {
        }

        [HttpPost("vpns/{id}/checksync")]
        public async Task<ActionResult> CheckSync(int id)
        {
            var item = await VpnService.GetByIDAsync(id);
            if (item == null)
            {
                return BadRequest("The vpn was not found.");
            }

            var result = await VpnService.CheckNetworkSyncAsync(item);
            var mappedItem = Mapper.Map<VpnViewModel>(item);

            await VpnService.UpdateRequiresSyncAsync(item, !result.IsSuccess, true);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    Success = true,
                    message = $"Vpn {item.Name} has been checked and is synchronised with the network."
                });
            }
            else
            {
                if (result.NetworkSyncServiceResults.Single().StatusCode == NetworkSyncStatusCode.Success)
                {
                    return Ok(new
                    {
                        Success = false,
                        Message = $"Vpn {item.Name} has been checked and is NOT synchronised with the network."
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

        [HttpPost("attachmentsets/{id}/vpns/checksync")]
        public async Task<IActionResult> CheckSyncAllByAttachmentSetID(int id)
        {
            var attachmentSet = await AttachmentSetService.GetByIDAsync(id);
            if (attachmentSet == null)
            {
                return BadRequest("The attachment set was not found.");
            }

            var vpns = await VpnService.GetAllByAttachmentSetIDAsync(id);
            if (vpns.Count() == 0)
            {
                return Ok(new
                {
                    Success = true,
                    Message = "No vpns were found."
                });
            }

            var progress = new Progress<ServiceResult>(UpdateClientProgress);
            var results = await VpnService.CheckNetworkSyncAsync(vpns, attachmentSet, progress);

            foreach (var r in results)
            {
                var item = (Vpn)r.Item;
                await VpnService.UpdateRequiresSyncAsync(item, !r.IsSuccess, true);
            }

            if (results.Where(q => q.IsSuccess).Count() == results.Count())
            {
                return Ok(new
                {
                    Success = true,
                    Message = "All vpns have been checked and are synchronised with the network."
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

        [HttpPost("vpns/{id}/sync")]
        public async Task<IActionResult> Sync(int id)
        {
            var item = await VpnService.GetByIDAsync(id);
            if (item == null)
            {
                return BadRequest("The vpn was not found.");
            }

            var result = await VpnService.SyncToNetworkAsync(item);
            item.RequiresSync = !result.IsSuccess;
            item.Created = false;
            await VpnService.UpdateAsync(item);

            var mappedItem = Mapper.Map<VpnViewModel>(item);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    Success = true,
                    Message = $"Vpn {item.Name} is synchronised with the network."
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

        [HttpPost("attachmentsets/{id}/vpns/sync")]
        public async Task<IActionResult> SyncAllByAttachmentSetID(int id)
        {
            var attachmentSet = await AttachmentSetService.GetByIDAsync(id);
            if (attachmentSet == null)
            {
                return BadRequest("The attachment set was not found.");
            }

            var vpns = await VpnService.GetAllByAttachmentSetIDAsync(id);
            if (vpns.Count() == 0)
            {
                return Ok(new
                {
                    Success = true,
                    Message = "No vpns were found."
                });
            }

            var progress = new Progress<ServiceResult>(UpdateClientProgress);
            var message = string.Empty;

            var results = await VpnService.SyncToNetworkAsync(vpns, attachmentSet, progress);

            foreach (var r in results)
            {
                var item = (Vpn)r.Item;
                item.RequiresSync = !r.IsSuccess;
                item.Created = false;
                await VpnService.UpdateAsync(item);
            }

            if (results.Where(q => q.IsSuccess).Count() == results.Count())
            {
                return Ok(new
                {
                    Success = true,
                    Message = "All vpns are synchronised with the network."
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
        /// individual vpn has completed.
        /// </summary>
        /// <param name="result"></param>
        private void UpdateClientProgress(ServiceResult result)
        {
            var vpn = (Vpn)result.Item;
            var attachmentSet = (AttachmentSet)result.Context;

            // Update all clients which are subscribed to the attachment context
            // supplied in the result object

            HubContext.Clients.Group($"AttachmentSet_{attachmentSet.AttachmentSetID}")
                .onSingleComplete(Mapper.Map<VpnViewModel>(vpn), result.IsSuccess);
        }
    }
}

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
    public class VifApiController : BaseApiController
    {
        public VifApiController(IVifService vifService,
            IAttachmentService attachmentService,
            IVrfService vrfService,
            IMapper mapper,
            IConnectionManager signalRConnectionManager) : base(mapper)
        {
            AttachmentService = attachmentService;
            VifService = vifService;
            VrfService = vrfService;
            HubContext = signalRConnectionManager.GetHubContext<NetworkSyncHub>();
        }

        private IAttachmentService AttachmentService { get; set; }
        private IVifService VifService { get; set; }
        private IVrfService VrfService { get; set; }
        private IHubContext HubContext { get; set; }

        // GET: api/attachments/1/vifs
        [HttpGet("attachments/{id}/vifs")]
        public async Task<IEnumerable<VifViewModel>> GetAllByAttachmentID(int id)
        {
            var vifs = await VifService.GetAllByAttachmentIDAsync(id);
            return Mapper.Map<List<VifViewModel>>(vifs);
        }

        // GET api/vifs/1
        [HttpGet("vifs/{id}")]
        public async Task<VifViewModel> Get(int id)
        {
            var vif = await VifService.GetByIDAsync(id);
            return Mapper.Map<VifViewModel>(vif);
        }

        // POST api/values
        [HttpPost("vifs")]
        public async Task<IActionResult> Create(VifRequestViewModel request)
        { 
            var mappedRequest = Mapper.Map<VifRequest>(request);
            var validationResult = await VifService.ValidateNewAsync(mappedRequest);

            if (!validationResult.IsSuccess)
            {
                validationResult.GetMessageList().ForEach(message => ModelState.AddModelError(string.Empty, message));
                return BadRequest(ModelState);
            }
            else
            {
                var result = await VifService.AddAsync(mappedRequest);
                if (!result.IsSuccess)
                {
                    result.GetMessageList().ForEach(message => ModelState.AddModelError(string.Empty, message));
                    return BadRequest(ModelState);
                }
                else
                {
                    var vif = Mapper.Map<Vif>(result.Item);
                    return CreatedAtRoute("GetVif", new { id = vif.VifID }, vif);
                }
            }
        }

        // PUT api/vifs/5
        [HttpPut("vifs/{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/vifs/5
        [HttpDelete("vifs/{id}")]
        public void Delete(int id)
        {
        }

        [HttpPost("vifs/{id}/checksync")]
        public async Task<ActionResult> CheckSync(int id)
        {
            var item = await VifService.GetByIDAsync(id);
            if (item == null)
            {
                return BadRequest("The vif was not found.");
            }

            var result = await VifService.CheckNetworkSyncAsync(item);
            await VifService.UpdateRequiresSyncAsync(item, !result.IsSuccess, true);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    Success = true,
                    Message = $"Vif {item.Name} has been checked and is synchronised with the network."
                });
            }
            else
            {
                if (result.NetworkSyncServiceResults.Single().StatusCode == NetworkSyncStatusCode.Success)
                {
                    return Ok(new
                    {
                        Success = false,
                        Message = $"Vif {item.Name} has been checked and is NOT synchronised with the network."
                        + "Press the 'Sync' button to update the network."
                    });
                }
                else
                {
                    return Ok(new
                    {
                        Success = false,
                        Message = result.GetMessageList()
                    });
                }
            }
        }

        [HttpPost("attachments/{id}/vifs/checksync")]
        public async Task<IActionResult> CheckSyncAllByAttachmentID(int id)
        {
            var attachment = await AttachmentService.GetByIDAsync(id);
            if (attachment == null)
            {
                return BadRequest("The attachment was not found.");
            }

            var vifs = await VifService.GetAllByAttachmentIDAsync(id);
            if (vifs.Count() == 0)
            {
                return Ok(new
                {
                    Success = true,
                    Message = "No vifs were found."
                });
            }

            var progress = new Progress<ServiceResult>(UpdateClientProgress);
            var results = await VifService.CheckNetworkSyncAsync(vifs, progress);

            foreach (var r in results)
            {
                var item = (Vif)r.Item;
                await VifService.UpdateRequiresSyncAsync(item, !r.IsSuccess, true);
            }

            if (results.Where(q => q.IsSuccess).Count() == results.Count())
            {
                return Ok(new
                {
                    Success = true,
                    Message = "All vifs have been checked and are synchronised with the network."
                });
            }
            else
            {
                return Ok(new
                {
                    Success = false,
                    Message = results.SelectMany(q => q.GetMessageList())
                });
            }
        }

        [HttpPost("vifs/{id}/sync")]
        public async Task<IActionResult> Sync(int id)
        {
            var item = await VifService.GetByIDAsync(id);
            if (item == null)
            {
                return BadRequest("The vif was not found.");
            }

            var result = await VifService.SyncToNetworkAsync(item);
            item.RequiresSync = !result.IsSuccess;
            item.Created = false;
            await VifService.UpdateAsync(item);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    Success = true,
                    Message = $"Vif {item.Name} is synchronised with the network."
                });
            }
            else
            {
                return Ok(new
                {
                    Success = false,
                    Message = result.GetMessageList()
                });
            }
        }

        [HttpPost("attachments/{id}/vifs/sync")]
        public async Task<IActionResult> SyncAllByAttachmentID(int id)
        {
            var attachment = await AttachmentService.GetByIDAsync(id);
            if (attachment == null)
            {
                return BadRequest("The attachment was not found.");
            }

            var vifs = await VifService.GetAllByAttachmentIDAsync(id);
            if (vifs.Count() == 0)
            {
                return Ok(new
                {
                    Success = true,
                    Message = "No vifs were found."
                });
            }

            var progress = new Progress<ServiceResult>(UpdateClientProgress);
            var results = await VifService.SyncToNetworkAsync(vifs, progress);

            foreach (var r in results)
            {
                var item = (Vif)r.Item;
                item.RequiresSync = !r.IsSuccess;
                item.Created = false;
                await VifService.UpdateAsync(item);
            }

            if (results.Where(q => q.IsSuccess).Count() == results.Count())
            {
                return Ok(new
                {
                    Success = true,
                    Message = "All vifs are synchronised with the network."
                });
            }
            else
            {
                return Ok(new
                {
                    Success = false,
                    Message = results.SelectMany(q => q.GetMessageList())
                });
            }
        }

        /// <summary>
        /// Delegate method which is called when sync or checksync of an 
        /// individual vif has completed.
        /// </summary>
        /// <param name="result"></param>
        private void UpdateClientProgress(ServiceResult result)
        {
            var vif = (Vif)result.Item;
            var attachment = (Attachment)result.Context;

            // Update all clients which are subscribed to the attachment context
            // supplied in the result object

            HubContext.Clients.Group($"Attachment_{attachment.AttachmentID}")
                .onSingleComplete(Mapper.Map<VifViewModel>(vif), result.IsSuccess);
        }
    }
}

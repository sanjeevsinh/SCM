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
    public class AttachmentApiController : BaseApiController
    {
        public AttachmentApiController(IAttachmentService attachmentService,
                                    IVrfService vrfService,
                                    ITenantService tenantService,
                                    IMapper mapper,
                                    IConnectionManager signalRConnectionManager) : base(mapper)
        {
            AttachmentService = attachmentService;
            VrfService = vrfService;
            TenantService = tenantService;
            HubContext = signalRConnectionManager.GetHubContext<NetworkSyncHub>();
        }

        private IAttachmentService AttachmentService { get; set; }
        private IVrfService VrfService { get; set; }
        private ITenantService TenantService { get; set; }
        private IHubContext HubContext { get; set; }

        // GET: api/tenant/1/attachments
        [HttpGet("tenant/{id}/attachments")]
        public async Task<IEnumerable<AttachmentViewModel>> GetAllByTenant(int id)
        {
            var tenant = await TenantService.GetByIDAsync(id);
            if (tenant != null)
            {
                var attachments = await AttachmentService.GetAllByTenantAsync(tenant);
                return Mapper.Map<List<AttachmentViewModel>>(attachments);
            }
            else
            {
                return null;
            }
        }

        // GET api/attachments/1
        [HttpGet("attachments/{id}")]
        public async Task<AttachmentViewModel> Get(int id)
        {
            var attachment = await AttachmentService.GetByIDAsync(id);
            return Mapper.Map<AttachmentViewModel>(attachment);
        }

        // POST api/values
        [HttpPost("attachments")]
        public async Task<IActionResult> Create(AttachmentRequestViewModel request)
        { 
            var mappedRequest = Mapper.Map<AttachmentRequest>(request);
            var validationResult = await AttachmentService.ValidateNewAsync(mappedRequest);

            if (!validationResult.IsSuccess)
            {
                validationResult.GetMessageList().ForEach(message => ModelState.AddModelError(string.Empty, message));
                return BadRequest(ModelState);
            }
            else
            {
                var result = await AttachmentService.AddAsync(mappedRequest);
                if (!result.IsSuccess)
                {
                    result.GetMessageList().ForEach(message => ModelState.AddModelError(string.Empty, message));
                    return BadRequest(ModelState);
                }
                else
                {
                    var attachment = Mapper.Map<Attachment>(result.Item);
                    return CreatedAtRoute("GetAttachment", new { id = attachment.AttachmentID }, attachment);
                }
            }
        }

        // PUT api/values/5
        [HttpPut("attachments/{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpPost("attachments/{id}/checksync")]
        public async Task<ActionResult> CheckSync(int id)
        {
            var item = await AttachmentService.GetByIDAsync(id);
            if (item == null)
            {
                return BadRequest("The attachment was not found.");
            }

            var result = await AttachmentService.CheckNetworkSyncAsync(item);
            var mappedItem = Mapper.Map<AttachmentViewModel>(item);

            await AttachmentService.UpdateRequiresSyncAsync(item, !result.IsSuccess, true);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    Success = true,
                    message = $"Attachment {item.Name} has been checked and is synchronised with the network."
                });
            }
            else
            {
                if (result.NetworkSyncServiceResults.Single().StatusCode == NetworkSyncStatusCode.Success)
                {
                    return Ok(new
                    {
                        Success = false,
                        Message = $"Attachment {item.Name} has been checked and is NOT synchronised with the network."
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

        [HttpPost("tenants/{id}/attachments/checksync")]
        public async Task<IActionResult> CheckSyncAllByTenantID(int id)
        {
            var tenant = await TenantService.GetByIDAsync(id);
            if (tenant == null)
            {
                return BadRequest("The tenant was not found.");
            }

            var attachments = await AttachmentService.GetAllByTenantIDAsync(id);
            if (attachments.Count() == 0)
            {
                return Ok(new
                {
                    Success = true,
                    Message = "No attachments were found."
                });
            }

            var progress = new Progress<ServiceResult>(UpdateClientProgress);
            var results = await AttachmentService.CheckNetworkSyncAsync(attachments, progress);

            foreach (var r in results)
            {
                var item = (Attachment)r.Item;
                await AttachmentService.UpdateRequiresSyncAsync(item, !r.IsSuccess, true);
            }

            if (results.Where(q => q.IsSuccess).Count() == results.Count())
            {
                return Ok(new
                {
                    Success = true,
                    Message = "All attachments have been checked and are synchronised with the network."
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

        [HttpPost("attachments/{id}/sync")]
        public async Task<IActionResult> Sync(int id)
        {
            var item = await AttachmentService.GetByIDAsync(id);
            if (item == null)
            {
                return BadRequest("The attachment was not found.");
            }

            var result = await AttachmentService.SyncToNetworkAsync(item);
            item.RequiresSync = !result.IsSuccess;
            item.Created = false;
            await AttachmentService.UpdateAsync(item);

            var mappedItem = Mapper.Map<AttachmentViewModel>(item);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    Success = true,
                    Message = $"Attachment {item.Name} is synchronised with the network."
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

        [HttpPost("tenants/{id}/attachments/sync")]
        public async Task<IActionResult> SyncAllByTenantID(int id)
        {
            var tenant = await TenantService.GetByIDAsync(id);
            if (tenant == null)
            {
                return BadRequest("The tenant was not found.");
            }

            var attachments = await AttachmentService.GetAllByTenantIDAsync(id);
            if (attachments.Count() == 0)
            {
                return Ok(new
                {
                    Success = true,
                    Message = "No attachments were found."
                });
            }

            var progress = new Progress<ServiceResult>(UpdateClientProgress);
            var message = string.Empty;

            var checkSyncResults = await AttachmentService.SyncToNetworkAsync(attachments, progress);

            foreach (var r in checkSyncResults)
            {
                var item = (Attachment)r.Item;
                item.RequiresSync = !r.IsSuccess;
                item.Created = false;
                await AttachmentService.UpdateAsync(item);
            }

            if (checkSyncResults.Where(q => q.IsSuccess).Count() == checkSyncResults.Count())
            {
                return Ok(new
                {
                    Success = true,
                    Message = "All attachments are synchronised with the network."
                });
            }
            else
            {
                checkSyncResults.ToList().ForEach(q => message += q.GetMessage());
                return Ok(new
                {
                    Success = false,
                    Message = message
                });
            }
        }

        /// <summary>
        /// Delegate method which is called when sync or checksync of an 
        /// individual Attachment has completed.
        /// </summary>
        /// <param name="result"></param>
        private void UpdateClientProgress(ServiceResult result)
        {
            var attachment = (Attachment)result.Item;
            var tenant = (Tenant)result.Context;

            // Update all clients which are subscribed to the Tenant context
            // supplied in the result object

            HubContext.Clients.Group($"TenantAttachment_{tenant.TenantID}")
                .onSingleComplete(Mapper.Map<AttachmentViewModel>(attachment), result.IsSuccess);
        }
    }
}

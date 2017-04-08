using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SCM.Models.ServiceModels;
using SCM.Models.ViewModels;
using SCM.Services;
using SCM.Services.SCMServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.Linq.Expressions;
using SCM.Data;
using SCM.Models;

namespace SCM.Controllers
{
    public class AttachmentOrVifController : BaseViewController
    {
        public AttachmentOrVifController(IAttachmentOrVifService attachmentOrVifService, 
            IAttachmentService attachmentService, IVifService vifService, IMapper mapper)
        {
            AttachmentOrVifService = attachmentOrVifService;
            Mapper = mapper;
            AttachmentService = attachmentService;
            VifService = vifService;
        }

        private IAttachmentOrVifService AttachmentOrVifService { get; set; }
        private IAttachmentService AttachmentService { get; set; }
        private IVifService VifService { get; set; }
        private IMapper Mapper { get; set; }

        [HttpGet]
        public async Task<IActionResult> GetAllByVpnID(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vpn = await AttachmentOrVifService.UnitOfWork.VpnRepository.GetByIDAsync(id);
            if (vpn == null)
            {
                return NotFound();
            }

            var result = await AttachmentOrVifService.GetAllByVpnIDAsync(id.Value);
            ViewBag.Vpn = vpn;
            return View(Mapper.Map<List<AttachmentOrVifViewModel>>(result));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id, [FromQuery]bool vif)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await AttachmentOrVifService.GetByIDAsync(id.Value, vif);

            return View(Mapper.Map<AttachmentOrVifViewModel>(item));
        }

        [HttpPost]
        public async Task<IActionResult> CheckSync(AttachmentOrVifViewModel attachmentOrVif)
        {

            NetworkCheckSyncServiceResult checkSyncResult;
            Object item;

            if (attachmentOrVif.IsVif)
            {
                item = await VifService.GetByIDAsync(attachmentOrVif.ID);
                if (item == null)
                {
                    return NotFound();
                }

                checkSyncResult = await VifService.CheckNetworkSyncAsync((Vif)item);
            }
            else
            {
                item = await AttachmentService.GetByIDAsync(attachmentOrVif.ID);
                if (item == null)
                {
                    return NotFound();
                }

                checkSyncResult = await AttachmentService.CheckNetworkSyncAsync((AttachmentAndVifs)item);
            }

            if (checkSyncResult.InSync)
            {
                ViewData["SuccessMessage"] = "The resource is synchronised with the network.";
            }
            else
            {
                if (checkSyncResult.NetworkSyncServiceResult.IsSuccess)
                {
                    ViewData["ErrorMessage"] = "The resource is not synchronised with the network. Press the 'Sync' button to update the network.";
                }
                else
                {
                    var message = checkSyncResult.NetworkSyncServiceResult.GetAllMessages();
                    ViewData["ErrorMessage"] = message;
                }
            }

            var returnItem = await AttachmentOrVifService.GetByIDAsync(attachmentOrVif.ID, attachmentOrVif.IsVif);
            return View("Details", Mapper.Map<AttachmentOrVifViewModel>(returnItem));
        }

        [HttpPost]
        public async Task<IActionResult> Sync(AttachmentOrVifViewModel attachmentOrVif)
        {

            NetworkSyncServiceResult syncResult;
            Object item;

            if (attachmentOrVif.IsVif)
            {
                item = await VifService.GetByIDAsync(attachmentOrVif.ID);
                if (item == null)
                {
                    return NotFound();
                }

                syncResult = await VifService.SyncToNetworkAsync((Vif)item);
            }
            else
            {
                item = await AttachmentService.GetByIDAsync(attachmentOrVif.ID);
                if (item == null)
                {
                    return NotFound();
                }

                syncResult = await AttachmentService.SyncToNetworkAsync((AttachmentAndVifs)item);
            }

            if (syncResult.IsSuccess)
            {
                ViewData["SuccessMessage"] = "The network is synchronised.";
            }
            else
            {
                ViewData["ErrorMessage"] = syncResult.GetMessage();
            }

            var returnItem = await AttachmentOrVifService.GetByIDAsync(attachmentOrVif.ID, attachmentOrVif.IsVif);

            return View("Details", Mapper.Map<AttachmentOrVifViewModel>(returnItem));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFromNetwork(AttachmentOrVifViewModel attachmentOrVif)
        {

            NetworkSyncServiceResult syncResult;
            Object item;

            if (attachmentOrVif.IsVif)
            {
                item = await VifService.GetByIDAsync(attachmentOrVif.ID);

                if (item == null)
                {
                    ViewData["VifDeletedMessage"] = "The vif has been deleted by another user. Return to the list.";

                    return View("VifDeleted", new { VpnID = Request.Query["VpnID"] });
                }

                syncResult = await VifService.DeleteFromNetworkAsync((Vif)item);
            }
            else
            {
                item = await AttachmentService.GetByIDAsync(attachmentOrVif.ID);

                if (item == null)
                {
                    ViewData["AttachmentDeletedMessage"] = "The attachment has been deleted by another user. Return to the list.";

                    return View("AttachmentDeleted", new { VpnID = Request.Query["VpnID"] });
                }

                syncResult = await AttachmentService.DeleteFromNetworkAsync((AttachmentAndVifs)item);
            }

            if (syncResult.IsSuccess)
            {
                ViewData["SuccessMessage"] = "The resource has been deleted from the network.";
            }
            else
            {
                var message = "There was a problem deleting the resource from the network. ";

                if (syncResult.NetworkHttpResponse != null)
                {
                    if (syncResult.NetworkHttpResponse.HttpStatusCode == HttpStatusCode.NotFound)
                    {
                        message += "The resource is not present in the network. ";
                    }
                }

                message += syncResult.GetHtmlListMessage();
                ViewData["ErrorMessage"] = message;
            }

            var returnItem = await AttachmentOrVifService.GetByIDAsync(attachmentOrVif.ID, attachmentOrVif.IsVif);

            return View("Delete", Mapper.Map<AttachmentOrVifViewModel>(returnItem));
        }
    }
}
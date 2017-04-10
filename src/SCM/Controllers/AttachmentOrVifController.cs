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
        public async Task<IActionResult> Details(int? id, bool? vif, bool? attachmentIsMultiPort)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await AttachmentOrVifService.GetByIDAsync(id.Value, vif, attachmentIsMultiPort);
            if (item == null)
            {
                return NotFound();
            }

            return View(Mapper.Map<AttachmentOrVifViewModel>(item));
        }

        [HttpPost]
        public async Task<IActionResult> CheckSync(AttachmentOrVifViewModel attachmentOrVif)
        {
            ServiceResult checkSyncResult;
            Object item;

            if (attachmentOrVif.IsVif)
            {
                item = await VifService.GetByIDAsync(attachmentOrVif.ID, attachmentOrVif.AttachmentIsMultiPort);
                if (item == null)
                {
                    return NotFound();
                }

                checkSyncResult = await VifService.CheckNetworkSyncAsync((Vif)item);
            }
            else
            {
                item = await AttachmentService.GetByIDAsync(attachmentOrVif.ID, attachmentOrVif.AttachmentIsMultiPort);
                if (item == null)
                {
                    return NotFound();
                }

                checkSyncResult = await AttachmentService.CheckNetworkSyncAsync((AttachmentAndVifs)item);
            }

            if (checkSyncResult.IsSuccess)
            {
                ViewData["SuccessMessage"] = "The resource is synchronised with the network.";
            }
            else
            {
                ViewData["ErrorMessage"] = checkSyncResult.GetHtmlListMessage();
            }

            var returnItem = await AttachmentOrVifService.GetByIDAsync(attachmentOrVif.ID, attachmentOrVif.IsVif, attachmentOrVif.AttachmentIsMultiPort);

            return View("Details", Mapper.Map<AttachmentOrVifViewModel>(returnItem));
        }

        [HttpPost]
        public async Task<IActionResult> Sync(AttachmentOrVifViewModel attachmentOrVif)
        {

            ServiceResult syncResult;
            Object item;

            if (attachmentOrVif.IsVif)
            {
                item = await VifService.GetByIDAsync(attachmentOrVif.ID, attachmentOrVif.AttachmentIsMultiPort);
                if (item == null)
                {
                    return NotFound();
                }

                syncResult = await VifService.SyncToNetworkAsync((Vif)item);
            }
            else
            {
                item = await AttachmentService.GetByIDAsync(attachmentOrVif.ID, attachmentOrVif.AttachmentIsMultiPort);
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
                ViewData["ErrorMessage"] = syncResult.GetHtmlListMessage();
            }

            var returnItem = await AttachmentOrVifService.GetByIDAsync(attachmentOrVif.ID, attachmentOrVif.IsVif, attachmentOrVif.AttachmentIsMultiPort);

            return View("Details", Mapper.Map<AttachmentOrVifViewModel>(returnItem));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFromNetwork(AttachmentOrVifViewModel attachmentOrVif)
        {

            ServiceResult syncResult;
            Object item;

            if (attachmentOrVif.IsVif)
            {
                item = await VifService.GetByIDAsync(attachmentOrVif.ID, attachmentOrVif.AttachmentIsMultiPort);

                if (item == null)
                {
                    ViewData["VifDeletedMessage"] = "The vif has been deleted by another user. Return to the list.";

                    return View("VifDeleted", new { VpnID = Request.Query["VpnID"] });
                }

                syncResult = await VifService.DeleteFromNetworkAsync((Vif)item);
            }
            else
            {
                item = await AttachmentService.GetByIDAsync(attachmentOrVif.ID, attachmentOrVif.AttachmentIsMultiPort);

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
                ViewData["ErrorMessage"] = syncResult.GetHtmlListMessage();
            }

            var returnItem = await AttachmentOrVifService.GetByIDAsync(attachmentOrVif.ID, attachmentOrVif.IsVif, attachmentOrVif.AttachmentIsMultiPort);

            return View("Delete", Mapper.Map<AttachmentOrVifViewModel>(returnItem));
        }
    }
}
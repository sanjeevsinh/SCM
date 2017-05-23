using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SCM.Models;
using SCM.Models.ViewModels;
using SCM.Services;
using SCM.Services.SCMServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SCM.Controllers
{
    public class VpnTenantCommunityController : BaseViewController
    {
        public VpnTenantCommunityController(IVpnTenantCommunityService vpnTenantCommunityService, 
            IVpnAttachmentSetService vpnAttachmentSetService, 
            ITenantCommunityService tenantCommunityService,
            IVpnService vpnService, 
            IMapper mapper)
        {
           VpnTenantCommunityService = vpnTenantCommunityService;
           VpnAttachmentSetService = vpnAttachmentSetService;
           TenantCommunityService = tenantCommunityService;
           VpnService = vpnService;
           Mapper = mapper;
        }
        private IVpnTenantCommunityService VpnTenantCommunityService { get; set; }
        private IVpnAttachmentSetService VpnAttachmentSetService { get; set; }
        private ITenantCommunityService TenantCommunityService { get; set; }
        private IVpnService VpnService { get; set; }
        private IMapper Mapper { get; set; }

        [HttpGet]
        public async Task<IActionResult> GetAllByVpnAttachmentSetID(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var vpnAttachmentSet = await VpnAttachmentSetService.GetByIDAsync(id.Value);

            if (vpnAttachmentSet == null)
            {
                return NotFound();
            }

            var vpnTenantCommunitys = await VpnTenantCommunityService.UnitOfWork.VpnTenantCommunityRepository.GetAsync(q => q.VpnAttachmentSetID == id,
                includeProperties: "TenantCommunity");
            ViewBag.VpnAttachmentSet = vpnAttachmentSet;

            return View(Mapper.Map<List<VpnTenantCommunityViewModel>>(vpnTenantCommunitys));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await VpnTenantCommunityService.GetByIDAsync(id.Value);
            if (item == null)
            {
                return NotFound();
            }

            return View(Mapper.Map<VpnTenantCommunityViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vpnAttachmentSet = await VpnAttachmentSetService.GetByIDAsync(id.Value);
            if (vpnAttachmentSet == null)
            {
                return NotFound();
            }

            ViewBag.VpnAttachmentSet = vpnAttachmentSet;
            await PopulateTenantCommunitysDropDownList(id.Value);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenantCommunityID,VpnAttachmentSetID")] VpnTenantCommunityViewModel vpnTenantCommunity)
        {
            var vpnAttachmentSet = await VpnAttachmentSetService.GetByIDAsync(vpnTenantCommunity.VpnAttachmentSetID);
            if (vpnAttachmentSet == null)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var mappedVpnTenantCommunity = Mapper.Map<VpnTenantCommunity>(vpnTenantCommunity);
                    var validationResult = await VpnTenantCommunityService.ValidateNewAsync(mappedVpnTenantCommunity, vpnAttachmentSet);

                    if (!validationResult.IsSuccess)
                    {
                        validationResult.GetMessageList().ForEach(message => ModelState.AddModelError(string.Empty, message));
                    }
                    else
                    {
                        await VpnService.UpdateRequiresSyncAsync(vpnAttachmentSet.VpnID, true, false);
                        await VpnTenantCommunityService.AddAsync(mappedVpnTenantCommunity);

                        return RedirectToAction("GetAllByVpnAttachmentSetID", new { id = vpnTenantCommunity.VpnAttachmentSetID });
                    }
                }
            }
            catch (DbUpdateException /** ex **/ )
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            ViewBag.VpnAttachmentSet = vpnAttachmentSet;
            await PopulateTenantCommunitysDropDownList(vpnTenantCommunity.VpnAttachmentSetID);

            return View(vpnTenantCommunity);
        }
  
        [HttpGet]
        public async Task<IActionResult> Delete(int? id, int? vpnAttachmentSetID, bool? concurrencyError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vpnTenantCommunity = await VpnTenantCommunityService.GetByIDAsync(id.Value);
            if (vpnTenantCommunity == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("GetAllByVpnAttachmentSetID", new { id = vpnAttachmentSetID });
                }

                return NotFound();
            }

            if (concurrencyError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] = "The record you attempted to delete "
                    + "was modified by another user after you got the original values. "
                    + "The delete operation was cancelled and the current values in the "
                    + "database have been displayed. If you still want to delete this "
                    + "record, click the Delete button again. Otherwise "
                    + "click the Back to List hyperlink.";
            }

            return View(Mapper.Map<VpnTenantCommunityViewModel>(vpnTenantCommunity));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(VpnTenantCommunityViewModel vpnTenantCommunity)
        {  
            try
            {
                var currentVpnTenantCommunity = await VpnTenantCommunityService.GetByIDAsync(vpnTenantCommunity.VpnTenantCommunityID);
                if (currentVpnTenantCommunity != null)
                {
                    await VpnService.UpdateRequiresSyncAsync(currentVpnTenantCommunity.VpnAttachmentSet.VpnID, true, false);
                    await VpnTenantCommunityService.DeleteAsync(currentVpnTenantCommunity);
                }

                return RedirectToAction("GetAllByVpnAttachmentSetID", new { id = vpnTenantCommunity.VpnAttachmentSetID });
            }

            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new
                {
                    concurrencyError = true,
                    id = vpnTenantCommunity.VpnTenantCommunityID,
                    vpnAttachmentSetID = vpnTenantCommunity.VpnAttachmentSetID
                });
            }
        }

        private async Task PopulateTenantCommunitysDropDownList(int vpnAttachmentSetID, object selectedTenantCommunity = null)
        {
            var tenantCommunities = await TenantCommunityService.GetAllByVpnAttachmentSetIDAsync(vpnAttachmentSetID);

            var result = tenantCommunities.Select(p => new
            {
                TenantCommunityID = p.TenantCommunityID,
                TenantCommunity = string.Concat(p.AutonomousSystemNumber, ":", p.Number)
            });

            ViewBag.TenantCommunityID = new SelectList(result, "TenantCommunityID", "TenantCommunity", selectedTenantCommunity);
        }
    }
}

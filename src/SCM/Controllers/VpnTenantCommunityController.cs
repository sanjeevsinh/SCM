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
        public VpnTenantCommunityController(IVpnTenantCommunityService vpnTenantCommunityService, IMapper mapper)
        {
           VpnTenantCommunityService = vpnTenantCommunityService;
           Mapper = mapper;
        }
        private IVpnTenantCommunityService VpnTenantCommunityService { get; set; }
        private IMapper Mapper { get; set; }

        [HttpGet]
        public async Task<IActionResult> GetAllByVpnAttachmentSetID(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vpnTenantCommunitys = await VpnTenantCommunityService.UnitOfWork.VpnTenantCommunityRepository.GetAsync(q => q.VpnAttachmentSetID == id,
                includeProperties:"TenantCommunity");
            var vpnAttachmentSet = await GetVpnAttachmentSetItem(id.Value);
            if (vpnAttachmentSet == null)
            {
                return NotFound();
            }

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

            var dbResult = await VpnTenantCommunityService.UnitOfWork.VpnTenantCommunityRepository.GetAsync(q => q.VpnTenantCommunityID == id, 
                includeProperties: "VpnAttachmentSet.Vpn,VpnAttachmentSet.AttachmentSet.Tenant,TenantCommunity");
            var item = dbResult.SingleOrDefault();

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

            var vpnAttachmentSet = await GetVpnAttachmentSetItem(id.Value);
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
            var vpnAttachmentSet = await GetVpnAttachmentSetItem(vpnTenantCommunity.VpnAttachmentSetID);
            if (vpnAttachmentSet == null)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var mappedVpnTenantCommunity = Mapper.Map<VpnTenantCommunity>(vpnTenantCommunity);
                    var validationResult = await VpnTenantCommunityService.ValidateVpnTenantCommunityAsync(mappedVpnTenantCommunity);

                    if (!validationResult.IsSuccess)
                    {
                        ModelState.AddModelError(string.Empty, validationResult.GetMessage());
                        ViewBag.VpnAttachmentSet = vpnAttachmentSet;
                        await PopulateTenantCommunitysDropDownList(vpnTenantCommunity.VpnAttachmentSetID);

                        return View(vpnTenantCommunity);
                    }

                    await VpnTenantCommunityService.AddAsync(mappedVpnTenantCommunity);
                    return RedirectToAction("GetAllByVpnAttachmentSetID", new { id = vpnTenantCommunity.VpnAttachmentSetID });
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
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbResult = await VpnTenantCommunityService.UnitOfWork.VpnTenantCommunityRepository.GetAsync(q => q.VpnTenantCommunityID == id.Value, 
                includeProperties:"VpnAttachmentSet.Vpn,TenantCommunity");
            var vpnTenantCommunity = dbResult.SingleOrDefault();

            if (vpnTenantCommunity == null)
            {
                return NotFound();
            }

            ViewBag.VpnAttachmentSet = await GetVpnAttachmentSetItem(vpnTenantCommunity.VpnAttachmentSetID);
            await PopulateTenantCommunitysDropDownList(vpnTenantCommunity.VpnAttachmentSetID);
            return View(Mapper.Map<VpnTenantCommunityViewModel>(vpnTenantCommunity));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("VpnTenantCommunityID,VpnAttachmentSetID,TenantCommunityID,RowVersion")] VpnTenantCommunityViewModel vpnTenantCommunity)
        {
            if (id != vpnTenantCommunity.VpnTenantCommunityID)
            {
                return NotFound();
            }

            var dbResult = await VpnTenantCommunityService.UnitOfWork.VpnTenantCommunityRepository.GetAsync(q => q.VpnTenantCommunityID == id, 
                includeProperties:"VpnAttachmentSet,TenantCommunity", AsTrackable: false);
            var currentVpnTenantCommunity = dbResult.SingleOrDefault();

            try
            {
                if (ModelState.IsValid)
                {
                    if (currentVpnTenantCommunity == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save changes. The item was deleted by another user.");

                        ViewBag.VpnAttachmentSet = await GetVpnAttachmentSetItem(vpnTenantCommunity.VpnAttachmentSetID);
                        await PopulateTenantCommunitysDropDownList(vpnTenantCommunity.VpnAttachmentSetID);
                        return View(vpnTenantCommunity);
                    }

                    await VpnTenantCommunityService.UpdateAsync(Mapper.Map<VpnTenantCommunity>(vpnTenantCommunity));
                    return RedirectToAction("GetAllByVpnAttachmentSetID", new { id = vpnTenantCommunity.VpnAttachmentSetID });
                }
            }

            catch (DbUpdateConcurrencyException ex)
            {
                var exceptionEntry = ex.Entries.Single();

                var proposedTenantCommunityID = (int)exceptionEntry.Property("TenantCommunityID").CurrentValue;
                if (currentVpnTenantCommunity.TenantCommunityID != proposedTenantCommunityID)
                {
                    ModelState.AddModelError("TenantCommunityID", $"Current value: {currentVpnTenantCommunity.TenantCommunity.AutonomousSystemNumber + ":" + currentVpnTenantCommunity.TenantCommunity.Number}");
                }

                ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                    + "was modified by another user after you got the original value. The "
                    + "edit operation was cancelled and the current values in the database "
                    + "have been displayed. If you still want to edit this record, click "
                    + "the Save button again. Otherwise click the Back to List hyperlink.");

                ModelState.Remove("RowVersion");
            }

            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            ViewBag.VpnAttachmentSet = await GetVpnAttachmentSetItem(currentVpnTenantCommunity.VpnAttachmentSetID);
            await PopulateTenantCommunitysDropDownList(currentVpnTenantCommunity.VpnAttachmentSetID);
            return View(Mapper.Map<VpnTenantCommunityViewModel>(currentVpnTenantCommunity));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbResult = await VpnTenantCommunityService.UnitOfWork.VpnTenantCommunityRepository.GetAsync(q => q.VpnTenantCommunityID == id.Value,
                includeProperties:"TenantCommunity");
            var vpnTenantCommunity = dbResult.SingleOrDefault();

            if (vpnTenantCommunity == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("GetAllByVpnAttachmentSetID", new { id = vpnTenantCommunity.VpnAttachmentSetID });
                }

                return NotFound();
            }

            if (concurrencyError.GetValueOrDefault())
            {
                ViewData["ConcurrencyErrorMessage"] = "The record you attempted to delete "
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
                var dbResult = await VpnTenantCommunityService.UnitOfWork.VpnTenantCommunityRepository.GetAsync(q => q.VpnTenantCommunityID == vpnTenantCommunity.VpnTenantCommunityID, AsTrackable:false);
                var currentVpnTenantCommunity = dbResult.SingleOrDefault();

                if (currentVpnTenantCommunity != null)
                {
                    await VpnTenantCommunityService.DeleteAsync(Mapper.Map<VpnTenantCommunity>(vpnTenantCommunity));
                }
                return RedirectToAction("GetAllByVpnAttachmentSetID", new { id = vpnTenantCommunity.VpnAttachmentSetID });
            }

            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { concurrencyError = true, id = vpnTenantCommunity.VpnTenantCommunityID });
            }
        }

        private async Task PopulateTenantCommunitysDropDownList(int vpnAttachmentSetID, object selectedTenantCommunity = null)
        {

            var dbResult1 = await VpnTenantCommunityService.UnitOfWork.VpnAttachmentSetRepository.GetAsync(q => q.VpnAttachmentSetID == vpnAttachmentSetID, 
                includeProperties:"AttachmentSet");
            var vpnAttachmentSet = dbResult1.SingleOrDefault();

            if (vpnAttachmentSet != null)
            {
                var dbResult2 = await VpnTenantCommunityService.UnitOfWork.TenantCommunityRepository.GetAsync(q => q.TenantID == vpnAttachmentSet.AttachmentSet.TenantID);
                var tenantCommunitys = dbResult2.Select(p => new { TenantCommunityID = p.TenantCommunityID, TenantCommunity = string.Concat(p.AutonomousSystemNumber, ":", p.Number) });

                ViewBag.TenantCommunityID = new SelectList(tenantCommunitys, "TenantCommunityID", "TenantCommunity", selectedTenantCommunity);
            }
        }
        private async Task<VpnAttachmentSet> GetVpnAttachmentSetItem(int vpnAttachmentSetID)
        {
            var dbResult = await VpnTenantCommunityService.UnitOfWork.VpnAttachmentSetRepository.GetAsync(q => q.VpnAttachmentSetID == vpnAttachmentSetID,
                includeProperties: "AttachmentSet.Tenant,Vpn");

           return dbResult.SingleOrDefault();
        }
    }
}

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

namespace SCM.Controllers
{
    public class TenantCommunityController : BaseViewController
    {
        public TenantCommunityController(ITenantCommunityService tenantNetworkService, IMapper mapper)
        {
           TenantCommunityService = tenantNetworkService;
           Mapper = mapper;
        }
        private ITenantCommunityService TenantCommunityService { get; set; }
        private IMapper Mapper { get; set; }

        [HttpGet]
        public async Task<IActionResult> GetAllByTenantID(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tenantNetworks = await TenantCommunityService.UnitOfWork.TenantCommunityRepository.GetAsync(q => q.TenantID == id);
            await PopulateTenantItem(id.Value);

            return View(Mapper.Map<List<TenantCommunityViewModel>>(tenantNetworks));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbResult = await TenantCommunityService.UnitOfWork.TenantCommunityRepository.GetAsync(q => q.TenantCommunityID == id.Value, includeProperties:"Tenant");
            var item = dbResult.SingleOrDefault();

            if (item == null)
            {
                return NotFound();
            }

            await PopulateTenantItem(item.TenantID);
            return View(Mapper.Map<TenantCommunityViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await PopulateTenantItem(id.Value);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenantID,AutonomousSystemNumber,Number,AllowExtranet")] TenantCommunityViewModel tenantNetwork)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await TenantCommunityService.AddAsync(Mapper.Map<TenantCommunity>(tenantNetwork));
                    return RedirectToAction("GetAllByTenantID", new { id = tenantNetwork.TenantID });
                }
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            await PopulateTenantItem(tenantNetwork.TenantID);
            return View(Mapper.Map<TenantCommunityViewModel>(tenantNetwork));
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TenantCommunity tenantNetwork = await TenantCommunityService.GetByIDAsync(id.Value);

            if (tenantNetwork == null)
            {
                return NotFound();
            }

            return View(Mapper.Map<TenantCommunityViewModel>(tenantNetwork));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("TenantCommunityID,TenantID,AutonomousSystemNumber,Number,AllowExtranet,RowVersion")] TenantCommunityViewModel tenantNetwork)
        {
            if (id != tenantNetwork.TenantCommunityID)
            {
                return NotFound();
            }

            var dbResult = await TenantCommunityService.UnitOfWork.TenantCommunityRepository.GetAsync(filter: d => d.TenantCommunityID == id,
                AsTrackable: false);
            var currentTenantCommunity = dbResult.SingleOrDefault();

            try
            {
                if (ModelState.IsValid)
                {
                    if (currentTenantCommunity == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save changes. The tenant community was deleted by another user.");
                        return View(tenantNetwork);
                    }

                    await TenantCommunityService.UpdateAsync(Mapper.Map<TenantCommunity>(tenantNetwork));
                    return RedirectToAction("GetAllByTenantID", new { id = tenantNetwork.TenantID });
                }
            }

            catch (DbUpdateConcurrencyException ex)
            {
                var exceptionEntry = ex.Entries.Single();

                var proposedAutonomousSystemNumber = (int)exceptionEntry.Property("AutonomousSystemNumber").CurrentValue;
                if (currentTenantCommunity.AutonomousSystemNumber != proposedAutonomousSystemNumber)
                {
                    ModelState.AddModelError("IpPrefix", $"Current value: {currentTenantCommunity.AutonomousSystemNumber}");
                }

                var proposedNumber = (int)exceptionEntry.Property("Number").CurrentValue;
                if (currentTenantCommunity.Number != proposedNumber)
                {
                    ModelState.AddModelError("Number", $"Current value: {currentTenantCommunity.Number}");
                }

                var proposedAllowExtranet = (bool)exceptionEntry.Property("AllowExtranet").CurrentValue;
                if (currentTenantCommunity.AllowExtranet != proposedAllowExtranet)
                {
                    ModelState.AddModelError("AllowExtranet", $"Current value: {currentTenantCommunity.AllowExtranet}");
                }

                ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                    + "was modified by another user after you got the original value. The "
                    + "edit operation was cancelled and the current values in the database "
                    + "have been displayed. If you still want to edit this record, click "
                    + "the Save button again. Otherwise click the Back to List hyperlink.");

                ModelState.Remove("RowVersion");
            }

            catch (DbUpdateException /* ex */ )
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");

            }

            return View(Mapper.Map<TenantCommunityViewModel>(currentTenantCommunity));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbResult = await TenantCommunityService.UnitOfWork.TenantCommunityRepository.GetAsync(q => q.TenantCommunityID == id.Value, includeProperties: "Tenant");
            var tenantNetwork = dbResult.SingleOrDefault();

            if (tenantNetwork == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("GetAllByTenantID", new { id = tenantNetwork.TenantID });
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

            return View(Mapper.Map<TenantCommunityViewModel>(tenantNetwork));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(TenantCommunityViewModel tenantNetwork)
        {
            try
            {
                var dbResult = await TenantCommunityService.UnitOfWork.TenantCommunityRepository.GetAsync(filter: d => d.TenantCommunityID == tenantNetwork.TenantCommunityID, AsTrackable: false);
                var currentTenantCommunity = dbResult.SingleOrDefault();

                if (currentTenantCommunity != null)
                {
                    await TenantCommunityService.DeleteAsync(Mapper.Map<TenantCommunity>(tenantNetwork));
                }
                return RedirectToAction("GetAllByTenantID", new { id = tenantNetwork.TenantID });
            }

            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { concurrencyError = true, id = tenantNetwork.TenantCommunityID });
            }
        }

        private async Task PopulateTenantItem(int tenantID)
        {
            ViewBag.Tenant = await TenantCommunityService.UnitOfWork.TenantRepository.GetByIDAsync(tenantID);
        }
    }
}

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
        public TenantCommunityController(ITenantCommunityService tenantCommunityService, IVpnService vpnService, IMapper mapper)
        {
           TenantCommunityService = tenantCommunityService;
           VpnService = vpnService;
           Mapper = mapper;
        }
        private ITenantCommunityService TenantCommunityService { get; set; }
        private IVpnService VpnService { get; set; }
        private IMapper Mapper { get; set; }

        [HttpGet]
        public async Task<IActionResult> GetAllByTenantID(int? id, string warningMessage = "")
        {
            if (id == null)
            {
                return NotFound();
            }

            var tenantCommunities = await TenantCommunityService.UnitOfWork.TenantCommunityRepository.GetAsync(q => q.TenantID == id);

            if (warningMessage.Length > 0)
            {
                ViewData["WarningMessage"] = warningMessage;
            }

            ViewBag.Tenant = await TenantCommunityService.UnitOfWork.TenantRepository.GetByIDAsync(id.Value);

            return View(Mapper.Map<List<TenantCommunityViewModel>>(tenantCommunities));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await TenantCommunityService.GetByIDAsync(id.Value);

            if (item == null)
            {
                return NotFound();
            }

            ViewBag.Tenant = await TenantCommunityService.UnitOfWork.TenantRepository.GetByIDAsync(item.TenantID);

            return View(Mapper.Map<TenantCommunityViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.Tenant = await TenantCommunityService.UnitOfWork.TenantRepository.GetByIDAsync(id.Value);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenantID,AutonomousSystemNumber,Number,AllowExtranet")] TenantCommunityViewModel tenantCommunity)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await TenantCommunityService.AddAsync(Mapper.Map<TenantCommunity>(tenantCommunity));
                    return RedirectToAction("GetAllByTenantID", new { id = tenantCommunity.TenantID });
                }
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            ViewBag.Tenant = await TenantCommunityService.UnitOfWork.TenantRepository.GetByIDAsync(tenantCommunity.TenantID);

            return View(Mapper.Map<TenantCommunityViewModel>(tenantCommunity));
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tenantCommunity = await TenantCommunityService.GetByIDAsync(id.Value);

            if (tenantCommunity == null)
            {
                return NotFound();
            }

            ViewBag.Tenant = await TenantCommunityService.UnitOfWork.TenantRepository.GetByIDAsync(id.Value);

            return View(Mapper.Map<TenantCommunityViewModel>(tenantCommunity));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("TenantCommunityID,TenantID,AutonomousSystemNumber,Number,AllowExtranet,RowVersion")] TenantCommunityViewModel tenantCommunity)
        {
            if (id != tenantCommunity.TenantCommunityID)
            {
                return NotFound();
            }

            var currentTenantCommunity = await TenantCommunityService.GetByIDAsync(id);

            try
            {
                if (ModelState.IsValid)
                {
                    if (currentTenantCommunity == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save changes. The tenant community was deleted by another user.");
                        return View(tenantCommunity);
                    }

                    await TenantCommunityService.UpdateAsync(Mapper.Map<TenantCommunity>(tenantCommunity));

                    // Update the requiresSync state for all VPNs which the tenant community is associated with 

                    var vpns = await VpnService.GetAllByTenantCommunityIDAsync(tenantCommunity.TenantCommunityID);
                    var warningMessage = string.Empty;

                    if (vpns.Count() > 0)
                    {

                        await VpnService.UpdateRequiresSyncAsync(vpns, true, true);
                        warningMessage = $"VPNs require synchronisation with the network as a result of this update. "
                            + "Follow this <a href = '/Vpn/GetAll'>link</a> to the VPNs page.";
                    }

                    return RedirectToAction("GetAllByTenantID", new { id = tenantCommunity.TenantID, warningMessage = warningMessage });
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

            ViewBag.Tenant = await TenantCommunityService.UnitOfWork.TenantRepository.GetByIDAsync(currentTenantCommunity.TenantID);

            return View(Mapper.Map<TenantCommunityViewModel>(currentTenantCommunity));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tenantCommunity = await TenantCommunityService.GetByIDAsync(id.Value);

            if (tenantCommunity == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("GetAllByTenantID", new { id = tenantCommunity.TenantID });
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

            ViewBag.Tenant = await TenantCommunityService.UnitOfWork.TenantRepository.GetByIDAsync(id.Value);

            return View(Mapper.Map<TenantCommunityViewModel>(tenantCommunity));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(TenantCommunityViewModel tenantCommunity)
        {
            try
            {
                var currentTenantCommunity = await TenantCommunityService.GetByIDAsync(tenantCommunity.TenantCommunityID);

                if (currentTenantCommunity != null)
                {
                    await TenantCommunityService.DeleteAsync(Mapper.Map<TenantCommunity>(tenantCommunity));
                }
                return RedirectToAction("GetAllByTenantID", new { id = tenantCommunity.TenantID });
            }

            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { concurrencyError = true, id = tenantCommunity.TenantCommunityID });
            }
        }
    }
}

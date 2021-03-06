﻿using System;
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
    public class TenantNetworkController : BaseViewController
    {
        public TenantNetworkController(ITenantNetworkService tenantNetworkService, IVpnService vpnService, IMapper mapper)
        {
           TenantNetworkService = tenantNetworkService;
           VpnService = vpnService;
           Mapper = mapper;
        }
        private ITenantNetworkService TenantNetworkService { get; set; }
        private IVpnService VpnService { get; set; }
        private IMapper Mapper { get; set; }

        [HttpGet]
        public async Task<IActionResult> GetAllByTenantID(int? id, string warningMessage = "")
        {
            if (id == null)
            {
                return NotFound();
            }

            var tenantNetworks = await TenantNetworkService.UnitOfWork.TenantNetworkRepository.GetAsync(q => q.TenantID == id);

            if (warningMessage.Length > 0)
            {
                ViewData["WarningMessage"] = warningMessage;
            }

            ViewBag.Tenant = await TenantNetworkService.UnitOfWork.TenantRepository.GetByIDAsync(id.Value);

            return View(Mapper.Map<List<TenantNetworkViewModel>>(tenantNetworks));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await TenantNetworkService.GetByIDAsync(id.Value);

            if (item == null)
            {
                return NotFound();
            }

            ViewBag.Tenant = await TenantNetworkService.UnitOfWork.TenantRepository.GetByIDAsync(item.TenantID);

            return View(Mapper.Map<TenantNetworkViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.Tenant = await TenantNetworkService.UnitOfWork.TenantRepository.GetByIDAsync(id.Value);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenantID,IpPrefix,Length,AllowExtranet")] TenantNetworkViewModel tenantNetwork)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await TenantNetworkService.AddAsync(Mapper.Map<TenantNetwork>(tenantNetwork));
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

            ViewBag.Tenant = await TenantNetworkService.UnitOfWork.TenantRepository.GetByIDAsync(tenantNetwork.TenantID);

            return View(Mapper.Map<TenantNetworkViewModel>(tenantNetwork));
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tenantNetwork = await TenantNetworkService.GetByIDAsync(id.Value);

            if (tenantNetwork == null)
            {
                return NotFound();
            }

            ViewBag.Tenant = await TenantNetworkService.UnitOfWork.TenantRepository.GetByIDAsync(id.Value);

            return View(Mapper.Map<TenantNetworkViewModel>(tenantNetwork));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("TenantNetworkID,TenantID,IpPrefix,Length,AllowExtranet,RowVersion")] TenantNetworkViewModel tenantNetwork)
        {
            if (id != tenantNetwork.TenantNetworkID)
            {
                return NotFound();
            }

            var currentTenantNetwork = await TenantNetworkService.GetByIDAsync(id);

            try
            {
                if (ModelState.IsValid)
                {
                    if (currentTenantNetwork == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save changes. The tenant network was deleted by another user.");
                        return View(tenantNetwork);
                    }

                    await TenantNetworkService.UpdateAsync(Mapper.Map<TenantNetwork>(tenantNetwork));

                    // Update the requiresSync state for all VPNs which the tenant network is associated with 

                    var vpns = await VpnService.GetAllByTenantNetworkIDAsync(tenantNetwork.TenantNetworkID);

                    var warningMessage = string.Empty;
                    if (vpns.Count() > 0) {

                        await VpnService.UpdateRequiresSyncAsync(vpns, true, true);
                        warningMessage = $"VPNs require synchronisation with the network as a result of this update. "
                            + "Follow this <a href = '/Vpn/GetAll'>link</a> to the VPNs page.";
                    }

                    return RedirectToAction("GetAllByTenantID", new { id = tenantNetwork.TenantID, warningMessage = warningMessage });
                }
            }

            catch (DbUpdateConcurrencyException ex)
            {
                var exceptionEntry = ex.Entries.Single();

                var proposedIpPrefix = (string)exceptionEntry.Property("IpPrefix").CurrentValue;
                if (currentTenantNetwork.IpPrefix != proposedIpPrefix)
                {
                    ModelState.AddModelError("IpPrefix", $"Current value: {currentTenantNetwork.IpPrefix}");
                }

                var proposedLength = (int)exceptionEntry.Property("Length").CurrentValue;
                if (currentTenantNetwork.Length != proposedLength)
                {
                    ModelState.AddModelError("Length", $"Current value: {currentTenantNetwork.Length}");
                }

                var proposedAllowExtranet = (bool)exceptionEntry.Property("AllowExtranet").CurrentValue;
                if (currentTenantNetwork.AllowExtranet != proposedAllowExtranet)
                {
                    ModelState.AddModelError("AllowExtranet", $"Current value: {currentTenantNetwork.AllowExtranet}");
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

            ViewBag.Tenant = await TenantNetworkService.UnitOfWork.TenantRepository.GetByIDAsync(currentTenantNetwork.TenantID);

            return View(Mapper.Map<TenantNetworkViewModel>(currentTenantNetwork));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tenantNetwork = await TenantNetworkService.GetByIDAsync(id.Value);

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

            ViewBag.Tenant = await TenantNetworkService.UnitOfWork.TenantRepository.GetByIDAsync(id.Value);

            return View(Mapper.Map<TenantNetworkViewModel>(tenantNetwork));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(TenantNetworkViewModel tenantNetwork)
        {
            try
            {
                var currentTenantNetwork = await TenantNetworkService.GetByIDAsync(tenantNetwork.TenantNetworkID);

                if (currentTenantNetwork != null)
                {
                    await TenantNetworkService.DeleteAsync(Mapper.Map<TenantNetwork>(tenantNetwork));
                }
                return RedirectToAction("GetAllByTenantID", new { id = tenantNetwork.TenantID });
            }

            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { concurrencyError = true, id = tenantNetwork.TenantNetworkID });
            }
        }
    }
}

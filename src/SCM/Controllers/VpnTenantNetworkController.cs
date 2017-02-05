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
    public class VpnTenantNetworkController : BaseViewController
    {
        public VpnTenantNetworkController(IVpnTenantNetworkService vpnTenantNetworkService, IMapper mapper)
        {
           VpnTenantNetworkService = vpnTenantNetworkService;
           Mapper = mapper;
        }
        private IVpnTenantNetworkService VpnTenantNetworkService { get; set; }
        private IMapper Mapper { get; set; }

        [HttpGet]
        public async Task<IActionResult> GetAllByVpnAttachmentSetID(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vpnTenantNetworks = await VpnTenantNetworkService.UnitOfWork.VpnTenantNetworkRepository.GetAsync(q => q.VpnAttachmentSetID == id,
                includeProperties:"TenantNetwork");
            var vpnAttachmentSet = await GetVpnAttachmentSetItem(id.Value);
            if (vpnAttachmentSet == null)
            {
                return NotFound();
            }

            ViewBag.VpnAttachmentSet = vpnAttachmentSet;
            return View(Mapper.Map<List<VpnTenantNetworkViewModel>>(vpnTenantNetworks));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbResult = await VpnTenantNetworkService.UnitOfWork.VpnTenantNetworkRepository.GetAsync(q => q.VpnTenantNetworkID == id, 
                includeProperties: "VpnAttachmentSet.Vpn,VpnAttachmentSet.AttachmentSet.Tenant,TenantNetwork");
            var item = dbResult.SingleOrDefault();

            if (item == null)
            {
                return NotFound();
            }
            return View(Mapper.Map<VpnTenantNetworkViewModel>(item));
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
            await PopulateTenantNetworksDropDownList(id.Value);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenantNetworkID,VpnAttachmentSetID")] VpnTenantNetworkViewModel vpnTenantNetwork)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await VpnTenantNetworkService.AddAsync(Mapper.Map<VpnTenantNetwork>(vpnTenantNetwork));
                    return RedirectToAction("GetAllByVpnAttachmentSetID", new { id = vpnTenantNetwork.VpnAttachmentSetID });
                }
            }
            catch (DbUpdateException /** ex **/ )
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            var vpnAttachmentSet = await GetVpnAttachmentSetItem(vpnTenantNetwork.VpnAttachmentSetID);
            if (vpnAttachmentSet == null)
            {
                return NotFound();
            }

            ViewBag.VpnAttachmentSet = vpnAttachmentSet;
            await PopulateTenantNetworksDropDownList(vpnTenantNetwork.VpnAttachmentSetID);
            return View(vpnTenantNetwork);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbResult = await VpnTenantNetworkService.UnitOfWork.VpnTenantNetworkRepository.GetAsync(q => q.VpnTenantNetworkID == id.Value, 
                includeProperties:"VpnAttachmentSet.Vpn,TenantNetwork");
            var vpnTenantNetwork = dbResult.SingleOrDefault();

            if (vpnTenantNetwork == null)
            {
                return NotFound();
            }

            ViewBag.VpnAttachmentSet = await GetVpnAttachmentSetItem(vpnTenantNetwork.VpnAttachmentSetID);
            await PopulateTenantNetworksDropDownList(vpnTenantNetwork.VpnAttachmentSetID);
            return View(Mapper.Map<VpnTenantNetworkViewModel>(vpnTenantNetwork));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("VpnTenantNetworkID,VpnAttachmentSetID,TenantNetworkID,RowVersion")] VpnTenantNetworkViewModel vpnTenantNetwork)
        {
            if (id != vpnTenantNetwork.VpnTenantNetworkID)
            {
                return NotFound();
            }

            var dbResult = await VpnTenantNetworkService.UnitOfWork.VpnTenantNetworkRepository.GetAsync(q => q.VpnTenantNetworkID == id, 
                includeProperties:"VpnAttachmentSet,TenantNetwork", AsTrackable: false);
            var currentVpnTenantNetwork = dbResult.SingleOrDefault();

            try
            {
                if (ModelState.IsValid)
                {
                    if (currentVpnTenantNetwork == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save changes. The item was deleted by another user.");

                        ViewBag.VpnAttachmentSet = await GetVpnAttachmentSetItem(vpnTenantNetwork.VpnAttachmentSetID);
                        await PopulateTenantNetworksDropDownList(vpnTenantNetwork.VpnAttachmentSetID);
                        return View(vpnTenantNetwork);
                    }

                    await VpnTenantNetworkService.UpdateAsync(Mapper.Map<VpnTenantNetwork>(vpnTenantNetwork));
                    return RedirectToAction("GetAllByVpnAttachmentSetID", new { id = vpnTenantNetwork.VpnAttachmentSetID });
                }
            }

            catch (DbUpdateConcurrencyException ex)
            {
                var exceptionEntry = ex.Entries.Single();

                var proposedTenantNetworkID = (int)exceptionEntry.Property("TenantNetworkID").CurrentValue;
                if (currentVpnTenantNetwork.TenantNetworkID != proposedTenantNetworkID)
                {
                    ModelState.AddModelError("TenantNetworkID", $"Current value: {currentVpnTenantNetwork.TenantNetwork.IpPrefix + "/" + currentVpnTenantNetwork.TenantNetwork.Length}");
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

            ViewBag.VpnAttachmentSet = await GetVpnAttachmentSetItem(currentVpnTenantNetwork.VpnAttachmentSetID);
            await PopulateTenantNetworksDropDownList(currentVpnTenantNetwork.VpnAttachmentSetID);
            return View(Mapper.Map<VpnTenantNetworkViewModel>(currentVpnTenantNetwork));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbResult = await VpnTenantNetworkService.UnitOfWork.VpnTenantNetworkRepository.GetAsync(q => q.VpnTenantNetworkID == id.Value,
                includeProperties:"TenantNetwork");
            var vpnTenantNetwork = dbResult.SingleOrDefault();

            if (vpnTenantNetwork == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("GetAllByVpnAttachmentSetID", new { id = vpnTenantNetwork.VpnAttachmentSetID });
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

            return View(Mapper.Map<VpnTenantNetworkViewModel>(vpnTenantNetwork));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(VpnTenantNetworkViewModel vpnTenantNetwork)
        {  
            try
            {
                var dbResult = await VpnTenantNetworkService.UnitOfWork.VpnTenantNetworkRepository.GetAsync(q => q.VpnTenantNetworkID == vpnTenantNetwork.VpnTenantNetworkID, AsTrackable:false);
                var currentVpnTenantNetwork = dbResult.SingleOrDefault();

                if (currentVpnTenantNetwork != null)
                {
                    await VpnTenantNetworkService.DeleteAsync(Mapper.Map<VpnTenantNetwork>(vpnTenantNetwork));
                }
                return RedirectToAction("GetAllByVpnAttachmentSetID", new { id = vpnTenantNetwork.VpnAttachmentSetID });
            }

            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { concurrencyError = true, id = vpnTenantNetwork.VpnTenantNetworkID });
            }
        }

        private async Task PopulateTenantNetworksDropDownList(int vpnAttachmentSetID, object selectedTenantNetwork = null)
        {

            var dbResult1 = await VpnTenantNetworkService.UnitOfWork.VpnAttachmentSetRepository.GetAsync(q => q.VpnAttachmentSetID == vpnAttachmentSetID, 
                includeProperties:"AttachmentSet");
            var vpnAttachmentSet = dbResult1.SingleOrDefault();

            if (vpnAttachmentSet != null)
            {
                var dbResult2 = await VpnTenantNetworkService.UnitOfWork.TenantNetworkRepository.GetAsync(q => q.TenantID == vpnAttachmentSet.AttachmentSet.TenantID);
                var tenantNetworks = dbResult2.Select(p => new { TenantNetworkID = p.TenantNetworkID, TenantNetwork = string.Concat(p.IpPrefix, "/", p.Length) });

                ViewBag.TenantNetworkID = new SelectList(tenantNetworks, "TenantNetworkID", "TenantNetwork", selectedTenantNetwork);
            }
        }
        private async Task<VpnAttachmentSet> GetVpnAttachmentSetItem(int vpnAttachmentSetID)
        {
            var dbResult = await VpnTenantNetworkService.UnitOfWork.VpnAttachmentSetRepository.GetAsync(q => q.VpnAttachmentSetID == vpnAttachmentSetID,
                includeProperties: "AttachmentSet.Tenant,Vpn");

           return dbResult.SingleOrDefault();
        }
    }
}

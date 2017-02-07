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
    public class VpnAttachmentSetController : BaseViewController
    {
        public VpnAttachmentSetController(IVpnAttachmentSetService vpnAttachmentSetService, IMapper mapper)
        {
           VpnAttachmentSetService = vpnAttachmentSetService;
           Mapper = mapper;
        }
        private IVpnAttachmentSetService VpnAttachmentSetService { get; set; }
        private IMapper Mapper { get; set; }

        [HttpGet]
        public async Task<IActionResult> GetAllByVpnID(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vpn = await GetVpn(id.Value);
            ViewBag.Vpn = vpn;

            var vpnAttachmentSets = await VpnAttachmentSetService.UnitOfWork.VpnAttachmentSetRepository.GetAsync(q => q.VpnID == id.Value,
                includeProperties:"AttachmentSet.Tenant");

            return View(Mapper.Map<List<VpnAttachmentSetViewModel>>(vpnAttachmentSets));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbResult = await VpnAttachmentSetService.UnitOfWork.VpnAttachmentSetRepository.GetAsync(q => q.VpnAttachmentSetID == id, 
                includeProperties: "AttachmentSet.Tenant,AttachmentSet.ContractBandwidth,Vpn");
            var item = dbResult.SingleOrDefault();

            if (item == null)
            {
                return NotFound();
            }

            ViewBag.Vpn = await GetVpn(item.VpnID);
            return View(Mapper.Map<VpnAttachmentSetViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> CreateStep1(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await PopulateTenantsDropDownList(id.Value);
            var vpn = await GetVpn(id.Value);
            ViewBag.Vpn = vpn;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStep2([Bind("TenantID,VpnID")] VpnAttachmentSetSelectionViewModel vpnAttachmentSetSelection)
        {
            await PopulateAttachmentSetsDropDownList(vpnAttachmentSetSelection);
            ViewBag.Vpn = await GetVpn(vpnAttachmentSetSelection.VpnID);
            ViewBag.VpnAttachmentSetSelection = vpnAttachmentSetSelection;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AttachmentSetID,VpnID")] VpnAttachmentSetViewModel vpnAttachmentSet,
            [Bind("VpnID,TenantID")] VpnAttachmentSetSelectionViewModel vpnAttachmentSetSelection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var mappedAttachmentSet = Mapper.Map<VpnAttachmentSet>(vpnAttachmentSet);
                    var validationResult = await VpnAttachmentSetService.ValidateVpnAttachmentSetAsync(mappedAttachmentSet);

                    if (!validationResult.IsValid)
                    {
                        ModelState.AddModelError(string.Empty, validationResult.GetMessage());
                        ViewBag.Vpn = await GetVpn(vpnAttachmentSet.VpnID);
                        ViewBag.VpnAttachmentSetSelection = vpnAttachmentSetSelection;
                        await PopulateAttachmentSetsDropDownList(vpnAttachmentSetSelection);

                        return View("CreateStep2", vpnAttachmentSet);
                    }

                    await VpnAttachmentSetService.AddAsync(Mapper.Map<VpnAttachmentSet>(mappedAttachmentSet));

                    return RedirectToAction("GetAllByVpnID", new { id = vpnAttachmentSet.VpnID });
                }
            }
            catch (DbUpdateException /** ex **/ )
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            ViewBag.Vpn = await GetVpn(vpnAttachmentSet.VpnID);
            ViewBag.VpnAttachmentSetSelection = vpnAttachmentSetSelection;
            await PopulateAttachmentSetsDropDownList(vpnAttachmentSetSelection);

            return View("CreateStep2", vpnAttachmentSet);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbResult = await VpnAttachmentSetService.UnitOfWork.VpnAttachmentSetRepository.GetAsync(q => q.VpnAttachmentSetID == id.Value,includeProperties:"AttachmentSet");
            var vpnAttachmentSet = dbResult.SingleOrDefault();

            if (vpnAttachmentSet == null)
            {
                return NotFound();
            }

            await PopulateAttachmentSetsDropDownList(new VpnAttachmentSetSelectionViewModel
            {
                VpnID = vpnAttachmentSet.VpnID,
                TenantID = vpnAttachmentSet.AttachmentSet.TenantID
            });

            ViewBag.Vpn = await GetVpn(vpnAttachmentSet.VpnID);
            return View(Mapper.Map<VpnAttachmentSetViewModel>(vpnAttachmentSet));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("VpnAttachmentSetID,AttachmentSetID,VpnID,RowVersion")] VpnAttachmentSetViewModel vpnAttachmentSet)
        {
            if (id != vpnAttachmentSet.VpnAttachmentSetID)
            {
                return NotFound();
            }

            var dbResult = await VpnAttachmentSetService.UnitOfWork.VpnAttachmentSetRepository.GetAsync(q => q.VpnAttachmentSetID == id, 
                includeProperties:"AttachmentSet,Vpn", AsTrackable: false);
            var currentAttachmentSetVpn = dbResult.SingleOrDefault();

            try
            {
                if (ModelState.IsValid)
                {
                    if (currentAttachmentSetVpn == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save changes. The item was deleted by another user.");
                        return View(vpnAttachmentSet);
                    }

                    await VpnAttachmentSetService.UpdateAsync(Mapper.Map<VpnAttachmentSet>(vpnAttachmentSet));
                    return RedirectToAction("GetAllByVpnID", new { id = vpnAttachmentSet.VpnID });
                }
            }

            catch (DbUpdateConcurrencyException ex)
            {
                var exceptionEntry = ex.Entries.Single();

                var proposedAttachmentSetID = (int)exceptionEntry.Property("AttachmentSetID").CurrentValue;
                if (currentAttachmentSetVpn.AttachmentSetID!= proposedAttachmentSetID)
                {
                    ModelState.AddModelError("AttachmentSetID", $"Current value: {currentAttachmentSetVpn.AttachmentSet.Name}");
                }

                var proposedVpnID = (int)exceptionEntry.Property("VpnID").CurrentValue;
                if (currentAttachmentSetVpn.VpnID != proposedVpnID)
                {
                    ModelState.AddModelError("VpnID", $"Current value: {currentAttachmentSetVpn.Vpn.Name}");
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

            await PopulateAttachmentSetsDropDownList(new VpnAttachmentSetSelectionViewModel
            {
                TenantID = currentAttachmentSetVpn.AttachmentSet.TenantID,
                VpnID = currentAttachmentSetVpn.VpnID
            });

            ViewBag.Vpn = await GetVpn(currentAttachmentSetVpn.VpnID);
            return View(Mapper.Map<VpnAttachmentSetViewModel>(currentAttachmentSetVpn));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbResult = await VpnAttachmentSetService.UnitOfWork.VpnAttachmentSetRepository.GetAsync(q => q.VpnAttachmentSetID == id.Value, includeProperties:"AttachmentSet");
            var vpnAttachmentSet = dbResult.SingleOrDefault();
            if (vpnAttachmentSet == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("GetAllByVpnID", new { id = vpnAttachmentSet.VpnID });
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

            ViewBag.Vpn = await GetVpn(vpnAttachmentSet.VpnID);
            return View(Mapper.Map<VpnAttachmentSetViewModel>(vpnAttachmentSet));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(VpnAttachmentSetViewModel vpnAttachmentSet)
        {  
            try
            {
                var dbResult = await VpnAttachmentSetService.UnitOfWork.VpnAttachmentSetRepository.GetAsync(q => q.VpnAttachmentSetID == vpnAttachmentSet.VpnAttachmentSetID, 
                    AsTrackable:false);
                var currentAttachmentSetVpn = dbResult.SingleOrDefault();

                if (currentAttachmentSetVpn != null)
                {
                    await VpnAttachmentSetService.DeleteAsync(Mapper.Map<VpnAttachmentSet>(vpnAttachmentSet));
                }

                
                return RedirectToAction("GetAllByVpnID", new { id = vpnAttachmentSet.VpnID });
            }

            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { concurrencyError = true, id = vpnAttachmentSet.VpnAttachmentSetID });
            }
        }

        private async Task PopulateAttachmentSetsDropDownList(VpnAttachmentSetSelectionViewModel vpnAttachmentSetSelection, object selectedAttachmentSet = null)
        {

            IEnumerable<AttachmentSet> attachmentSets  = await VpnAttachmentSetService.UnitOfWork.AttachmentSetRepository.GetAsync(q => 
                        q.TenantID == vpnAttachmentSetSelection.TenantID);

            ViewBag.AttachmentSetID = new SelectList(attachmentSets, "AttachmentSetID", "Name", selectedAttachmentSet);
        }

        private async Task PopulateTenantsDropDownList(int vpnID, object selectedTenant = null)
        {
            var dbResult = await VpnAttachmentSetService.UnitOfWork.VpnRepository.GetAsync(q => q.VpnID == vpnID, 
                includeProperties: "VpnTenancyType");

            var vpn = dbResult.Single();
            IEnumerable<Tenant> tenants;

            // Get all Tenants if the VPN is multi-tenant, other get only the tenant owner
            if (vpn.VpnTenancyType.TenancyType == "Multi")
            {
                tenants = await VpnAttachmentSetService.UnitOfWork.TenantRepository.GetAsync();
            }
            else
            {
                tenants = await VpnAttachmentSetService.UnitOfWork.TenantRepository.GetAsync(q => q.TenantID == vpn.TenantID);
            }

            ViewBag.TenantID = new SelectList(tenants, "TenantID", "Name", selectedTenant);
        }

        private async Task<Vpn> GetVpn(int vpnID)
        {
            var dbResult = await VpnAttachmentSetService.UnitOfWork.VpnRepository.GetAsync(q => q.VpnID == vpnID);

            return dbResult.SingleOrDefault();
           
        }
    }
}

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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SCM.Controllers
{
    public class AttachmentSetController : BaseViewController
    {
        public AttachmentSetController(IAttachmentSetService attachmentSetService, IMapper mapper)
        {
           AttachmentSetService = attachmentSetService;
           Mapper = mapper;
        }
        private IAttachmentSetService AttachmentSetService { get; set; }
        private IMapper Mapper { get; set; }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var attachmentSets = await AttachmentSetService.GetAllAsync();
            return View(Mapper.Map<List<AttachmentSetViewModel>>(attachmentSets));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbResult = await AttachmentSetService.UnitOfWork.AttachmentSetRepository.GetAsync(q => q.AttachmentSetID == id, includeProperties: "Tenant,SubRegion.Region,ContractBandwidth,AttachmentRedundancy");
            var item = dbResult.SingleOrDefault();

            if (item == null)
            {
                return NotFound();
            }
            return View(Mapper.Map<AttachmentSetViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> CreateStep1()
        {
            await PopulateRegionsDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStep2([Bind("RegionID,Name")] RegionViewModel region)
        {
            await PopulateTenantsDropDownList();
            await PopulateSubRegionsDropDownList(region.RegionID);
            await PopulateContractBandwidthsDropDownList();
            await PopulateAttachmentRedundancyDropDownList();
            ViewBag.Region = region;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,RegionID,SubRegionID,TenantID,ContractBandwidthID,AttachmentRedundancyID")] AttachmentSetViewModel attachmentSet)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await AttachmentSetService.AddAsync(Mapper.Map<AttachmentSet>(attachmentSet));
                    return RedirectToAction("GetAll");
                }
            }
            catch (DbUpdateException /** ex **/ )
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            await PopulateTenantsDropDownList();
            await PopulateSubRegionsDropDownList(attachmentSet.SubRegion.RegionID);
            await PopulateContractBandwidthsDropDownList();
            await PopulateAttachmentRedundancyDropDownList();
            return View(attachmentSet);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbResult = await AttachmentSetService.UnitOfWork.AttachmentSetRepository.GetAsync(q => q.AttachmentSetID == id.Value, includeProperties:"Tenant,SubRegion,Region,ContractBandwidth,AttachmentRedundancy");
            var attachmentSet = dbResult.SingleOrDefault();

            if (attachmentSet == null)
            {
                return NotFound();
            }

            await PopulateTenantsDropDownList();
            await PopulateSubRegionsDropDownList(attachmentSet.SubRegion.RegionID);
            await PopulateContractBandwidthsDropDownList();
            await PopulateAttachmentRedundancyDropDownList();

            return View(Mapper.Map<AttachmentSetViewModel>(attachmentSet));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("AttachmentSetID,Name,Description,RegionID,SubRegionID,TenantID,ContractBandwidthID,AttachmentRedundancyID,RowVersion")] AttachmentSetViewModel attachmentSet)
        {
            if (id != attachmentSet.AttachmentSetID)
            {
                return NotFound();
            }

            var dbResult = await AttachmentSetService.UnitOfWork.AttachmentSetRepository.GetAsync(q => q.AttachmentSetID == id, 
                includeProperties:"SubRegion,Region,Tenant,ContractBandwidth,AttachmentRedundancy", AsTrackable: false);
            var currentAttachmentSet = dbResult.SingleOrDefault();

            try
            {
                if (ModelState.IsValid)
                {
                    if (currentAttachmentSet == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save changes. The attachment set was deleted by another user.");
                        return View(attachmentSet);
                    }

                    if (currentAttachmentSet.RegionID != attachmentSet.RegionID)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save changes. The Region cannot be changed.");
                        return View(attachmentSet);
                    }

                    await AttachmentSetService.UpdateAsync(Mapper.Map<AttachmentSet>(attachmentSet));
                    return RedirectToAction("GetAll");
                }
            }

            catch (DbUpdateConcurrencyException ex)
            {
                var exceptionEntry = ex.Entries.Single();

                var proposedName = (string)exceptionEntry.Property("Name").CurrentValue;
                if (currentAttachmentSet.Name != proposedName)
                {
                    ModelState.AddModelError("Name", $"Current value: {currentAttachmentSet.Name}");
                }

                var proposedDescription = (string)exceptionEntry.Property("Description").CurrentValue;
                if (currentAttachmentSet.Description != proposedDescription)
                {
                    ModelState.AddModelError("Description", $"Current value: {currentAttachmentSet.Description}");
                }

                var proposedRegionID = (int?)exceptionEntry.Property("RegionID").CurrentValue;
                if (currentAttachmentSet.RegionID != proposedRegionID)
                {
                    ModelState.AddModelError("SubRegionID", $"Current value: {currentAttachmentSet.SubRegion.Name}");
                }

                var proposedSubRegionID = (int?)exceptionEntry.Property("SubRegionID").CurrentValue;
                if (currentAttachmentSet.SubRegionID != proposedSubRegionID)
                {
                    ModelState.AddModelError("SubRegionID", $"Current value: {currentAttachmentSet.SubRegion.Name}");
                }

                var proposedAttachmentRedundancyID = (int)exceptionEntry.Property("AttachmentRedundancyID").CurrentValue;
                if (currentAttachmentSet.AttachmentRedundancyID != proposedAttachmentRedundancyID)
                {
                    ModelState.AddModelError("AttachmentRedundancyID", $"Current value: {currentAttachmentSet.AttachmentRedundancy.Name}");
                }

                var proposedContractBandwidthID = (int)exceptionEntry.Property("ContractBandwidthID").CurrentValue;
                if (currentAttachmentSet.ContractBandwidthID != proposedContractBandwidthID)
                {
                    ModelState.AddModelError("ContractBandwidthID", $"Current value: {currentAttachmentSet.ContractBandwidth.BandwidthKbps}");
                }

                var proposedTenantID = (int)exceptionEntry.Property("TenantID").CurrentValue;
                if (currentAttachmentSet.TenantID != proposedTenantID)
                {
                    ModelState.AddModelError("TenantID", $"Current value: {currentAttachmentSet.Tenant.Name}");
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

            await PopulateTenantsDropDownList();
            await PopulateSubRegionsDropDownList(currentAttachmentSet.RegionID);
            await PopulateContractBandwidthsDropDownList();
            await PopulateAttachmentRedundancyDropDownList();
            return View(Mapper.Map<AttachmentSetViewModel>(currentAttachmentSet));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attachmentSet = await AttachmentSetService.GetByIDAsync(id.Value);
            if (attachmentSet == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("GetAll");
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

            return View(Mapper.Map<AttachmentSetViewModel>(attachmentSet));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(AttachmentSetViewModel attachmentSet)
        {  
            try
            {
                var dbResult = await AttachmentSetService.UnitOfWork.AttachmentSetRepository.GetAsync(q => q.AttachmentSetID == attachmentSet.AttachmentSetID, AsTrackable:false);
                var currentAttachmentSet = dbResult.SingleOrDefault();

                if (currentAttachmentSet != null)
                {
                    await AttachmentSetService.DeleteAsync(Mapper.Map<AttachmentSet>(attachmentSet));
                }
                return RedirectToAction("GetAll");
            }

            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { concurrencyError = true, id = attachmentSet.AttachmentSetID });
            }
        }

        private async Task PopulateAttachmentRedundancyDropDownList(object selectedAttachmentRedundancy = null)
        {
            var attachmentRedundancies = await AttachmentSetService.UnitOfWork.AttachmentRedundancyRepository.GetAsync();
            ViewBag.AttachmentRedundancyID = new SelectList(attachmentRedundancies, "AttachmentRedundancyID", "Name", selectedAttachmentRedundancy);
        }

        private async Task PopulateRegionsDropDownList(object selectedRegion = null)
        {
            var regions = await AttachmentSetService.UnitOfWork.RegionRepository.GetAsync();
            ViewBag.RegionID = new SelectList(regions, "RegionID", "Name", selectedRegion);
        }

        private async Task PopulateSubRegionsDropDownList(int regionID, object selectedSubRegion = null)
        {
            var subRegions = await AttachmentSetService.UnitOfWork.SubRegionRepository.GetAsync(q => q.RegionID == regionID);
            ViewBag.SubRegionID = new SelectList(subRegions, "SubRegionID", "Name", selectedSubRegion);
        }

        private async Task PopulateTenantsDropDownList(object selectedTenant = null)
        {
            var tenants = await AttachmentSetService.UnitOfWork.TenantRepository.GetAsync();
            ViewBag.TenantID = new SelectList(tenants, "TenantID", "Name", selectedTenant);
        }

        private async Task PopulateContractBandwidthsDropDownList(object selectedContractBandwidth = null)
        {
            var contractBandwidths = await AttachmentSetService.UnitOfWork.ContractBandwidthRepository.GetAsync();
            ViewBag.ContractBandwidthID = new SelectList(contractBandwidths, "ContractBandwidthID", "BandwidthKbps", selectedContractBandwidth);
        }
    }
}
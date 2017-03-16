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
        public async Task<IActionResult> GetAllByTenantID(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.Tenant = await AttachmentSetService.UnitOfWork.TenantRepository.GetByIDAsync(id);
            var attachmentSets = await AttachmentSetService.UnitOfWork.AttachmentSetRepository.GetAsync(q => q.TenantID == id,
                includeProperties: "Tenant,SubRegion,Region,AttachmentRedundancy");

            return View(Mapper.Map<List<AttachmentSetViewModel>>(attachmentSets));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbResult = await AttachmentSetService.UnitOfWork.AttachmentSetRepository.GetAsync(q => q.AttachmentSetID == id, includeProperties: 
                "Tenant,SubRegion.Region,AttachmentRedundancy");
            var item = dbResult.SingleOrDefault();

            if (item == null)
            {
                return NotFound();
            }
            return View(Mapper.Map<AttachmentSetViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> CreateStep1(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.Tenant = GetTenant(id.Value);
            await PopulateRegionsDropDownList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStep2(int? id, [Bind("RegionID,Name")] RegionViewModel region)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.Tenant = GetTenant(id.Value);
            await PopulateSubRegionsDropDownList(region.RegionID);
            await PopulateAttachmentRedundancyDropDownList();
            ViewBag.Region = region;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,RegionID,SubRegionID,TenantID,AttachmentRedundancyID,IsLayer3")] AttachmentSetViewModel attachmentSet)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await AttachmentSetService.AddAsync(Mapper.Map<AttachmentSet>(attachmentSet));

                    return RedirectToAction("GetAllByTenantID", new { id = attachmentSet.TenantID });
                }
            }
            catch (DbUpdateException /** ex **/ )
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            await PopulateDropDownLists(attachmentSet.RegionID);
            return View(attachmentSet);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbResult = await AttachmentSetService.UnitOfWork.AttachmentSetRepository.GetAsync(q => q.AttachmentSetID == id.Value, 
                includeProperties:"Tenant,SubRegion,Region,AttachmentRedundancy");
            var attachmentSet = dbResult.SingleOrDefault();

            if (attachmentSet == null)
            {
                return NotFound();
            }

            await PopulateDropDownLists(attachmentSet.RegionID);

            return View(Mapper.Map<AttachmentSetViewModel>(attachmentSet));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("AttachmentSetID,Name,Description,RegionID,SubRegionID,TenantID,AttachmentRedundancyID,IsLayer3,RowVersion")] AttachmentSetViewModel attachmentSet)
        {
            if (id != attachmentSet.AttachmentSetID)
            {
                return NotFound();
            }

            var dbResult = await AttachmentSetService.UnitOfWork.AttachmentSetRepository.GetAsync(q => q.AttachmentSetID == id, 
                includeProperties:"SubRegion,Region,Tenant,AttachmentRedundancy", AsTrackable: false);
            var currentAttachmentSet = dbResult.SingleOrDefault();

            try
            {
                if (ModelState.IsValid)
                {
                    if (currentAttachmentSet == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save changes. The attachment set was deleted by another user.");
                    }
                    else
                    {
                        var mappedAttachmentSet = Mapper.Map<AttachmentSet>(attachmentSet);
                        var validationResult = await AttachmentSetService.ValidateChangesAsync(mappedAttachmentSet);

                        if (!validationResult.IsSuccess)
                        {
                            validationResult.GetMessageList().ForEach(message => ModelState.AddModelError(string.Empty, message));
                        }
                        else
                        {
                            await AttachmentSetService.UpdateAsync(mappedAttachmentSet);
                            return RedirectToAction("GetAllByTenantID", new { id = attachmentSet.TenantID });
                        }
                    }
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

            await PopulateDropDownLists(attachmentSet.RegionID);
            return View(Mapper.Map<AttachmentSetViewModel>(attachmentSet));
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
                    return RedirectToAction("GetAllByTenantID", new { id = attachmentSet.TenantID });
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
                return RedirectToAction("GetAllByTenantID", new { id = currentAttachmentSet.TenantID });
            }

            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { concurrencyError = true, id = attachmentSet.AttachmentSetID });
            }
        }

        /// <summary>
        /// Helper to populate the drop-down lists for the view.
        /// </summary>
        /// <param name="attachmentSet"></param>
        /// <returns></returns>
        private async Task PopulateDropDownLists(int regionID)
        {
            await PopulateSubRegionsDropDownList(regionID);
            await PopulateAttachmentRedundancyDropDownList();
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

        private async Task GetTenant(int tenantID)
        {
            ViewBag.Tenant = await AttachmentSetService.UnitOfWork.TenantRepository.GetByIDAsync(tenantID);
        }
    }
}

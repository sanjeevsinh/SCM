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
    public class AttachmentSetVrfController : BaseViewController
    {
        public AttachmentSetVrfController(IAttachmentSetVrfService attachmentSetVrfService, IMapper mapper)
        {
           AttachmentSetVrfService = attachmentSetVrfService;
           Mapper = mapper;
        }
        private IAttachmentSetVrfService AttachmentSetVrfService { get; set; }
        private IMapper Mapper { get; set; }

        [HttpGet]
        public async Task<IActionResult> GetAllByAttachmentSetID(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attachmentSetVrfs = await AttachmentSetVrfService.UnitOfWork.AttachmentSetVrfRepository.GetAsync(q => q.AttachmentSetID == id);
            return View(Mapper.Map<List<AttachmentSetVrfViewModel>>(attachmentSetVrfs));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbResult = await AttachmentSetVrfService.UnitOfWork.AttachmentSetVrfRepository.GetAsync(q => q.AttachmentSetVrfID == id, 
                includeProperties: "AttachmentSet,Vrf");
            var item = dbResult.SingleOrDefault();

            if (item == null)
            {
                return NotFound();
            }
            return View(Mapper.Map<AttachmentSetVrfViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await PopulateVrfsDropDownList(id.Value);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AttachmentSetID,VrfID")] AttachmentSetVrfViewModel attachmentSetVrf)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await AttachmentSetVrfService.AddAsync(Mapper.Map<AttachmentSetVrf>(attachmentSetVrf));
                    return RedirectToAction("GetAllByAttachmentSetID", new { id = attachmentSetVrf.AttachmentSetID });
                }
            }
            catch (DbUpdateException /** ex **/ )
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            await PopulateVrfsDropDownList(attachmentSetVrf.VrfID);
            return View(attachmentSetVrf);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbResult = await AttachmentSetVrfService.UnitOfWork.AttachmentSetVrfRepository.GetAsync(q => q.AttachmentSetVrfID == id.Value, 
                includeProperties:"AttachmentSet,Vrf");
            var attachmentSetVrf = dbResult.SingleOrDefault();

            if (attachmentSetVrf == null)
            {
                return NotFound();
            }

            await PopulateVrfsDropDownList(attachmentSetVrf.AttachmentSetID);
            return View(Mapper.Map<AttachmentSetVrfViewModel>(attachmentSetVrf));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("AttachmentSetVrfID,AttachmentSetID,VrfID,RowVersion")] AttachmentSetVrfViewModel attachmentSetVrf)
        {
            if (id != attachmentSetVrf.AttachmentSetVrfID)
            {
                return NotFound();
            }

            var dbResult = await AttachmentSetVrfService.UnitOfWork.AttachmentSetVrfRepository.GetAsync(q => q.AttachmentSetVrfID == id, 
                includeProperties:"AttachmentSet,Vrf", AsTrackable: false);
            var currentAttachmentSetVrf = dbResult.SingleOrDefault();

            try
            {
                if (ModelState.IsValid)
                {
                    if (currentAttachmentSetVrf == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save changes. The VRF was deleted by another user.");
                        return View(attachmentSetVrf);
                    }

                    await AttachmentSetVrfService.UpdateAsync(Mapper.Map<AttachmentSetVrf>(attachmentSetVrf));
                    return RedirectToAction("GetAllByAttachmentSetID", new { id = attachmentSetVrf.AttachmentSetID });
                }
            }

            catch (DbUpdateConcurrencyException ex)
            {
                var exceptionEntry = ex.Entries.Single();

                var proposedAttachmentSetID = (int)exceptionEntry.Property("AttachmentSetID").CurrentValue;
                if (currentAttachmentSetVrf.AttachmentSetID!= proposedAttachmentSetID)
                {
                    ModelState.AddModelError("AttachmentSetID", $"Current value: {currentAttachmentSetVrf.AttachmentSet.Name}");
                }

                var proposedVrfID = (int)exceptionEntry.Property("VrfID").CurrentValue;
                if (currentAttachmentSetVrf.VrfID != proposedVrfID)
                {
                    ModelState.AddModelError("VrfID", $"Current value: {currentAttachmentSetVrf.Vrf.Name}");
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

            await PopulateVrfsDropDownList(currentAttachmentSetVrf.AttachmentSetID);
            return View(Mapper.Map<AttachmentSetVrfViewModel>(currentAttachmentSetVrf));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attachmentSetVrf = await AttachmentSetVrfService.GetByIDAsync(id.Value);
            if (attachmentSetVrf == null)
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

            return View(Mapper.Map<AttachmentSetVrfViewModel>(attachmentSetVrf));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(AttachmentSetVrfViewModel attachmentSetVrf)
        {  
            try
            {
                var dbResult = await AttachmentSetVrfService.UnitOfWork.AttachmentSetVrfRepository.GetAsync(q => q.AttachmentSetVrfID == attachmentSetVrf.AttachmentSetVrfID, AsTrackable:false);
                var currentAttachmentSetVrf = dbResult.SingleOrDefault();

                if (currentAttachmentSetVrf != null)
                {
                    await AttachmentSetVrfService.DeleteAsync(Mapper.Map<AttachmentSetVrf>(attachmentSetVrf));
                }
                return RedirectToAction("GetAllByAttachmentSetID", new { id = attachmentSetVrf.AttachmentSetID });
            }

            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { concurrencyError = true, id = attachmentSetVrf.AttachmentSetVrfID });
            }
        }

        private async Task PopulateVrfsDropDownList(int attachmentSetID, object selectedVrf = null)
        {
            var dbResult = await AttachmentSetVrfService.UnitOfWork.AttachmentSetRepository.GetAsync(q => q.AttachmentSetID == attachmentSetID);
            var attachmentSet = dbResult.SingleOrDefault();
            if (attachmentSet != null) {
                var vrfs = await AttachmentSetVrfService.UnitOfWork.AttachmentSetVrfRepository.GetAsync(q => q.Vrf.Device.Location.SubRegion.RegionID == attachmentSet.RegionID);
                ViewBag.AttachmentSetVrfID = new SelectList(vrfs, "VrfID", "Name", selectedVrf);
            }
        }
    }
}

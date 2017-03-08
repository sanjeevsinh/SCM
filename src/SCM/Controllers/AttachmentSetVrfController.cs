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

            var attachmentSet = await GetAttachmentSet(id.Value);
            ViewBag.AttachmentSet = attachmentSet;

            var attachmentSetVrfs = await AttachmentSetVrfService.UnitOfWork.AttachmentSetVrfRepository.GetAsync(q => q.AttachmentSetID == id.Value, 
                includeProperties:"Vrf.Device.Location.SubRegion.Region,Vrf.Interfaces.Port,Vrf.InterfaceVlans,Vrf.Interfaces.ContractBandwidthPool");

            var validationResult = await AttachmentSetVrfService.ValidateVrfsAsync(attachmentSet);
            if (!validationResult.IsSuccess)
            {
                validationResult.GetMessageList().ForEach(message => ModelState.AddModelError(string.Empty, message));
            }
            else
            {
                ViewData["ValidationSuccessMessage"] = "The VRFs for this attachment set are configured correctly!";
            }

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
                includeProperties: "AttachmentSet.Tenant,Vrf.Device.Location.SubRegion.Region,Vrf.Device.Plane,Vrf.Interfaces.Port,Vrf.Interfaces.ContractBandwidthPool");
            var item = dbResult.SingleOrDefault();

            if (item == null)
            {
                return NotFound();
            }

            ViewBag.AttachmentSet = await GetAttachmentSet(item.AttachmentSetID);
            return View(Mapper.Map<AttachmentSetVrfViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> CreateStep1(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attachmentSet = await AttachmentSetVrfService.UnitOfWork.AttachmentSetRepository.GetByIDAsync(id.Value);
            await PopulateLocationsDropDownList(attachmentSet);
            await PopulatePlanesDropDownList();
            ViewBag.AttachmentSet = attachmentSet;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStep2([Bind("AttachmentSetID,LocationID,PlaneID,TenantID")] AttachmentSetVrfSelectionViewModel attachmentSelection)
        {
            await PopulateVrfsDropDownList(attachmentSelection);
            ViewBag.AttachmentSet = await GetAttachmentSet(attachmentSelection.AttachmentSetID);
            ViewBag.AttachmentSelection = attachmentSelection;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AttachmentSetID,VrfID,Preference")] AttachmentSetVrfViewModel attachmentSetVrf,
            [Bind("AttachmentSetID,TenantID,LocationID,PlaneID")] AttachmentSetVrfSelectionViewModel attachmentSelection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var mappedAttachmentSetVrf = Mapper.Map<AttachmentSetVrf>(attachmentSetVrf);
                    var validationResult = await AttachmentSetVrfService.ValidateVrfChangesAsync(mappedAttachmentSetVrf);

                    if (!validationResult.IsSuccess)
                    {
                        ModelState.AddModelError(string.Empty, validationResult.GetMessage());

                        ViewBag.AttachmentSet = await GetAttachmentSet(attachmentSetVrf.AttachmentSetID);
                        ViewBag.AttachmentSelection = attachmentSelection;
                        await PopulateVrfsDropDownList(attachmentSelection);
       
                        return View("CreateStep2", attachmentSetVrf);
                    }

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

            ViewBag.AttachmentSet = await GetAttachmentSet(attachmentSetVrf.AttachmentSetID);
            await PopulateVrfsDropDownList(attachmentSelection);
            ViewBag.AttachmentSelection = attachmentSelection;
            return View("CreateStep2", attachmentSetVrf);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbResult = await AttachmentSetVrfService.UnitOfWork.AttachmentSetVrfRepository.GetAsync(q => q.AttachmentSetVrfID == id.Value, 
                includeProperties:"AttachmentSet,Vrf.Device.Location,Vrf.Interfaces.Port,Vrf.Interfaces.ContractBandwidthPool");
            var attachmentSetVrf = dbResult.SingleOrDefault();

            if (attachmentSetVrf == null)
            {
                return NotFound();
            }

            await PopulateVrfsDropDownList(new AttachmentSetVrfSelectionViewModel
            {
                LocationID = attachmentSetVrf.Vrf.Device.LocationID,
                TenantID = attachmentSetVrf.AttachmentSet.TenantID,
                PlaneID = attachmentSetVrf.Vrf.Device.PlaneID
            });

            ViewBag.AttachmentSet = await GetAttachmentSet(attachmentSetVrf.AttachmentSetID);
            return View(Mapper.Map<AttachmentSetVrfViewModel>(attachmentSetVrf));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("AttachmentSetVrfID,AttachmentSetID,VrfID,Preference,RowVersion")] AttachmentSetVrfViewModel attachmentSetVrf)
        {
            if (id != attachmentSetVrf.AttachmentSetVrfID)
            {
                return NotFound();
            }

            var dbResult = await AttachmentSetVrfService.UnitOfWork.AttachmentSetVrfRepository.GetAsync(q => q.AttachmentSetVrfID == id, 
                includeProperties:"AttachmentSet,Vrf.Device,Vrf.Interfaces", AsTrackable: false);
            var currentAttachmentSetVrf = dbResult.SingleOrDefault();

            try
            {
                if (ModelState.IsValid)
                {
                    if (currentAttachmentSetVrf == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save changes. The record was deleted by another user.");
                    }
                    else
                    {
                        var validationResult = await AttachmentSetVrfService.ValidateVrfChangesAsync(currentAttachmentSetVrf);

                        if (!validationResult.IsSuccess)
                        {
                            validationResult.GetMessageList().ForEach(message => ModelState.AddModelError(string.Empty, message));
                        }
                        else
                        {
                            await AttachmentSetVrfService.UpdateAsync(Mapper.Map<AttachmentSetVrf>(attachmentSetVrf));
                            return RedirectToAction("GetAllByAttachmentSetID", new { id = attachmentSetVrf.AttachmentSetID });
                        }
                    }
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

            await PopulateVrfsDropDownList(new AttachmentSetVrfSelectionViewModel
            {
                LocationID = currentAttachmentSetVrf.Vrf.Device.LocationID,
                TenantID = currentAttachmentSetVrf.AttachmentSet.TenantID,
                PlaneID = currentAttachmentSetVrf.Vrf.Device.PlaneID
            });

            ViewBag.AttachmentSet = await GetAttachmentSet(currentAttachmentSetVrf.AttachmentSetID);
            return View(Mapper.Map<AttachmentSetVrfViewModel>(currentAttachmentSetVrf));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbResult = await AttachmentSetVrfService.UnitOfWork.AttachmentSetVrfRepository.GetAsync(q => q.AttachmentSetVrfID == id.Value, 
                includeProperties: "Vrf.Interfaces");
            var attachmentSetVrf = dbResult.SingleOrDefault();

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
                ViewData["ErrorMessage"] = "The record you attempted to delete "
                    + "was modified by another user after you got the original values. "
                    + "The delete operation was cancelled and the current values in the "
                    + "database have been displayed. If you still want to delete this "
                    + "record, click the Delete button again. Otherwise "
                    + "click the Back to List hyperlink.";
            }

            ViewBag.AttachmentSet = await GetAttachmentSet(attachmentSetVrf.AttachmentSetID);
            return View(Mapper.Map<AttachmentSetVrfViewModel>(attachmentSetVrf));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(AttachmentSetVrfViewModel attachmentSetVrf)
        {  
            try
            {
                var dbResult = await AttachmentSetVrfService.UnitOfWork.AttachmentSetVrfRepository.GetAsync(q => q.AttachmentSetVrfID == attachmentSetVrf.AttachmentSetVrfID,
                    includeProperties:"AttachmentSet,Vrf.Interfaces", AsTrackable:false);
                var currentAttachmentSetVrf = dbResult.SingleOrDefault();

                if (currentAttachmentSetVrf != null)
                {
                    var validationResult = await AttachmentSetVrfService.ValidateVrfChangesAsync(currentAttachmentSetVrf);
                    if (!validationResult.IsSuccess)
                    {
                        ViewData["ErrorMessage"] = validationResult.GetHtmlListMessage();
                        ViewBag.AttachmentSet = currentAttachmentSetVrf.AttachmentSet;
                        return View(Mapper.Map<AttachmentSetVrfViewModel>(currentAttachmentSetVrf));
                    }

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

        private async Task PopulatePlanesDropDownList(object selectedPlane = null)
        {
            var planes = await AttachmentSetVrfService.UnitOfWork.PlaneRepository.GetAsync();
            ViewBag.PlaneID = new SelectList(planes, "PlaneID", "Name", selectedPlane);
        }

        private async Task PopulateLocationsDropDownList(AttachmentSet attachmentSet)
        {

            IEnumerable<Location> locations = await AttachmentSetVrfService.UnitOfWork.LocationRepository.GetAsync(q => q.SubRegion.RegionID == attachmentSet.RegionID);
            if (attachmentSet.SubRegionID != null)
            {
                locations = locations.Where(q => q.SubRegionID == attachmentSet.SubRegionID);
            }
 
            ViewBag.LocationID = new SelectList(locations, "LocationID", "SiteName");
        }

        private async Task PopulateVrfsDropDownList(AttachmentSetVrfSelectionViewModel attachmentSelection, object selectedVrf = null)
        {

            var vrfs = await AttachmentSetVrfService.UnitOfWork.VrfRepository.GetAsync(q => q.Device.LocationID == attachmentSelection.LocationID 
                && q.TenantID == attachmentSelection.TenantID, includeProperties:"Device");

            if (attachmentSelection.PlaneID != null)
            {
                vrfs = vrfs.Where(q => q.Device.PlaneID == attachmentSelection.PlaneID).ToList();
            }
     
            ViewBag.VrfID = new SelectList(vrfs, "VrfID", "Name", selectedVrf);
        }

        private async Task<AttachmentSet> GetAttachmentSet(int attachmentSetID)
        {
            var dbResult = await AttachmentSetVrfService.UnitOfWork.AttachmentSetRepository.GetAsync(q => q.AttachmentSetID == attachmentSetID,
                 includeProperties:"AttachmentRedundancy");

            return dbResult.SingleOrDefault();
           
        }
    }
}

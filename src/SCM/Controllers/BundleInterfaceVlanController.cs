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
    public class BundleInterfaceVlanController : BaseViewController
    {
        public BundleInterfaceVlanController(IBundleInterfaceVlanService bundleIfaceVlanService, IMapper mapper)
        {
            BundleInterfaceVlanService = bundleIfaceVlanService;
            Mapper = mapper;
        }
        private IBundleInterfaceVlanService BundleInterfaceVlanService { get; set; }
        private IMapper Mapper { get; set; }

        [HttpGet]
        public async Task<IActionResult> GetByID(int id)
        {
            var item = await BundleInterfaceVlanService.GetByIDAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(Mapper.Map<BundleInterfaceVlanViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllByBundleInterfaceID(int id)
        {
            var dbResult = await BundleInterfaceVlanService.UnitOfWork.BundleInterfaceRepository.GetAsync(q => q.BundleInterfaceID == id);
            var bundleIface = dbResult.SingleOrDefault();
            if (bundleIface == null)
            {
                return NotFound();
            }

            var bundleIfaceVlans = await BundleInterfaceVlanService.UnitOfWork.BundleInterfaceVlanRepository.GetAsync(q => q.BundleInterfaceID == id,
                includeProperties: "BundleInterface,Vrf");

            ViewBag.BundleInterface = bundleIface;
            return View(Mapper.Map<List<BundleInterfaceVlanViewModel>>(bundleIfaceVlans));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbResult = await BundleInterfaceVlanService.UnitOfWork.BundleInterfaceVlanRepository.GetAsync(q => q.BundleInterfaceVlanID == id.Value,
                includeProperties: "BundleInterface,Vrf");
            var item = dbResult.SingleOrDefault();

            if (item == null)
            {
                return NotFound();
            }

            await PopulateBundleInterfaceItem(item.BundleInterfaceID);
            return View(Mapper.Map<BundleInterfaceVlanViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? id) 
        {
            if (id == null)
            {
                return NotFound();
            }

            await PopulateVrfsDropDownList(id.Value);
            await PopulateBundleInterfaceItem(id.Value);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BundleInterfaceID,IpAddress,IsLayer3,SubnetMask,VlanTag,VrfID")] BundleInterfaceVlanViewModel bundleIfaceVlan)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var mappedBundleIfaceVlan = Mapper.Map<BundleInterfaceVlan>(bundleIfaceVlan);
                    var validationResult = await BundleInterfaceVlanService.ValidateBundleInterfaceVlan(mappedBundleIfaceVlan);

                    if (!validationResult.IsValid) 
                    {
                        ModelState.AddModelError(string.Empty, validationResult.GetMessage());
                    }
                    else
                    {
                        await BundleInterfaceVlanService.AddAsync(mappedBundleIfaceVlan);
                        return RedirectToAction("GetAllByBundleInterfaceID", new { id = bundleIfaceVlan.BundleInterfaceID });
                    }
                }
            }
            catch (DbUpdateException)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            await PopulateVrfsDropDownList(bundleIfaceVlan.BundleInterfaceID);
            await PopulateBundleInterfaceItem(bundleIfaceVlan.BundleInterfaceID);
            return View(bundleIfaceVlan);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BundleInterfaceVlan bundleIfaceVlan = await BundleInterfaceVlanService.GetByIDAsync(id.Value);

            if (bundleIfaceVlan == null)
            {
                return NotFound();
            }

            await PopulateBundleInterfaceItem(bundleIfaceVlan.BundleInterfaceID);
            await PopulateVrfsDropDownList(bundleIfaceVlan.BundleInterfaceID);
            return View(Mapper.Map<BundleInterfaceVlanViewModel>(bundleIfaceVlan));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("BundleInterfaceVlanID,BundleInterfaceID,IsLayer3,IpAddress,SubnetMask,VlanTag,VrfID,RowVersion")] BundleInterfaceVlanViewModel bundleIfaceVlan)
        {
            if (id != bundleIfaceVlan.BundleInterfaceVlanID)
            {
                return NotFound();
            }

            var dbResult = await BundleInterfaceVlanService.UnitOfWork.BundleInterfaceVlanRepository.GetAsync(filter: d => d.BundleInterfaceVlanID == id,
               includeProperties:"Vrf", AsTrackable: false);
            var currentBundleIfaceVlan = dbResult.SingleOrDefault();

            try
            {
                if (ModelState.IsValid)
                {
                    var mappedBundleIfaceVlan = Mapper.Map<BundleInterfaceVlan>(bundleIfaceVlan);
                    var validationResult = await BundleInterfaceVlanService.ValidateBundleInterfaceVlanChanges(mappedBundleIfaceVlan,
                        currentBundleIfaceVlan);

                    if (!validationResult.IsValid)
                    {
                        ModelState.AddModelError(string.Empty, validationResult.GetMessage());
                    }
                    else
                    {
                        await BundleInterfaceVlanService.UpdateAsync(mappedBundleIfaceVlan);
                        return RedirectToAction("GetAllByBundleInterfaceID", new { id = bundleIfaceVlan.BundleInterfaceID });
                    }
                }
            }

            catch (DbUpdateConcurrencyException ex)
            {
                var exceptionEntry = ex.Entries.Single();

                var proposedIpAddress = (string)exceptionEntry.Property("IpAddress").CurrentValue;
                if (currentBundleIfaceVlan.IpAddress != proposedIpAddress)
                {
                    ModelState.AddModelError("IpAddress", $"Current value: {currentBundleIfaceVlan.IpAddress}");
                }

                var proposedSubnetMask = (string)exceptionEntry.Property("SubnetMask").CurrentValue;
                if (currentBundleIfaceVlan.SubnetMask != proposedSubnetMask)
                {
                    ModelState.AddModelError("SubnetMask", $"Current value: {currentBundleIfaceVlan.SubnetMask}");
                }

                var proposedVlanTag = (int)exceptionEntry.Property("VlanTag").CurrentValue;
                if (currentBundleIfaceVlan.VlanTag != proposedVlanTag)
                {
                    ModelState.AddModelError("VlanTag", $"Current value: {currentBundleIfaceVlan.VlanTag}");
                }

                var proposedVrfID = (int)exceptionEntry.Property("VrfID").CurrentValue;
                if (currentBundleIfaceVlan.VrfID != proposedVrfID)
                {
                    ModelState.AddModelError("VrfID", $"Current value: {currentBundleIfaceVlan.Vrf.Name}");
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

            await PopulateBundleInterfaceItem(currentBundleIfaceVlan.BundleInterfaceID);
            await PopulateVrfsDropDownList(currentBundleIfaceVlan.BundleInterfaceID);
            return View(Mapper.Map<BundleInterfaceVlanViewModel>(currentBundleIfaceVlan));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bundleIfaceVlan = await BundleInterfaceVlanService.GetByIDAsync(id.Value);
            if (bundleIfaceVlan == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("GetAllByBundleInterfaceID", new { id = bundleIfaceVlan.BundleInterfaceID });
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

            await PopulateBundleInterfaceItem(bundleIfaceVlan.BundleInterfaceID);
            return View(Mapper.Map<BundleInterfaceVlanViewModel>(bundleIfaceVlan));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(BundleInterfaceVlanViewModel bundleIfaceVlan)
        {
            try
            {
                var dbResult = await BundleInterfaceVlanService.UnitOfWork.BundleInterfaceVlanRepository.GetAsync(filter: d => d.BundleInterfaceVlanID == bundleIfaceVlan.BundleInterfaceVlanID, 
                    AsTrackable: false);
                var currentBundleInterfaceVlan = dbResult.SingleOrDefault();

                if (currentBundleInterfaceVlan != null)
                {
                    await BundleInterfaceVlanService.DeleteAsync(Mapper.Map<BundleInterfaceVlan>(bundleIfaceVlan));
                }
                return RedirectToAction("GetAllByBundleInterfaceID", new { id = bundleIfaceVlan.BundleInterfaceID });
            }

            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { concurrencyError = true, id = bundleIfaceVlan.BundleInterfaceVlanID });
            }
        }
        private async Task PopulateBundleInterfaceItem(int bundleIfaceID)
        {
            var dbResult = await BundleInterfaceVlanService.UnitOfWork.BundleInterfaceRepository.GetAsync(q => q.BundleInterfaceID == bundleIfaceID, includeProperties:"Device");
            var bundleIface = dbResult.Single();
            ViewBag.BundleInterface = bundleIface;
        }
       
        private async Task PopulateVrfsDropDownList(int bundleIfaceID, object selectedVrf = null)
        {
            var bundleIface = await BundleInterfaceVlanService.UnitOfWork.BundleInterfaceRepository.GetByIDAsync(bundleIfaceID);
            if (bundleIface != null)
            {
                var vrfs = await BundleInterfaceVlanService.UnitOfWork.VrfRepository.GetAsync(q => q.DeviceID == bundleIface.DeviceID);
                ViewBag.VrfID = new SelectList(vrfs, "VrfID", "Name", selectedVrf);
            }
        }
        
        /// <summary>
        /// Validates that a bundle interface is tagged. If the bundle interface is not tagged then vlans
        /// cannot be created.
        /// </summary>
        /// <param name="bundleIfaceVlan"></param>
        /// <returns></returns>
        private async Task<bool> ValidateBundleInterfaceVlan(int bundleIfaceID)
        {
            var iface = await BundleInterfaceVlanService.UnitOfWork.BundleInterfaceRepository.GetByIDAsync(bundleIfaceID);
            return iface.IsTagged;
        }
    }
}

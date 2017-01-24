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
    public class DeviceController : BaseViewController
    {
        public DeviceController(IDeviceService deviceService, IMapper mapper)
        {
           DeviceService = deviceService;
           Mapper = mapper;
        }
        private IDeviceService DeviceService { get; set; }
        private IMapper Mapper { get; set; }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var devices = await DeviceService.GetAllAsync();
            return View(Mapper.Map<List<DeviceViewModel>>(devices));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await DeviceService.GetByIDAsync(id.Value);
            if (item == null)
            {
                return NotFound();
            }
            return View(Mapper.Map<DeviceViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulatePlanesDropDownList();
            await PopulateLocationsDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,PlaneID,LocationID")] DeviceViewModel device)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await DeviceService.AddAsync(Mapper.Map<Device>(device));
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

            await PopulateLocationsDropDownList(device.LocationID);
            await PopulatePlanesDropDownList(device.PlaneID);
            return View(device);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Device device = await DeviceService.GetByIDAsync(id.Value);

            if (device == null)
            {
                return NotFound();
            }

            await PopulatePlanesDropDownList(device.Plane);
            await PopulateLocationsDropDownList(device.Location);

            return View(Mapper.Map<DeviceViewModel>(device));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("ID,Name,Description,PlaneID,LocationID,RowVersion")] DeviceViewModel device)
        {
            if (id != device.ID)
            {
                return NotFound();
            }

            var dbResult = await DeviceService.UnitOfWork.DeviceRepository.GetAsync(filter: d => d.ID == id, 
                includeProperties:"Plane,Location", AsTrackable: false);
            var currentDevice = dbResult.SingleOrDefault();

            try
            {
                if (ModelState.IsValid)
                {
                    if (currentDevice == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save changes. The device was deleted by another user.");
                        return View(device);
                    }

                    await DeviceService.UpdateAsync(Mapper.Map<Device>(device));
                    return RedirectToAction("GetAll");
                }
            }

            catch (DbUpdateConcurrencyException ex)
            {
                var exceptionEntry = ex.Entries.Single();

                var proposedName = (string)exceptionEntry.Property("Name").CurrentValue;
                if (currentDevice.Name != proposedName)
                {
                    ModelState.AddModelError("Name", $"Current value: {currentDevice.Name}");
                }

                var proposedDescription = (string)exceptionEntry.Property("Description").CurrentValue;
                if (currentDevice.Description != proposedDescription)
                {
                    ModelState.AddModelError("Description", $"Current value: {currentDevice.Description}");
                }

                var proposedPlaneID = (int)exceptionEntry.Property("PlaneID").CurrentValue;
                if (currentDevice.PlaneID != proposedPlaneID)
                {
                    ModelState.AddModelError("PlaneID", $"Current value: {currentDevice.Plane.Name}");
                }

                var proposedLocationID = (int)exceptionEntry.Property("LocationID").CurrentValue;
                if (currentDevice.LocationID != proposedLocationID)
                {
                    ModelState.AddModelError("LocationID", $"Current value: {currentDevice.Location.SiteName}");
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

            await PopulateLocationsDropDownList(currentDevice.Location);
            await PopulatePlanesDropDownList(currentDevice.Plane);

            return View(Mapper.Map<DeviceViewModel>(currentDevice));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = await DeviceService.GetByIDAsync(id.Value);
            if (device == null)
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

            return View(Mapper.Map<DeviceViewModel>(device));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(DeviceViewModel device)
        {  
            try
            {
                var dbResult = await DeviceService.UnitOfWork.DeviceRepository.GetAsync(filter: d => d.ID == device.ID, AsTrackable:false);
                var currentDevice = dbResult.SingleOrDefault();

                if (currentDevice != null)
                {
                    await DeviceService.DeleteAsync(Mapper.Map<Device>(device));
                }
                return RedirectToAction("GetAll");
            }

            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { concurrencyError = true, id = device.ID });
            }
        }

        private async Task PopulatePlanesDropDownList(object selectedPlane = null)
        {
            var planes = await DeviceService.UnitOfWork.PlaneRepository.GetAsync();
            ViewBag.PlaneID = new SelectList(planes, "PlaneID", "Name", selectedPlane);
        }

        private async Task PopulateLocationsDropDownList(object selectedLocation = null)
        {
            var locations = await DeviceService.UnitOfWork.LocationRepository.GetAsync();
            ViewBag.LocationID = new SelectList(locations, "LocationID", "SiteName", selectedLocation);
        }
    }
}

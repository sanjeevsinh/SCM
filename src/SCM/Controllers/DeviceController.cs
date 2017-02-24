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
using System.Net;

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

        [HttpPost]
        public async Task<IActionResult> CheckSync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var checkSyncResult = await DeviceService.CheckNetworkSyncAsync(id.Value);
            if (checkSyncResult.InSync)
            {
                ViewData["SyncSuccessMessage"] = "The Device is synchronised with the network.";
            }
            else
            {
                if (!checkSyncResult.NetworkSyncServiceResult.IsSuccess)
                {
                    ViewData["SyncErrorMessage"] = checkSyncResult.NetworkSyncServiceResult.GetMessage();
                }
                else
                {
                    ViewData["SyncErrorMessage"] = "The Device is not synchronised with the network. Press the 'Sync' button to update the network.";
                }
            }

            var item = await DeviceService.GetByIDAsync(id.Value);
            if (item == null)
            {
                return NotFound();
            }

            return View("Details", Mapper.Map<DeviceViewModel>(item));
        }


        [HttpPost]
        public async Task<IActionResult> Sync(int? id)
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

            var syncResult = await DeviceService.SyncToNetworkAsync(id.Value);

            if (syncResult.IsSuccess)
            {
                ViewData["SyncSuccessMessage"] = "The network is synchronised.";
            }
            else
            {
                ViewData["SyncErrorMessage"] = syncResult.GetMessage();
            }

            return View("Details", Mapper.Map<DeviceViewModel>(item));
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

            return View(Mapper.Map<DeviceViewModel>(device));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("ID,Description,RowVersion")] DeviceViewModel device)
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

                    // The name property must not be changed because this is used as a key
                    // in the network service model.

                    // LocationID and PlaneID properties must not be changed because these properties affect the 
                    // association of VRFs with VPNs using Attachment Sets.

                    device.Name = currentDevice.Name;
                    device.PlaneID = currentDevice.PlaneID;
                    device.LocationID = currentDevice.LocationID;
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

            return View(Mapper.Map<DeviceViewModel>(device));
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
                    var result = await DeviceService.DeleteAsync(Mapper.Map<Device>(device));
                    if (!result.IsSuccess)
                    {
                        ViewData["DeleteErrorMessage"] = result.GetMessage();
                        return View(Mapper.Map<DeviceViewModel>(currentDevice));
                    }
                }
                return RedirectToAction("GetAll");
            }

            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { concurrencyError = true, id = device.ID });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFromNetwork(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = await DeviceService.GetByIDAsync(id.Value);
            if (device == null)
            {

                ViewData["DeviceDeletedMessage"] = "The Device has been deleted by another user. Return to the list.";
                return View("DeviceDeleted");
            }

            var syncResult = await DeviceService.DeleteFromNetworkAsync(id.Value);
            if (syncResult.IsSuccess)
            {
                ViewData["SyncSuccessMessage"] = "The Device has been deleted from the network.";
            }
            else
            {
                if (syncResult.NetworkHttpResponse.HttpStatusCode == HttpStatusCode.NotFound)
                {
                    ViewData["SyncErrorMessage"] = "The Device has already been deleted from the network.";
                }
                else
                {
                    ViewData["SyncErrorMessage"] = "There was a problem deleting the Device from the network. " + syncResult.GetAllMessages();
                }
            }

            return View("Delete", Mapper.Map<DeviceViewModel>(device));
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

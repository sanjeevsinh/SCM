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
using Microsoft.AspNetCore.SignalR.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using SCM.Hubs;

namespace SCM.Controllers
{
    public class DeviceController : BaseViewController
    {
        public DeviceController(IDeviceService deviceService, 
            IMapper mapper,
            IConnectionManager signalRConnectionManager)
        {
           DeviceService = deviceService;
           Mapper = mapper;
           HubContext = signalRConnectionManager.GetHubContext<NetworkSyncHub>();
        }
        private IDeviceService DeviceService { get; set; }
        private IMapper Mapper { get; set; }
        private IHubContext HubContext { get; set; }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var devices = await DeviceService.GetAllAsync();

            var checkSyncResult = DeviceService.ShallowCheckNetworkSync(devices);
            if (checkSyncResult.IsSuccess)
            {
                ViewData["SuccessMessage"] = "All devices appear to be synchronised with the network.";
            }
            else
            {
                ViewData["ErrorMessage"] = FormatAsHtmlList(checkSyncResult.GetMessage());
            }

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
        public async Task CheckSync(int? id)
        {
            if (id == null)
            {
                RedirectToAction("PageNotFound");
                return;
            }

            var item = await DeviceService.GetByIDAsync(id.Value);
            if (item == null)
            {
                RedirectToAction("PageNotFound");
                return;
            }

            var mappedItem = Mapper.Map<DeviceViewModel>(item);
            var result = await DeviceService.CheckNetworkSyncAsync(item);
            if (result.IsSuccess)
            {
                HubContext.Clients.Group("Devices")
                    .onSingleComplete(mappedItem, true, $"Device {item.Name} is synchronised with the network.");
            }
            else
            {
                if (result.NetworkSyncServiceResults.Single().StatusCode == NetworkSyncStatusCode.Success)
                {
                    HubContext.Clients.Group("Devices")
                        .onSingleComplete(mappedItem, false,
                        FormatAsHtmlList($"Device {item.Name} is not synchronised with the network." 
                        + "Press the 'Sync' button to update the network."));
                }
                else
                {
                    HubContext.Clients.Group("Devices")
                        .onSingleComplete(mappedItem, false, FormatAsHtmlList(result.GetMessage()));
                }
            }

            await DeviceService.UpdateDeviceRequiresSyncAsync(item.ID, !result.IsSuccess, true);
        }


        [HttpPost]
        public async Task Sync(int? id)
        {
            if (id == null)
            {
                RedirectToAction("PageNotFound");
                return;
            }

            var item = await DeviceService.GetByIDAsync(id.Value);
            if (item == null)
            {
                RedirectToAction("PageNotFound");
                return;
            }

            var mappedItem = Mapper.Map<DeviceViewModel>(item);
            var syncResult = await DeviceService.SyncToNetworkAsync(item);

            if (syncResult.IsSuccess)
            {
                HubContext.Clients.Group("Devices")
                    .onSingleComplete(mappedItem, true, FormatAsHtmlList($"Device {item.Name} is synchronised with the network."));
            }
            else
            {
                HubContext.Clients.Group("Devices")
                    .onSingleComplete(mappedItem, false, FormatAsHtmlList(syncResult.GetMessage()));
            }

            await DeviceService.UpdateDeviceRequiresSyncAsync(item.ID, !syncResult.IsSuccess, true);
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
        public async Task<ActionResult> Edit(int id, [Bind("ID,Name,Description,RowVersion")] DeviceViewModel device)
        {
            if (id != device.ID)
            {
                return NotFound();
            }

            var currentDevice = await DeviceService.GetByIDAsync(id);
         
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
                    device.RequiresSync = currentDevice.RequiresSync;

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
                ViewData["ErrorMessage"] = "The record you attempted to delete "
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
                var currentDevice = await DeviceService.GetByIDAsync(device.ID);

                if (currentDevice == null)
                {
                    return NotFound();
                }

                var syncResult = await DeviceService.DeleteFromNetworkAsync(currentDevice);

                // Delete from network may return IsSuccess false if the resource was not found - this should be ignored
                // because it probably means the resource was either previously deleted from the network or it was 
                // never syncd to the network

                var inSync = true;
                ViewData["ErrorMessage"] = String.Empty;

                if (!syncResult.IsSuccess)
                {
                    syncResult.NetworkSyncServiceResults.ForEach(f => inSync = f.StatusCode != NetworkSyncStatusCode.NotFound ? false : inSync);
                }

                if (!inSync)
                {
                    ViewData["ErrorMessage"] += FormatAsHtmlList(syncResult.GetMessage());
                }
                else
                {
                    var result = await DeviceService.DeleteAsync(currentDevice);
                    if (!result.IsSuccess)
                    {
                        ViewData["ErrorMessage"] = FormatAsHtmlList(result.GetMessage());

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

                ViewData["DeviceDeletedMessage"] = "The device has been deleted by another user. Return to the list.";
                return View("DeviceDeleted");
            }

            await DeviceService.UpdateDeviceRequiresSyncAsync(device.ID, true, false);

            var syncResult = await DeviceService.DeleteFromNetworkAsync(device);
            if (syncResult.IsSuccess)
            {
                ViewData["SuccessMessage"] = "The device has been deleted from the network.";
            }
            else
            {
                ViewData["ErrorMessage"] = FormatAsHtmlList(syncResult.GetMessage());
            }

            device.RequiresSync = !syncResult.IsSuccess;
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

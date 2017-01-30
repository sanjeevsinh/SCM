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
    public class BundleInterfaceController : BaseViewController
    {
        public BundleInterfaceController(IBundleInterfaceService interfaceService, IMapper mapper)
        {
            BundleInterfaceService = interfaceService;
            Mapper = mapper;
        }
        private IBundleInterfaceService BundleInterfaceService { get; set; }
        private IMapper Mapper { get; set; }

        [HttpGet]
        public async Task<IActionResult> GetByID(int id)
        {
            var item = await BundleInterfaceService.GetByIDAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(Mapper.Map<BundleInterfaceViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllByDeviceID(int id)
        {
            var device = await BundleInterfaceService.UnitOfWork.DeviceRepository.GetByIDAsync(id);
            if (device == null)
            {
                return NotFound();
            }

            var bundleIfaces = await BundleInterfaceService.UnitOfWork.BundleInterfaceRepository.GetAsync(q => q.DeviceID == id,
                includeProperties: "Device,Vrf,InterfaceBandwidth");

            ViewBag.Device = device;
            return View(Mapper.Map<List<BundleInterfaceViewModel>>(bundleIfaces));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbResult = await BundleInterfaceService.UnitOfWork.BundleInterfaceRepository.GetAsync(q => q.BundleInterfaceID == id.Value,
                includeProperties: "InterfaceBandwidth,Device,Vrf");
            var item = dbResult.SingleOrDefault();

            if (item == null)
            {
                return NotFound();
            }

            await PopulateDeviceItem(item.DeviceID);
            return View(Mapper.Map<BundleInterfaceViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? id) 
        {
            if (id == null)
            {
                return NotFound();
            }

            await PopulateDeviceItem(id.Value);
            await PopulateInterfaceBandwidthsDropDownList();
            await PopulateVrfsDropDownList(id.Value);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,IpAddress,SubnetMask,IsTagged,IsLayer3,DeviceID,VrfID,InterfaceBandwidthID")] BundleInterfaceViewModel bundleIface)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await BundleInterfaceService.AddAsync(Mapper.Map<BundleInterface>(bundleIface));
                    return RedirectToAction("GetAllByDeviceID", new { id = bundleIface.DeviceID });
                }
            }
            catch (DbUpdateException)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            await PopulateDeviceItem(bundleIface.DeviceID);
            await PopulateInterfaceBandwidthsDropDownList();
            await PopulateVrfsDropDownList(bundleIface.DeviceID);
            return View(bundleIface);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BundleInterface bundleIface = await BundleInterfaceService.GetByIDAsync(id.Value);

            if (bundleIface == null)
            {
                return NotFound();
            }

            await PopulateDeviceItem(bundleIface.DeviceID);
            await PopulateInterfaceBandwidthsDropDownList();
            await PopulateVrfsDropDownList(bundleIface.DeviceID);
            return View(Mapper.Map<BundleInterfaceViewModel>(bundleIface));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("BundleInterfaceID,ID,IpAddress,SubnetMask,IsTagged,IsLayer3,DeviceID,VrfID,InterfaceBandwidthID,RowVersion")] BundleInterfaceViewModel bundleIface)
        {
            if (id != bundleIface.BundleInterfaceID)
            {
                return NotFound();
            }

            var dbResult = await BundleInterfaceService.UnitOfWork.BundleInterfaceRepository.GetAsync(filter: d => d.BundleInterfaceID == id,
               AsTrackable: false);
            var currentBundleInterface = dbResult.SingleOrDefault();

            try
            {
                if (ModelState.IsValid)
                {
                    if (currentBundleInterface == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save changes. The interface was deleted by another user.");
                        return View(bundleIface);
                    }

                    await BundleInterfaceService.UpdateAsync(Mapper.Map<BundleInterface>(bundleIface));
                    return RedirectToAction("GetAllByDeviceID", new { id = bundleIface.DeviceID });
                }
            }

            catch (DbUpdateConcurrencyException ex)
            {
                var exceptionEntry = ex.Entries.Single();

                var proposedIpAddress = (string)exceptionEntry.Property("IpAddress").CurrentValue;
                if (currentBundleInterface.IpAddress != proposedIpAddress)
                {
                    ModelState.AddModelError("IpAddress", $"Current value: {currentBundleInterface.IpAddress}");
                }

                var proposedSubnetMask = (string)exceptionEntry.Property("SubnetMask").CurrentValue;
                if (currentBundleInterface.SubnetMask != proposedSubnetMask)
                {
                    ModelState.AddModelError("SubnetMask", $"Current value: {currentBundleInterface.SubnetMask}");
                }

                var proposedInterfaceBandwidthID = (int)exceptionEntry.Property("InterfaceBandwidthID").CurrentValue;
                if (currentBundleInterface.InterfaceBandwidthID != proposedInterfaceBandwidthID)
                {
                    ModelState.AddModelError("InterfaceBandwidthID", $"Current value: {currentBundleInterface.InterfaceBandwidth.BandwidthKbps}");
                }

                var proposedVrfID = (int?)exceptionEntry.Property("VrfID").CurrentValue;
                if (currentBundleInterface.VrfID != proposedVrfID)
                {
                    ModelState.AddModelError("VrfID", $"Current value: {currentBundleInterface.Vrf.Name}");
                }

                var proposedIsTagged = (bool)exceptionEntry.Property("IsTagged").CurrentValue;
                if (currentBundleInterface.IsTagged != proposedIsTagged)
                {
                    ModelState.AddModelError("IsTagged", $"Current value: {currentBundleInterface.IsTagged}");
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

            await PopulateDeviceItem(currentBundleInterface.DeviceID);
            await PopulateInterfaceBandwidthsDropDownList();
            await PopulateVrfsDropDownList(currentBundleInterface.DeviceID);
            return View(Mapper.Map<BundleInterfaceViewModel>(currentBundleInterface));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bundleIface = await BundleInterfaceService.GetByIDAsync(id.Value);
            if (bundleIface == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("GetAllByDeviceID", new { id = bundleIface.DeviceID });
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

            return View(Mapper.Map<BundleInterfaceViewModel>(bundleIface));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(BundleInterfaceViewModel bundleIface)
        {
            try
            {
                var dbResult = await BundleInterfaceService.UnitOfWork.BundleInterfaceRepository.GetAsync(filter: d => d.DeviceID == bundleIface.DeviceID,
                    AsTrackable: false);
                var currentBundleInterface = dbResult.SingleOrDefault();

                if (currentBundleInterface != null)
                {
                    await BundleInterfaceService.DeleteAsync(Mapper.Map<BundleInterface>(bundleIface));
                }
                return RedirectToAction("GetAllByDeviceID", new { id = bundleIface.DeviceID });
            }

            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { concurrencyError = true, id = bundleIface.BundleInterfaceID });
            }
        }
        private async Task PopulateDeviceItem(int deviceID)
        {
            var device = await BundleInterfaceService.UnitOfWork.DeviceRepository.GetByIDAsync(deviceID);
            ViewBag.Device = device;
        }

        private async Task PopulateInterfaceBandwidthsDropDownList(object selectedInterfaceBandwidth = null)
        {
            var interfaceBandwidths = await BundleInterfaceService.UnitOfWork.InterfaceBandwidthRepository.GetAsync();
            ViewBag.InterfaceBandwidthID = new SelectList(interfaceBandwidths, "InterfaceBandwidthID", "BandwidthKbps", selectedInterfaceBandwidth);
        }
        private async Task PopulateVrfsDropDownList(int deviceID, object selectedVrf = null)
        {
            var vrfs = await BundleInterfaceService.UnitOfWork.VrfRepository.GetAsync(q => q.DeviceID == deviceID);
            ViewBag.VrfID = new SelectList(vrfs, "VrfID", "Name", selectedVrf);
        }
    }
}

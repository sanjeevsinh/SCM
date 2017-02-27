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
    public class BundleInterfacePortController : BaseViewController
    {
        public BundleInterfacePortController(IBundleInterfacePortService bundleInterfacePortService, IMapper mapper)
        {
            BundleInterfacePortService = bundleInterfacePortService;
            Mapper = mapper;
        }

        private IBundleInterfacePortService BundleInterfacePortService { get; set; }
        private IMapper Mapper { get; set; }

        [HttpGet]
        public async Task<IActionResult> GetByID(int id)
        {
            var item = await BundleInterfacePortService.GetByIDAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(Mapper.Map<BundleInterfacePortViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllByBundleInterfaceID(int id)
        {
            var bundleIface = await BundleInterfacePortService.UnitOfWork.BundleInterfaceRepository.GetByIDAsync(id);
            if (bundleIface == null)
            {
                return NotFound();
            }

            var bundleIfacePorts = await BundleInterfacePortService.UnitOfWork.BundleInterfacePortRepository.GetAsync(q => q.BundleInterfaceID == id,
                includeProperties: "BundleInterface,Port");

            ViewBag.BundleInterface = bundleIface;
            return View(Mapper.Map<List<BundleInterfacePortViewModel>>(bundleIfacePorts));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbResult = await BundleInterfacePortService.UnitOfWork.BundleInterfacePortRepository.GetAsync(q => q.BundleInterfacePortID == id.Value,
                includeProperties: "BundleInterface,Port");
            var item = dbResult.SingleOrDefault();

            if (item == null)
            {
                return NotFound();
            }

            await PopulateBundleInterfaceItem(item.BundleInterfaceID);
            return View(Mapper.Map<BundleInterfacePortViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? id) 
        {
            if (id == null)
            {
                return NotFound();
            }

            await PopulateBundleInterfaceItem(id.Value);
            await PopulatePortsDropDownList(id.Value);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BundleInterfaceID,PortID")] BundleInterfacePortViewModel bundleIfacePort)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!await ValidatePortBandwidth(bundleIfacePort))
                    {
                        ModelState.AddModelError(string.Empty, "The port bandwidth must match existing ports in the bundle.");
                    }
                    else
                    {

                        await BundleInterfacePortService.AddAsync(Mapper.Map<BundleInterfacePort>(bundleIfacePort));
                        return RedirectToAction("GetAllByBundleInterfaceID", new { id = bundleIfacePort.BundleInterfaceID });
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

            await PopulatePortsDropDownList(bundleIfacePort.BundleInterfaceID);
            await PopulateBundleInterfaceItem(bundleIfacePort.BundleInterfaceID);
            return View(bundleIfacePort);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BundleInterfacePort bundleIfacePort = await BundleInterfacePortService.GetByIDAsync(id.Value);

            if (bundleIfacePort == null)
            {
                return NotFound();
            }

            await PopulateBundleInterfaceItem(bundleIfacePort.BundleInterfaceID);
            await PopulatePortsDropDownList(bundleIfacePort.BundleInterfaceID);
            return View(Mapper.Map<BundleInterfacePortViewModel>(bundleIfacePort));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("BundleInterfacePortID,BundleInterfaceID,PortID,RowVersion")] BundleInterfacePortViewModel bundleIfacePort)
        {
            if (id != bundleIfacePort.BundleInterfacePortID)
            {
                return NotFound();
            }

            var dbResult = await BundleInterfacePortService.UnitOfWork.BundleInterfacePortRepository.GetAsync(filter: d => d.BundleInterfacePortID == id,
               AsTrackable: false);
            var currentBundleInterfacePort = dbResult.SingleOrDefault();

            try
            {
                if (ModelState.IsValid)
                {
                    if (currentBundleInterfacePort == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save changes. The port was deleted by another user.");
                        return View(bundleIfacePort);
                    }

                    if (!await ValidatePortBandwidth(bundleIfacePort))
                    {
                        ModelState.AddModelError(string.Empty, "The port bandwidth must match existing ports in the bundle.");
                    }
                    else
                    {
                        await BundleInterfacePortService.UpdateAsync(Mapper.Map<BundleInterfacePort>(bundleIfacePort));
                        return RedirectToAction("GetAllByBundleInterfaceID", new { id = bundleIfacePort.BundleInterfaceID });
                    }
                }
            }

            catch (DbUpdateConcurrencyException ex)
            {
                var exceptionEntry = ex.Entries.Single();

                var proposedPortID = (int)exceptionEntry.Property("PortID").CurrentValue;
                if (currentBundleInterfacePort.PortID != proposedPortID)
                {
                    ModelState.AddModelError("PortID", $"Current value: {currentBundleInterfacePort.Port.Type}{currentBundleInterfacePort.Port.Name}");
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

            await PopulateBundleInterfaceItem(currentBundleInterfacePort.BundleInterfaceID);
            await PopulatePortsDropDownList(currentBundleInterfacePort.BundleInterfaceID);
            return View(Mapper.Map<BundleInterfacePortViewModel>(currentBundleInterfacePort));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbResult = await BundleInterfacePortService.UnitOfWork.BundleInterfacePortRepository.GetAsync(q => q.BundleInterfacePortID == id.Value,
                includeProperties:"Port");
            var bundleIfacePort = dbResult.SingleOrDefault();

            if (bundleIfacePort == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("GetAllByBundleInterfaceID", new { id = bundleIfacePort.BundleInterfaceID });
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

            await PopulateBundleInterfaceItem(bundleIfacePort.BundleInterfaceID);
            return View(Mapper.Map<BundleInterfacePortViewModel>(bundleIfacePort));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(BundleInterfacePortViewModel bundleIfacePort)
        {
            try
            {
                var dbResult = await BundleInterfacePortService.UnitOfWork.BundleInterfacePortRepository.GetAsync(filter: d => d.BundleInterfacePortID == bundleIfacePort.BundleInterfacePortID,
                    AsTrackable: false);
                var currentBundleInterfacePort = dbResult.SingleOrDefault();

                if (currentBundleInterfacePort != null)
                {
                    await BundleInterfacePortService.DeleteAsync(Mapper.Map<BundleInterfacePort>(bundleIfacePort));
                }
                return RedirectToAction("GetAllByBundleInterfaceID", new { id = bundleIfacePort.BundleInterfaceID });
            }

            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { concurrencyError = true, id = bundleIfacePort.BundleInterfacePortID });
            }
        }
        private async Task PopulateBundleInterfaceItem(int bundleInterfaceID)
        {
            var bundleInterface = await BundleInterfacePortService.UnitOfWork.BundleInterfaceRepository.GetByIDAsync(bundleInterfaceID);
            ViewBag.BundleInterface = bundleInterface;
        }

        private async Task PopulatePortsDropDownList(int bundleIfaceID, object selectedPort = null)
        {
            var dbResult = await BundleInterfacePortService.UnitOfWork.BundleInterfaceRepository.GetAsync(q => q.BundleInterfaceID == bundleIfaceID, includeProperties: "Device.Ports.Interface,Device.Ports.BundleInterfacePort");
            var bundleIface = dbResult.SingleOrDefault();
            if (bundleIface != null)
            {
                // Get only those Ports which are registered to the same tenant as the bundle interface,
                // and do not have interfaces, and are not already associated with a bundle interface and are not a member of the 
                // current bundle.  
                // These ports can become members of the bundle

                var ports = from p in bundleIface.Device.Ports.Where(q => q.TenantID == bundleIface.TenantID 
                            && q.Interface == null 
                            && (q.BundleInterfacePort == null || q.BundleInterfacePort.BundleInterfaceID == bundleIfaceID))
                            select new { ID = p.ID, Name = string.Concat(p.Type, p.Name) };

                ViewBag.PortID = new SelectList(ports, "ID", "Name", selectedPort);
            }
        }

        /// <summary>
        /// Validates that the port bandwidth of a new port to be added to a bundle interface
        /// matches the port bandwidth of any existing ports.
        /// </summary>
        /// <param name="port"></param>
        /// <param name="bundleIfaceID"></param>
        /// <returns></returns>
        private async Task<Boolean> ValidatePortBandwidth(BundleInterfacePortViewModel bundleIfacePort)
        {
            var dbResult = await BundleInterfacePortService.UnitOfWork.BundleInterfaceRepository.GetAsync(q => q.BundleInterfaceID == bundleIfacePort.BundleInterfaceID,
                includeProperties: "BundleInterfacePorts.Port.PortBandwidth");
            var bundleIface = dbResult.SingleOrDefault();
            if (bundleIface == null)
            {
                return false;
            }

            var port = await BundleInterfacePortService.UnitOfWork.PortRepository.GetByIDAsync(bundleIfacePort.PortID);
            if (port == null)
            {
                return false;
            }

            var bundlePorts = bundleIface.BundleInterfacePorts;
            if (bundlePorts.Count > 0)
            {
                var portBandwidth = bundlePorts.First().Port.PortBandwidth;
                if (port.PortBandwidthID != portBandwidth.PortBandwidthID)
                {
                    return false;
                }
            }
            return true;
        }
    }
}

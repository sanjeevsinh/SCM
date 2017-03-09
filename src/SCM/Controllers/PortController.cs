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
    public class PortController : BaseViewController
    {
        public PortController(IPortService portService, IMapper mapper)
        {
            PortService = portService;
            Mapper = mapper;
        }
        private IPortService PortService { get; set; }
        private IMapper Mapper { get; set; }

        [HttpGet]
        public async Task<IActionResult> GetAllByDeviceID(int id)
        {
            var device = await PortService.UnitOfWork.DeviceRepository.GetByIDAsync(id);
            if (device == null)
            {
                return NotFound();
            }

            var ports = await PortService.UnitOfWork.PortRepository.GetAsync(q => q.DeviceID == id, 
                includeProperties:"Tenant");
            ViewBag.Device = device;
            return View(Mapper.Map<List<PortViewModel>>(ports));
        }

        [HttpGet]
        public async Task<IActionResult> GetByID(int id)
        {
            var item = await PortService.GetByIDAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(Mapper.Map<PortViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbResult = await PortService.UnitOfWork.PortRepository.GetAsync(q => q.ID == id.Value,
                includeProperties: "PortBandwidth,Tenant");
            var item = dbResult.SingleOrDefault();

            if (item == null)
            {
                return NotFound();
            }

            await PopulateDeviceItem(item.DeviceID);
            return View(Mapper.Map<PortViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? id) 
        {
            if (id == null)
            {
                return NotFound();
            }

            await PopulatePortBandwidthsDropDownList();
            await PopulateTenantsDropDownList();
            await PopulateDeviceItem(id.Value);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DeviceID,Type,Name,TenantID,PortBandwidthID")] PortViewModel port)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await PortService.AddAsync(Mapper.Map<Port>(port));
                    return RedirectToAction("GetAllByDeviceID", new { id = port.DeviceID });
                }
            }
            catch (DbUpdateException /** ex **/ )
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            await PopulatePortBandwidthsDropDownList();
            await PopulateTenantsDropDownList();
            await PopulateDeviceItem(port.DeviceID);
            return View(port);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Port port = await PortService.GetByIDAsync(id.Value);

            if (port == null)
            {
                return NotFound();
            }

            await PopulatePortBandwidthsDropDownList();
            await PopulateTenantsDropDownList();
            await PopulateDeviceItem(port.DeviceID);
            return View(Mapper.Map<PortViewModel>(port));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("ID,Type,Name,DeviceID,TenantID,PortBandwidthID,RowVersion")] PortViewModel port)
        {
            if (id != port.ID)
            {
                return NotFound();
            }

            var dbResult = await PortService.UnitOfWork.PortRepository.GetAsync(filter: d => d.ID == id,
               includeProperties:"PortBandwidth,Tenant",
               AsTrackable: false);
            var currentPort = dbResult.SingleOrDefault();

            try
            {
                if (ModelState.IsValid)
                {
                    if (currentPort == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save changes. The port was deleted by another user.");
                        return View(port);
                    }

                    var p = Mapper.Map<Port>(port);
                    if (!await ValidatePortBandwidth(p, currentPort))
                    {
                        ModelState.AddModelError(string.Empty, "The port bandwidth cannot be changed because the port is a member of a bundle.");
                        return View(port);
                    }

                    await PortService.UpdateAsync(p);
                    return RedirectToAction("GetAllByDeviceID", new { id = port.DeviceID });
                }
            }

            catch (DbUpdateConcurrencyException ex)
            {
                var exceptionEntry = ex.Entries.Single();

                var proposedName = (string)exceptionEntry.Property("Name").CurrentValue;
                if (currentPort.Name != proposedName)
                {
                    ModelState.AddModelError("Name", $"Current value: {currentPort.Name}");
                }

                var proposedType = (string)exceptionEntry.Property("Type").CurrentValue;
                if (currentPort.Type != proposedType)
                {
                    ModelState.AddModelError("Type", $"Current value: {currentPort.Type}");
                }

                var proposedPortBandwidthID = (int)exceptionEntry.Property("PortBandwidthID").CurrentValue;
                if (currentPort.PortBandwidthID != proposedPortBandwidthID)
                {
                    ModelState.AddModelError("PortBandwidthID", $"Current value: {currentPort.PortBandwidth.BandwidthGbps}");
                }

                var proposedTenantID = (int)exceptionEntry.Property("TenantID").CurrentValue;
                if (currentPort.TenantID != proposedTenantID)
                {
                    ModelState.AddModelError("TenantID", $"Current value: {currentPort.Tenant.Name}");
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

            await PopulatePortBandwidthsDropDownList();
            await PopulateTenantsDropDownList();
            await PopulateDeviceItem(currentPort.DeviceID);
            return View(Mapper.Map<PortViewModel>(currentPort));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var port = await PortService.GetByIDAsync(id.Value);
            if (port == null)
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
                    + "click the Back to Device hyperlink.";
            }

            await PopulateDeviceItem(port.DeviceID);
            return View(Mapper.Map<PortViewModel>(port));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(PortViewModel port)
        {
            try
            {
                var dbResult = await PortService.UnitOfWork.PortRepository.GetAsync(filter: d => d.ID == port.ID, AsTrackable: false);
                var currentPort = dbResult.SingleOrDefault();

                if (currentPort != null)
                {
                    var validationResult = PortService.ValidateDelete(currentPort);
                    if (!validationResult.IsSuccess)
                    {
                        ViewData["ErrorMessage"] = validationResult.GetHtmlListMessage();

                        return View(Mapper.Map<PortViewModel>(currentPort));
                    }

                    await PortService.DeleteAsync(Mapper.Map<Port>(port));
                }
                return RedirectToAction("GetAllByDeviceID", new { id = port.DeviceID });
            }

            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { concurrencyError = true, id = port.ID });
            }
        }
        private async Task PopulateDeviceItem(int deviceID)
        {
            var device = await PortService.UnitOfWork.DeviceRepository.GetByIDAsync(deviceID);
            ViewBag.Device = device;
        }

        private async Task PopulatePortBandwidthsDropDownList(object selectedPortBandwidth = null)
        {
            var portBandwidths = await PortService.UnitOfWork.PortBandwidthRepository.GetAsync();
            ViewBag.PortBandwidthID = new SelectList(portBandwidths, "PortBandwidthID", "BandwidthGbps", selectedPortBandwidth);
        }
        private async Task PopulateTenantsDropDownList(object selectedTenant = null)
        {
            var tenants = await PortService.UnitOfWork.TenantRepository.GetAsync();
            ViewBag.TenantID = new SelectList(tenants, "TenantID", "Name", selectedTenant);
        }

        /// <summary>
        /// Checks if the bandwidth of the port is being updated from the current bandwidth.
        /// If it is then check if the port belongs to a bundle. If it does then return false
        /// (the port bandwidth should not be updated).
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        private async Task<bool> ValidatePortBandwidth(Port port, Port currentPort)
        {
            if (currentPort.PortBandwidthID == port.PortBandwidthID)
            {
                return true;
            }

            var dbResult = await PortService.UnitOfWork.BundleInterfacePortRepository.GetAsync(q => q.PortID == port.ID);
            if (dbResult.Count > 0)
            {
                return false;
            }
            return true;
        }
    }
}

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
    public class VrfController : BaseViewController
    {
        public VrfController(IVrfService vrfService, IMapper mapper)
        {
            VrfService = vrfService;
            Mapper = mapper;
        }
        private IVrfService VrfService { get; set; }
        private IMapper Mapper { get; set; }

        [HttpGet]
        public async Task<IActionResult> GetAllByDeviceID(int id)
        {
            var device = await VrfService.UnitOfWork.DeviceRepository.GetByIDAsync(id);
            if (device == null)
            {
                return NotFound();
            }

            var vrfs = await VrfService.UnitOfWork.VrfRepository.GetAsync(q => q.DeviceID == id, 
                includeProperties:"Tenant");
            ViewBag.Device = device;

            return View(Mapper.Map<List<VrfViewModel>>(vrfs));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllByTenantID(int id)
        {
            var tenant = await VrfService.UnitOfWork.TenantRepository.GetByIDAsync(id);
            if (tenant == null)
            {
                return NotFound();
            }

            var vrfs = await VrfService.UnitOfWork.VrfRepository.GetAsync(q => q.Tenant.TenantID == id,
                includeProperties: "Device.Location.SubRegion,Device.Plane");
            ViewBag.Tenant = tenant;

            return View(Mapper.Map<List<VrfViewModel>>(vrfs));
        }

        [HttpGet]
        public async Task<IActionResult> GetByID(int id)
        {
            var item = await VrfService.GetByIDAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(Mapper.Map<VrfViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbResult = await VrfService.UnitOfWork.VrfRepository.GetAsync(q => q.VrfID == id.Value,
                includeProperties: "Tenant");
            var item = dbResult.SingleOrDefault();

            if (item == null)
            {
                return NotFound();
            }

            await PopulateDeviceItem(item.DeviceID);
            return View(Mapper.Map<VrfViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? id) 
        {
            if (id == null)
            {
                return NotFound();
            }

            await PopulateTenantsDropDownList();
            await PopulateDeviceItem(id.Value);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DeviceID,Name,AdministratorSubField,AssignedNumberSubField,TenantID")] VrfViewModel vrf)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await VrfService.AddAsync(Mapper.Map<Vrf>(vrf));
                    return RedirectToAction("GetAllByDeviceID", new { id = vrf.DeviceID });
                }
            }
            catch (DbUpdateException /** ex **/ )
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            await PopulateTenantsDropDownList();
            await PopulateDeviceItem(vrf.DeviceID);
            return View(vrf);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Vrf vrf = await VrfService.GetByIDAsync(id.Value);

            if (vrf == null)
            {
                return NotFound();
            }

            await PopulateTenantsDropDownList();
            await PopulateDeviceItem(vrf.DeviceID);
            return View(Mapper.Map<VrfViewModel>(vrf));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("VrfID,Name,DeviceID,TenantID,AdministratorSubField,AssignedNumberSubField,RowVersion")] VrfViewModel vrf)
        {
            if (id != vrf.VrfID)
            {
                return NotFound();
            }

            var dbResult = await VrfService.UnitOfWork.VrfRepository.GetAsync(filter: d => d.VrfID == id,
               includeProperties:"Device,Tenant",
               AsTrackable: false);
            var currentVrf = dbResult.SingleOrDefault();

            try
            {
                if (ModelState.IsValid)
                {
                    if (currentVrf == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save changes. The vrf was deleted by another user.");
                        return View(vrf);
                    }

                    await VrfService.UpdateAsync(Mapper.Map<Vrf>(vrf));
                    return RedirectToAction("GetAllByDeviceID", new { id = vrf.DeviceID });
                }
            }

            catch (DbUpdateConcurrencyException ex)
            {
                var exceptionEntry = ex.Entries.Single();

                var proposedName = (string)exceptionEntry.Property("Name").CurrentValue;
                if (currentVrf.Name != proposedName)
                {
                    ModelState.AddModelError("Name", $"Current value: {currentVrf.Name}");
                }

                var proposedAdministratorSubField = (int)exceptionEntry.Property("AdministratorSubField").CurrentValue;
                if (currentVrf.AdministratorSubField != proposedAdministratorSubField)
                {
                    ModelState.AddModelError("AdministratorSubField", $"Current value: {currentVrf.AdministratorSubField}");
                }

                var proposedAssignedNumberSubField = (int)exceptionEntry.Property("AssignedNumberSubField").CurrentValue;
                if (currentVrf.AssignedNumberSubField != proposedAssignedNumberSubField)
                {
                    ModelState.AddModelError("AssignedNumberSubField", $"Current value: {currentVrf.AssignedNumberSubField}");
                }

                var proposedTenantID = (int)exceptionEntry.Property("TenantID").CurrentValue;
                if (currentVrf.TenantID != proposedTenantID)
                {
                    ModelState.AddModelError("TenantID", $"Current value: {currentVrf.Tenant.Name}");
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

            await PopulateTenantsDropDownList();
            await PopulateDeviceItem(currentVrf.DeviceID);
            return View(Mapper.Map<VrfViewModel>(currentVrf));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vrf = await VrfService.GetByIDAsync(id.Value);
            if (vrf == null)
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

            await PopulateDeviceItem(vrf.DeviceID);
            return View(Mapper.Map<VrfViewModel>(vrf));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(VrfViewModel vrf)
        {
            try
            {
                var dbResult = await VrfService.UnitOfWork.VrfRepository.GetAsync(filter: d => d.VrfID == vrf.VrfID, AsTrackable: false);
                var currentVrf = dbResult.SingleOrDefault();

                if (currentVrf != null)
                {
                    await VrfService.DeleteAsync(Mapper.Map<Vrf>(vrf));
                }
                return RedirectToAction("GetAllByDeviceID", new { id = vrf.DeviceID });
            }

            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { concurrencyError = true, id = vrf.VrfID });
            }
        }
        private async Task PopulateDeviceItem(int deviceID)
        {
            var device = await VrfService.UnitOfWork.DeviceRepository.GetByIDAsync(deviceID);
            ViewBag.Device = device;
        }

        private async Task PopulateTenantsDropDownList(object selectedTenant = null)
        {
            var tenants = await VrfService.UnitOfWork.TenantRepository.GetAsync();
            ViewBag.TenantID = new SelectList(tenants, "TenantID", "Name", selectedTenant);
        }
    }
}

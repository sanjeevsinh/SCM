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

namespace SCM.Controllers
{
    public class TenantController : BaseViewController
    {
        public TenantController(ITenantService tenantService, IMapper mapper)
        {
           TenantService = tenantService;
           Mapper = mapper;
        }
        private ITenantService TenantService { get; set; }
        private IMapper Mapper { get; set; }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tenants = await TenantService.GetAllAsync();
            return View(Mapper.Map<List<TenantViewModel>>(tenants));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await TenantService.GetByIDAsync(id.Value);
            if (item == null)
            {
                return NotFound();
            }
            return View(Mapper.Map<TenantViewModel>(item));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] TenantViewModel tenant)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await TenantService.AddAsync(Mapper.Map<Tenant>(tenant));
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
            return View(Mapper.Map<TenantViewModel>(tenant));
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Tenant tenant = await TenantService.GetByIDAsync(id.Value);

            if (tenant == null)
            {
                return NotFound();
            }

            return View(Mapper.Map<TenantViewModel>(tenant));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("TenantID,Name,RowVersion")] TenantViewModel tenant)
        {
            if (id != tenant.TenantID)
            {
                return NotFound();
            }

            var dbResult = await TenantService.UnitOfWork.TenantRepository.GetAsync(filter: d => d.TenantID == id,
                AsTrackable: false);
            var currentTenant = dbResult.SingleOrDefault();

            try
            {
                if (ModelState.IsValid)
                {
                    if (currentTenant == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save changes. The tenant was deleted by another user.");
                        return View(tenant);
                    }

                    await TenantService.UpdateAsync(Mapper.Map<Tenant>(tenant));
                    return RedirectToAction("GetAll");
                }
            }

            catch (DbUpdateConcurrencyException ex)
            {
                var exceptionEntry = ex.Entries.Single();

                var proposedName = (string)exceptionEntry.Property("Name").CurrentValue;
                if (currentTenant.Name != proposedName)
                {
                    ModelState.AddModelError("Name", $"Current value: {currentTenant.Name}");
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

            return View(Mapper.Map<TenantViewModel>(currentTenant));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tenant = await TenantService.GetByIDAsync(id.Value);
            if (tenant == null)
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

            return View(Mapper.Map<TenantViewModel>(tenant));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(TenantViewModel tenant)
        {
            try
            {
                var dbResult = await TenantService.UnitOfWork.TenantRepository.GetAsync(filter: d => d.TenantID == tenant.TenantID, AsTrackable: false);
                var currentTenant = dbResult.SingleOrDefault();

                if (currentTenant != null)
                {
                    await TenantService.DeleteAsync(Mapper.Map<Tenant>(tenant));
                }
                return RedirectToAction("GetAll");
            }

            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { concurrencyError = true, id = tenant.TenantID });
            }
        }
    }
}

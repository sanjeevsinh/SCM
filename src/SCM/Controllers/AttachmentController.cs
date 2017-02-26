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
    public class AttachmentController : BaseViewController
    {
        public AttachmentController(IAttachmentService attachmentService, IMapper mapper)
        {
            AttachmentService = attachmentService;
            Mapper = mapper;
        }
        private IAttachmentService AttachmentService { get; set; }
        private IMapper Mapper { get; set; }

      
        [HttpGet]
        public async Task<IActionResult> GetByTenantID(int id)
        {
            var tenant = await AttachmentService.UnitOfWork.TenantRepository.GetByIDAsync(id);
            if (tenant == null)
            {
                return NotFound();
            }

            var attachments = await AttachmentService.GetByTenantAsync(tenant);

            return View(Mapper.Map<TenantAttachmentsViewModel>(attachments));
        }

        [HttpGet]
        public async Task<IActionResult> AttachmentInterfaceDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await AttachmentService.GetAttachmentInterfaceByIDAsync(id.Value);
            if (item == null)
            {
                return NotFound();
            }

            return View(Mapper.Map<AttachmentInterfaceViewModel>(item));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DeviceID,VrfName,VrfAdministratorSubField,VrfAssignedNumberSubField,TenantID")] AttachmentInterfaceViewModel attachment)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await AttachmentService.AddAsync(Mapper.Map<AttachmentRequest>(attachment));
                    return RedirectToAction("GetByTenantID", new { id = attachment.TenantID });
                }
            }
            catch (DbUpdateException /** ex **/ )
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            return View(attachment);
        }

        [HttpGet]
        public async Task<PartialViewResult> SubRegions(int id)
        {
            var subRegions = await AttachmentService.UnitOfWork.SubRegionRepository.GetAsync(q => q.RegionID == id);
            return PartialView(Mapper.Map<List<SubRegionViewModel>>(subRegions));
        }

        [HttpGet]
        public async Task<PartialViewResult> Locations(int id)
        {
            var locations = await AttachmentService.UnitOfWork.LocationRepository.GetAsync(q => q.SubRegionID == id);
            return PartialView(Mapper.Map<List<LocationViewModel>>(locations));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteAttachmentInterface(AttachmentInterfaceViewModel attachment, bool? concurrencyError = false)
        {

            var currentAttachment = await AttachmentService.GetAttachmentInterfaceByIDAsync(attachment.ID);
            if (currentAttachment == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("GetByTenantID", new { id = attachment.TenantID });
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

            return View(Mapper.Map<AttachmentInterfaceViewModel>(attachment));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAttachmentInterface(AttachmentInterfaceViewModel attachment)
        {
            try
            {
                var currentAttachment = await AttachmentService.GetAttachmentInterfaceByIDAsync(attachment.ID);

                if (currentAttachment != null)
                {
                    await AttachmentService.DeleteAttachmentInterfaceAsync(Mapper.Map<AttachmentInterface>(attachment));
                }
                return RedirectToAction("GetByTenantID", new { id = attachment.TenantID });
            }

            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { concurrencyError = true, attachment = attachment });
            }
        }
        private async Task PopulateDeviceItem(int deviceID)
        {
            var device = await AttachmentService.UnitOfWork.DeviceRepository.GetByIDAsync(deviceID);
            ViewBag.Device = device;
        }

        private async Task PopulateTenantsDropDownList(object selectedTenant = null)
        {
            var tenants = await AttachmentService.UnitOfWork.TenantRepository.GetAsync();
            ViewBag.TenantID = new SelectList(tenants, "TenantID", "Name", selectedTenant);
        }
        private async Task PopulateTenantItem(int tenantID)
        {
            var tenant = await AttachmentService.UnitOfWork.TenantRepository.GetByIDAsync(tenantID);
            ViewBag.Tenant = tenant;
        }

        private async Task PopulateRegionsDropDownList(object selectedRegion = null)
        {
            var regions = await AttachmentService.UnitOfWork.RegionRepository.GetAsync();
            ViewBag.RegionID = new SelectList(regions, "RegionID", "Name", selectedRegion);
        }
    }
}

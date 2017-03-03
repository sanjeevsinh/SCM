﻿using System;
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
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await AttachmentService.GetByIDAsync(id.Value);
            if (item == null)
            {
                return NotFound();
            }

            return View(Mapper.Map<AttachmentInterfaceViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await PopulateTenantItem(id.Value);
            await PopulatePlanesDropDownList();
            await PopulateRegionsDropDownList();
            await PopulateBandwidthsDropDownList();

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VrfName,VrfAdministratorSubField,VrfAssignedNumberSubField,TenantID," +
            "IpAddress,SubnetMask,BandwidthID,RegionID,SubRegionID,LocationID,PlaneID,IsLayer3,IsTagged")] AttachmentRequestViewModel request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await AttachmentService.AddAsync(Mapper.Map<AttachmentRequest>(request));
                    if (!result.IsSuccess)
                    {
                        ModelState.AddModelError(string.Empty, result.GetMessage());
                    }
                    else
                    {
                        return RedirectToAction("GetByTenantID", "TenantAttachments", new { id = request.TenantID });
                    }
                }
            }
            catch (DbUpdateException /** ex **/ )
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            await PopulateTenantItem(request.TenantID);
            await PopulatePlanesDropDownList();
            await PopulateRegionsDropDownList(request.RegionID);
            await PopulateSubRegionsDropDownList(request.RegionID, request.SubRegionID);
            await PopulateLocationsDropDownList(request.SubRegionID, request.LocationID);
            await PopulateBandwidthsDropDownList();
            return View(request);
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
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentAttachment = await AttachmentService.GetByIDAsync(id.Value);
            if (currentAttachment == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("GetByTenantID", "TenantAttachments", new { id = Request.Query["TenantID"] });
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

            await PopulateTenantItem(currentAttachment.TenantID);
            return View(Mapper.Map<AttachmentInterfaceViewModel>(currentAttachment));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(AttachmentInterfaceViewModel attachment)
        {
            try
            {
                var currentAttachment = await AttachmentService.GetByIDAsync(attachment.ID);

                if (currentAttachment != null)
                {
                    var result = await AttachmentService.DeleteAsync(currentAttachment);
                    if (!result.IsSuccess)
                    {
                        ViewData["DeleteErrorMessage"] = result.GetMessage();
                        await PopulateTenantItem(currentAttachment.TenantID);

                        return View(Mapper.Map<AttachmentInterfaceViewModel>(currentAttachment));
                    }
                }

                return RedirectToAction("GetByTenantID", "TenantAttachments", new { id = attachment.TenantID });
            }

            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { concurrencyError = true, attachment = attachment });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CheckSync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var checkSyncResult = await AttachmentService.CheckNetworkSyncAsync(id.Value);
            if (checkSyncResult.InSync)
            {
                ViewData["SyncSuccessMessage"] = "The Attachment Interface is synchronised with the network.";
            }
            else
            {
                if (!checkSyncResult.NetworkSyncServiceResult.IsSuccess)
                {
                    ViewData["SyncErrorMessage"] = checkSyncResult.NetworkSyncServiceResult.GetAllMessages();
                }
                else
                {
                    ViewData["SyncErrorMessage"] = "The Attachment Interface is not synchronised with the network. Press the 'Sync' button to update the network.";
                }
            }

            var item = await AttachmentService.GetByIDAsync(id.Value);
            if (item == null)
            {
                return NotFound();
            }

            return View("Details", Mapper.Map<AttachmentInterfaceViewModel>(item));
        }

        [HttpPost]
        public async Task<IActionResult> Sync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await AttachmentService.GetByIDAsync(id.Value);
            if (item == null)
            {
                return NotFound();
            }

            var syncResult = await AttachmentService.SyncToNetworkAsync(id.Value);

            if (syncResult.IsSuccess)
            {
                ViewData["SyncSuccessMessage"] = "The network is synchronised.";
            }
            else
            {
                ViewData["SyncErrorMessage"] = syncResult.GetMessage();
            }

            return View("Details", Mapper.Map<AttachmentInterfaceViewModel>(item));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFromNetwork(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attachment = await AttachmentService.GetByIDAsync(id.Value);

            if (attachment == null)
            {

                ViewData["AttachmentDeletedMessage"] = "The Attachment has been deleted by another user. Return to the list.";

                return View("AttachmentDeleted", new { TenantID = Request.Query["TenantID"] });
            }

            var syncResult = await AttachmentService.DeleteFromNetworkAsync(id.Value);
            if (syncResult.IsSuccess)
            {
                ViewData["SyncSuccessMessage"] = "The Attachment has been deleted from the network.";
            }
            else
            {
                if (syncResult.NetworkHttpResponse.HttpStatusCode == HttpStatusCode.NotFound)
                {
                    ViewData["SyncErrorMessage"] = "The Attachment has already been deleted from the network.";
                }
                else
                {
                    ViewData["SyncErrorMessage"] = "There was a problem deleting the Attachment from the network. " + syncResult.GetAllMessages();
                }
            }

            await PopulateTenantItem(attachment.TenantID);
            return View("Delete", Mapper.Map<AttachmentInterfaceViewModel>(attachment));
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

        private async Task PopulateSubRegionsDropDownList(int regionID, object selectedSubRegion = null)
        {
            var subRegions = await AttachmentService.UnitOfWork.SubRegionRepository.GetAsync(q => q.RegionID == regionID);
            ViewBag.SubRegionID = new SelectList(subRegions, "SubRegionID", "Name", selectedSubRegion);
        }

        private async Task PopulateLocationsDropDownList(int subRegionID, object selectedLocation = null)
        {
            var locations = await AttachmentService.UnitOfWork.LocationRepository.GetAsync(q => q.SubRegionID == subRegionID);
            ViewBag.LocationID = new SelectList(locations, "LocationID", "SiteName", selectedLocation);
        }

        private async Task PopulatePlanesDropDownList(object selectedPlane = null)
        {
            var planes = await AttachmentService.UnitOfWork.PlaneRepository.GetAsync();
            ViewBag.PlaneID = new SelectList(planes, "PlaneID", "Name", selectedPlane);
        }
        private async Task PopulateBandwidthsDropDownList(object selectedBandwidth = null)
        {
            var bandwidths = await AttachmentService.UnitOfWork.InterfaceBandwidthRepository.GetAsync();
            ViewBag.BandwidthID = new SelectList(bandwidths, "InterfaceBandwidthID", "BandwidthGbps", selectedBandwidth);
        }
    }
}
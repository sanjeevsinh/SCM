﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SCM.Models;
using SCM.Models.ServiceModels;
using SCM.Models.ViewModels;
using SCM.Services;
using SCM.Services.SCMServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.Linq.Expressions;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using SCM.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace SCM.Controllers
{
    public class AttachmentController : BaseViewController
    {
        public AttachmentController(IAttachmentService attachmentService, 
            IVrfService vrfService,
            ITenantService tenantService,
            IMapper mapper,
            IConnectionManager signalRConnectionManager)
        {
            AttachmentService = attachmentService;
            VrfService = vrfService;
            TenantService = tenantService;
            Mapper = mapper;
            HubContext = signalRConnectionManager.GetHubContext<NetworkSyncHub>();
        }

        private IAttachmentService AttachmentService { get; set; }
        private IVrfService VrfService { get; set; }
        private ITenantService TenantService { get; set; }
        private IMapper Mapper { get; set; }
        private IHubContext HubContext { get; set; }

        [HttpGet]
        public async Task<IActionResult> GetAllByTenantID(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tenant = await TenantService.GetByIDAsync(id.Value);
            if (tenant == null)
            {
                return NotFound();
            }

            var attachments = await AttachmentService.GetAllByTenantAsync(tenant);
            var successMessage = string.Empty;
            attachments.Where(q => q.Created).ToList().ForEach(q => successMessage += $"{q.Name} has been created.");

            var checkSyncResult = AttachmentService.ShallowCheckNetworkSync(attachments);
            if (checkSyncResult.IsSuccess)
            {
                successMessage += "All attachments appear to be synchronised with the network.";
            }
            else
            {
                ViewData["ErrorMessage"] = FormatAsHtmlList(checkSyncResult.GetMessage());
            }

            ViewData["SuccessMessage"] = FormatAsHtmlList(successMessage);
            ViewBag.Tenant = tenant;

            return View(Mapper.Map<List<AttachmentViewModel>>(attachments));
        }

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

            return View(Mapper.Map<AttachmentViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> GetBundleInterfaceMembers(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attachment = await AttachmentService.GetByIDAsync(id.Value);
            ViewBag.Attachment = Mapper.Map<AttachmentViewModel>(attachment);
            var ports = attachment.Interfaces.SelectMany(q => q.Ports).ToList();

            return View(Mapper.Map<List<BundleInterfaceMemberViewModel>>(ports));
        }

        [HttpGet]
        public async Task<IActionResult> GetMultiPortMembers(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attachment = await AttachmentService.GetByIDAsync(id.Value);
            ViewBag.Attachment = Mapper.Map<AttachmentViewModel>(attachment);

            var ifaces = attachment.Interfaces;

            return View(Mapper.Map<List<MultiPortMemberViewModel>>(ifaces));
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tenant = await TenantService.GetByIDAsync(id.Value);
            if (tenant == null)
            {
                return NotFound();
            }

            ViewBag.Tenant = Mapper.Map<TenantViewModel>(tenant);

            await PopulatePlanesDropDownList();
            await PopulateRegionsDropDownList();
            await PopulateBandwidthsDropDownList();
            await PopulateContractBandwidthsDropDownList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenantID,IpAddress1,SubnetMask1,IpAddress2,SubnetMask2,"
            + "IpAddress3,SubnetMask3,IpAddress4,SubnetMask4,BandwidthID,RegionID,SubRegionID,LocationID,PlaneID,"
            + "IsLayer3,IsTagged,BundleRequired,MultiPortRequired,ContractBandwidthID,TrustReceivedCosDscp")] AttachmentRequestViewModel request)
        {

            if (ModelState.IsValid)
            {
                var mappedRequest = Mapper.Map<AttachmentRequest>(request);
                var validationResult = await AttachmentService.ValidateNewAsync(mappedRequest);

                if (!validationResult.IsSuccess)
                {
                    validationResult.GetMessageList().ForEach(message => ModelState.AddModelError(string.Empty, message));
                }
                else
                {
                    var result = await AttachmentService.AddAsync(mappedRequest);
                    if (!result.IsSuccess)
                    {
                        result.GetMessageList().ForEach(message => ModelState.AddModelError(string.Empty, message));
                    }
                    else
                    {
                        return RedirectToAction("GetAllByTenantID", new { id = request.TenantID });
                    }
                }
            }

            var tenant = await TenantService.GetByIDAsync(request.TenantID);
            ViewBag.Tenant = tenant;

            await PopulatePlanesDropDownList();
            await PopulateRegionsDropDownList(request.RegionID);
            await PopulateSubRegionsDropDownList(request.RegionID, request.SubRegionID);
            await PopulateLocationsDropDownList(request.SubRegionID, request.LocationID);
            await PopulateBandwidthsDropDownList();
            await PopulateContractBandwidthsDropDownList();

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

            var item = await AttachmentService.GetByIDAsync(id.Value);
            if (item == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("GetAllByTenantID", new { id = Request.Query["TenantID"] });
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

            var tenant = await TenantService.GetByIDAsync(id.Value);
            ViewBag.Tenant = tenant;

            return View(Mapper.Map<AttachmentViewModel>(item));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(AttachmentViewModel attachment)
        {
            var item = await AttachmentService.GetByIDAsync(attachment.AttachmentID);

            if (item == null)
            {
                return NotFound();
            }

            try
            {
                var validationResult = await ValidateDelete(item);

                if (validationResult)
                {
                    // Delete resource from the network first

                    var syncResult = await AttachmentService.DeleteFromNetworkAsync(item);

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
                        var result = await AttachmentService.DeleteAsync(item);
                        if (!result.IsSuccess)
                        {
                            ViewData["ErrorMessage"] = FormatAsHtmlList(result.GetMessage());
                        }
                        else
                        {
                            return RedirectToAction("GetAllByTenantID", new { id = Request.Query["TenantID"] });
                        }
                    }
                }

                var tenant = await TenantService.GetByIDAsync(item.TenantID);
                ViewBag.Tenant = tenant;

                return View(Mapper.Map<AttachmentViewModel>(item));
            }

            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { concurrencyError = true, id = item.AttachmentID });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFromNetwork(AttachmentViewModel attachment)
        {
            var item = await AttachmentService.GetByIDAsync(attachment.AttachmentID);
            if (item == null)
            {
                ViewData["AttachmentDeletedMessage"] = "The attachment has been deleted by another user. Return to the list.";

                return View("AttachmentDeleted", new { TenantID = Request.Query["TenantID"] });
            }

            var validationResult = await ValidateDelete(item);
            if (validationResult)
            {
                var syncResult = await AttachmentService.DeleteFromNetworkAsync(item);
                if (syncResult.IsSuccess)
                {
                    ViewData["SuccessMessage"] = "The attachment has been deleted from the network.";
                }
                else
                {
                    ViewData["ErrorMessage"] = FormatAsHtmlList(syncResult.GetMessage());
                }
            }

            await AttachmentService.UpdateRequiresSyncAsync(item, true, true);

            var tenant = await TenantService.GetByIDAsync(item.TenantID);
            ViewBag.Tenant = tenant;

            item.RequiresSync = true;

            return View("Delete", Mapper.Map<AttachmentViewModel>(item));
        }

        /// <summary>
        /// Helper to validate that an Attachment can be deleted.
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns></returns>
        private async Task<bool> ValidateDelete(Attachment attachment)
        {
            var result = true;

            if (!attachment.IsTagged)
            {
                var vrfValidationResult = await VrfService.ValidateDeleteAsync(attachment.VrfID.Value);

                if (!vrfValidationResult.IsSuccess)
                {
                    result = false;
                    ViewData["ErrorMessage"] = FormatAsHtmlList(vrfValidationResult.GetMessage());

                    return result;

                }
            }

            // Check if the attachment has any vifs, then check if each vif has a vrf.
            // If a vrf exists, check if it participates in any services

            if (attachment.Vifs.Count > 0)
            {
                foreach (var vif in attachment.Vifs)
                {
                    var vifVrfValidationResult = await VrfService.ValidateDeleteAsync(vif.VrfID.Value);
                    if (!vifVrfValidationResult.IsSuccess)
                    {
                        result = false;
                        ViewData["ErrorMessage"] = FormatAsHtmlList(vifVrfValidationResult.GetMessage());

                        return result;
                    }
                }
            }

            return result;
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
            var bandwidths = await AttachmentService.UnitOfWork.AttachmentBandwidthRepository.GetAsync();
            ViewBag.BandwidthID = new SelectList(bandwidths.OrderBy(b => b.BandwidthGbps), "AttachmentBandwidthID", "BandwidthGbps", selectedBandwidth);
        }

        private async Task PopulateContractBandwidthsDropDownList(object selectedContractBandwidth = null)
        {
            var contractBandwidths = await AttachmentService.UnitOfWork.ContractBandwidthRepository.GetAsync();
            ViewBag.ContractBandwidthID = new SelectList(contractBandwidths.OrderBy(b => b.BandwidthMbps), "ContractBandwidthID", "BandwidthMbps", selectedContractBandwidth);
        }
    }
}
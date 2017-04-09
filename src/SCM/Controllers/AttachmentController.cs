using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SCM.Models.ServiceModels;
using SCM.Models.ViewModels;
using SCM.Services;
using SCM.Services.SCMServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.Linq.Expressions;

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
        public async Task<IActionResult> GetAllByTenantID(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tenant = await AttachmentService.UnitOfWork.TenantRepository.GetByIDAsync(id);
            if (tenant == null)
            {
                return NotFound();
            }

            var attachments = await AttachmentService.GetAllByTenantAsync(tenant);
            ViewBag.Tenant = tenant;

            return View(Mapper.Map<List<AttachmentViewModel>>(attachments));
        }

        [HttpGet]
        public async Task<IActionResult> Details(AttachmentViewModel attachment)
        {
            var item = await AttachmentService.GetByIDAsync(attachment.ID, attachment.IsMultiPort);
            if (item == null)
            {
                return NotFound();
            }

            return View(Mapper.Map<AttachmentViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> GetBundleInterfaceMemberPorts(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bundleInterfacePorts = await AttachmentService.UnitOfWork.BundleInterfacePortRepository.GetAsync(q => q.InterfaceID == id.Value,
                includeProperties:"Port.Device", AsTrackable:false);
            ViewBag.Attachment = await AttachmentService.GetByIDAsync(id.Value);
           
            return View(Mapper.Map<List<BundleInterfacePortViewModel>>(bundleInterfacePorts));
        }

        [HttpGet]
        public async Task<IActionResult> GetMultiPortMemberPorts(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var multiPortMemberPorts = await AttachmentService.UnitOfWork.PortRepository.GetAsync(q => q.MultiPortID == id.Value,
                includeProperties: "Device", AsTrackable: false);
            ViewBag.Attachment = await AttachmentService.GetByIDAsync(id.Value, multiPort: true);

            return View(Mapper.Map<List<PortViewModel>>(multiPortMemberPorts));
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
            await PopulateContractBandwidthPoolsDropDownList(id.Value);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenantID,IpAddress1,SubnetMask1,IpAddress2,SubnetMask2,"
            + "IpAddress3,SubnetMask3,IpAddress4,SubnetMask4,BandwidthID,RegionID,SubRegionID,LocationID,PlaneID," 
            + "IsLayer3,IsTagged,BundleRequired,MultiPortRequired,ContractBandwidthPoolID")] AttachmentRequestViewModel request)
        {

            if (ModelState.IsValid)
            {
                var mappedRequest = Mapper.Map<AttachmentRequest>(request);
                var bandwidth = await AttachmentService.UnitOfWork.InterfaceBandwidthRepository.GetByIDAsync(request.BandwidthID);
                mappedRequest.Bandwidth = bandwidth;

                var validationResult = await AttachmentService.ValidateAsync(mappedRequest);

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

            await PopulateTenantItem(request.TenantID);
            await PopulatePlanesDropDownList();
            await PopulateRegionsDropDownList(request.RegionID);
            await PopulateSubRegionsDropDownList(request.RegionID, request.SubRegionID);
            await PopulateLocationsDropDownList(request.SubRegionID, request.LocationID);
            await PopulateBandwidthsDropDownList();
            await PopulateContractBandwidthPoolsDropDownList(request.TenantID);
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
        public async Task<IActionResult> Delete(int? id, [FromQuery]bool isMultiPort, bool? concurrencyError = false)
        {

            var item = await AttachmentService.GetByIDAsync(id.Value, multiPort:isMultiPort);
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

            await PopulateTenantItem(item.TenantID);
            return View(Mapper.Map<AttachmentViewModel>(item));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Attachment attachment)
        {
            try
            {
                var item = await AttachmentService.GetByIDAsync(attachment.ID, multiPort:attachment.IsMultiPort);

                if (item != null)
                {
                    var result = await AttachmentService.DeleteAsync(item);
                    if (!result.IsSuccess)
                    {
                        ViewData["ErrorMessage"] = result.GetHtmlListMessage();
                        await PopulateTenantItem(item.TenantID);

                        return View(Mapper.Map<AttachmentViewModel>(item));
                    }
                }

                return RedirectToAction("GetAllByTenantID", new { id = Request.Query["TenantID"] });
            }

            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { concurrencyError = true, id = attachment.ID });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CheckSync(AttachmentViewModel attachment)
        {
            var item = await AttachmentService.GetByIDAsync(attachment.ID, attachment.IsMultiPort);

            if (item == null)
            {
                return NotFound();
            }

            var checkSyncResult = await AttachmentService.CheckNetworkSyncAsync(item);
            if (checkSyncResult.IsSuccess)
            {
                ViewData["SuccessMessage"] = "The attachment is synchronised with the network.";
                item.RequiresSync = false;
            }
            else
            {
                if (!checkSyncResult.IsSuccess)
                {
                    ViewData["ErrorMessage"] = checkSyncResult.GetMessagesAsHtmlList();
                }
                else
                {
                    ViewData["ErrorMessage"] = "The attachment is not synchronised with the network. Press the 'Sync' button to update the network.";
                }

                item.RequiresSync = true;
            }

            return View("Details", Mapper.Map<AttachmentViewModel>(item));
        }

        [HttpPost]
        public async Task<IActionResult> Sync(AttachmentViewModel attachment)
        {

            var item = await AttachmentService.GetByIDAsync(attachment.ID, attachment.IsMultiPort);
            if (item == null)
            {
                return NotFound();
            }

            var syncResult = await AttachmentService.SyncToNetworkAsync(item);

            if (syncResult.IsSuccess)
            {
                ViewData["SuccessMessage"] = "The network is synchronised.";
                item.RequiresSync = false;
            }
            else
            {
                ViewData["ErrorMessage"] = syncResult.GetMessagesAsHtmlList();
                item.RequiresSync = true;
            }

            return View("Details", Mapper.Map<AttachmentViewModel>(item));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFromNetwork(Attachment attachment)
        {
            var item = await AttachmentService.GetByIDAsync(attachment.ID, attachment.IsMultiPort);

            if (item == null)
            {
                ViewData["AttachmentDeletedMessage"] = "The attachment has been deleted by another user. Return to the list.";

                return View("AttachmentDeleted", new { TenantID = Request.Query["TenantID"] });
            }

            var syncResult = await AttachmentService.DeleteFromNetworkAsync(item);
            if (syncResult.IsSuccess)
            {
                ViewData["SuccessMessage"] = "The attachment has been deleted from the network.";
            }
            else
            {
                ViewData["ErrorMessage"] = syncResult.GetHtmlListMessage();
            }

            await PopulateTenantItem(item.TenantID);
            item.RequiresSync = true;
            return View("Delete", Mapper.Map<AttachmentViewModel>(item));
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
            ViewBag.BandwidthID = new SelectList(bandwidths.OrderBy(f => f.BandwidthGbps), "InterfaceBandwidthID", "BandwidthGbps", selectedBandwidth);
        }
        private async Task PopulateContractBandwidthPoolsDropDownList(int tenantID, object selectedContractBandwidthPool = null)
        {
            var contractBandwidthPools = await AttachmentService.UnitOfWork.ContractBandwidthPoolRepository.GetAsync(q => q.TenantID == tenantID);
            ViewBag.ContractBandwidthPoolID = new SelectList(contractBandwidthPools, "ContractBandwidthPoolID", "Name", selectedContractBandwidthPool);
        }
    }
}
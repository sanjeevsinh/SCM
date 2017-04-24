using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SCM.Models.ServiceModels;
using SCM.Models;
using SCM.Models.ViewModels;
using SCM.Services;
using SCM.Services.SCMServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;

namespace SCM.Controllers
{
    public class VifController : BaseViewController
    {
        public VifController(IVifService vifService, IAttachmentService attachmentService, IVrfService vrfService, IMapper mapper)
        {
            VifService = vifService;
            AttachmentService = attachmentService;
            VrfService = vrfService;
            Mapper = mapper;
        }
        
        private IAttachmentService AttachmentService { get; set; } 
        private IVifService VifService { get; set; }
        private IVrfService VrfService { get; set; }
        private IMapper Mapper { get; set; }

        [HttpGet]
        public async Task<IActionResult> GetByID(int id)
        {
            var item = await VifService.GetByIDAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(Mapper.Map<VifViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllByAttachmentID(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attachment = await AttachmentService.GetByIDAsync(id.Value);
            if (attachment == null)
            {
                return NotFound();
            }

            ViewBag.Attachment = attachment;
            var vifs = await VifService.GetAllByAttachmentIDAsync(id.Value);

            return View(Mapper.Map<List<VifViewModel>>(vifs));
        }

        [HttpGet]
        public async Task<IActionResult> GetMultiPortVlansByVifID(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vif = await VifService.GetByIDAsync(id.Value);
            if (vif == null)
            {
                return NotFound();
            }

            ViewBag.Vif = vif;
            var multiPortVlans = await VifService.UnitOfWork.VlanRepository.GetAsync(q => q.VifID == id.Value);

            return View(Mapper.Map<List<MultiPortVifViewModel>>(multiPortVlans));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await VifService.GetByIDAsync(id.Value);

            if (item == null)
            {
                return NotFound();
            }

            var attachment = await AttachmentService.GetByIDAsync(item.AttachmentID);
            if (attachment == null)
            {
                return NotFound();
            }
            ViewBag.Attachment = attachment;

            return View(Mapper.Map<VifViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attachment = await AttachmentService.GetByIDAsync(id.Value);
            if (attachment == null)
            {
                return NotFound();
            }
            ViewBag.Attachment = attachment;

            await PopulateTenantsDropDownList(attachment.TenantID);
            PopulateContractBandwidthPoolsDropDownList(attachment, attachment.TenantID);
            await PopulateContractBandwidthsDropDownList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AttachmentID,AttachmentIsMultiPort,IpAddress1,SubnetMask1,"
            + "IpAddress2,SubnetMask2,IpAddress3,SubnetMask3,"
            + "IpAddress4,SubnetMask4,IsLayer3,AutoAllocateVlanTag,"
            + "RequestedVlanTag,TenantID,ContractBandwidthPoolID," 
            + "ContractBandwidthID,TrustReceivedCosDscp")] VifRequestViewModel request)
        {

            if (ModelState.IsValid)
            {
                var mappedRequest = Mapper.Map<VifRequest>(request);

                var validationResult = await VifService.ValidateNewAsync(mappedRequest);

                if (!validationResult.IsSuccess)
                {
                    validationResult.GetMessageList().ForEach(message => ModelState.AddModelError(string.Empty, message));
                }
                else
                {
                    var result = await VifService.AddAsync(mappedRequest);
                    if (!result.IsSuccess)
                    {
                        result.GetMessageList().ForEach(message => ModelState.AddModelError(string.Empty, message));
                    }
                    else
                    {
                        return RedirectToAction("GetAllByAttachmentID", new { id = request.AttachmentID });
                    }
                }
            }

            var attachment = await AttachmentService.GetByIDAsync(request.AttachmentID);
            if (attachment == null)
            {
                return NotFound();
            }
            ViewBag.Attachment = attachment;

            await PopulateTenantsDropDownList(attachment.TenantID);
            PopulateContractBandwidthPoolsDropDownList(attachment, request.TenantID, request.ContractBandwidthPoolID);
            await PopulateContractBandwidthsDropDownList(request.ContractBandwidthID);

            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id, int? attachmentID, bool? concurrencyError = false)
        {
            if (id == null || attachmentID == null)
            {
                return NotFound();
            }

            var vif = await VifService.GetByIDAsync(id.Value);
            if (vif == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("GetAllByAttachmentID", new { id = attachmentID });
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

            var attachment = await AttachmentService.GetByIDAsync(attachmentID.Value);
            if (attachment == null)
            {
                return NotFound();
            }
            ViewBag.Attachment = attachment;

            return View(Mapper.Map<VifViewModel>(vif));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(VifViewModel vif)
        {
            try
            {
                var item = await VifService.GetByIDAsync(vif.VifID);

                if (item == null)
                {
                    return NotFound();
                }

                var validateVrfDelete = await VrfService.ValidateDeleteAsync(item.VrfID);
                if (!validateVrfDelete.IsSuccess)
                {
                    ViewData["ErrorMessage"] = validateVrfDelete.GetHtmlListMessage();
                }
                else
                {

                    // Delete resource from the network first

                    var syncResult = await VifService.DeleteFromNetworkAsync(item);

                    // Delete from network may return IsSuccess false if the resource was not found - this should be ignored
                    // because it probably means the resource was either previously deleted from the network or it was 
                    // never syncd to the network

                    var inSync = true;
                    if (!syncResult.IsSuccess)
                    {
                        foreach (var r in syncResult.NetworkSyncServiceResults)
                        {
                            if (r.HttpStatusCode != HttpStatusCode.NotFound)
                            {
                                // Something went wrong, so flag for exit

                                inSync = false;
                                ViewData["ErrorMessage"] = syncResult.GetHtmlListMessage();
                            }
                        }
                    }

                    if (inSync)
                    {
                        var result = await VifService.DeleteAsync(item);
                        if (!result.IsSuccess)
                        {
                            ViewData["ErrorMessage"] = result.GetHtmlListMessage();
                        }
                        else
                        {
                            return RedirectToAction("GetAllByAttachmentID", new { id = vif.AttachmentID });
                        }
                    }
                }

                var attachment = await AttachmentService.GetByIDAsync(item.AttachmentID);
                ViewBag.Attachment = attachment;

                return View(Mapper.Map<VifViewModel>(item));
            }

            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new
                {
                    concurrencyError = true,
                    id = vif.VifID,
                    attachmentID = vif.AttachmentID
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CheckSync(VifViewModel vif)
        {
            var item = await VifService.GetByIDAsync(vif.VifID);

            if (item == null)
            {
                return NotFound();
            }

            var checkSyncResult = await VifService.CheckNetworkSyncAsync(item);

            if (checkSyncResult.IsSuccess)
            {
                ViewData["SuccessMessage"] = "The vif is synchronised with the network.";
                item.RequiresSync = false;
            }
            else
            {
                ViewData["ErrorMessage"] = checkSyncResult.GetHtmlListMessage();
                item.RequiresSync = true;
            }

            var attachment = await AttachmentService.GetByIDAsync(item.AttachmentID);
            if (attachment == null)
            {
                return NotFound();
            }

            ViewBag.Attachment = attachment;

            return View("Details", Mapper.Map<VifViewModel>(item));
        }

        [HttpPost]
        public async Task<IActionResult> Sync(VifViewModel vif)
        {

            var item = await VifService.GetByIDAsync(vif.VifID);
            if (item == null)
            {
                return NotFound();
            }

            var syncResult = await VifService.SyncToNetworkAsync(item);

            if (syncResult.IsSuccess)
            {
                ViewData["SuccessMessage"] = "The network is synchronised.";
                item.RequiresSync = false;
            }
            else
            {
                ViewData["ErrorMessage"] = syncResult.GetHtmlListMessage();
                item.RequiresSync = true;
            }

            var attachment = await AttachmentService.GetByIDAsync(item.AttachmentID);
            if (attachment == null)
            {
                return NotFound();
            }
            ViewBag.Attachment = attachment;

            return View("Details", Mapper.Map<VifViewModel>(item));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFromNetwork(VifViewModel vif)
        {
            var item = await VifService.GetByIDAsync(vif.VifID);

            if (item == null)
            {
                ViewData["VifDeletedMessage"] = "The vif has been deleted by another user. Return to the list.";

                return View("VifDeleted", vif);
            }

            // Check for VPN Attachment Sets - if a VRF for the Attachment exists, which 
            // is to be deleted, and one or more VPNs are bound to the VRF, 
            // then quit and warn the user

            var validationResult = await VrfService.ValidateDeleteAsync(item.VrfID);
            if (!validationResult.IsSuccess)
            {
                ViewData["ErrorMessage"] = validationResult.GetHtmlListMessage();
            }
            else
            {
                var syncResult = await VifService.DeleteFromNetworkAsync(item);
                if (syncResult.IsSuccess)
                {
                    ViewData["SuccessMessage"] = "The vif has been deleted from the network.";
                }
                else
                {
                    ViewData["ErrorMessage"] = syncResult.GetHtmlListMessage();
                }
            }

            var attachment = await AttachmentService.GetByIDAsync(item.AttachmentID);
            ViewBag.Attachment = attachment;

            item.RequiresSync = true;

            return View("Delete", Mapper.Map<VifViewModel>(item));
        }

        [HttpGet]
        public async Task<PartialViewResult> ContractBandwidthPools(int tenantID, int attachmentID)
        {
            var attachment = await AttachmentService.GetByIDAsync(attachmentID);

            var contractBandwidthPools = attachment.Vifs.Select(q => q.ContractBandwidthPool).Where(q => q.TenantID == tenantID)
                .GroupBy(q => q.ContractBandwidthPoolID)
                .Select(group => group.First());

            return PartialView(Mapper.Map<List<ContractBandwidthPoolViewModel>>(contractBandwidthPools));
        }

        private async Task PopulateTenantsDropDownList(object selectedTenant = null)
        {
            var tenants = await VifService.UnitOfWork.TenantRepository.GetAsync();
            ViewBag.TenantID = new SelectList(tenants, "TenantID", "Name", selectedTenant);
        }

        private void PopulateContractBandwidthPoolsDropDownList(Attachment attachment, int tenantID, object selectedContractBandwidthPool = null)
        {
            var contractBandwidthPools = attachment.Vifs.Select(q => q.ContractBandwidthPool).Where(q => q.TenantID == tenantID)
                .GroupBy(q => q.ContractBandwidthPoolID)
                .Select(group => group.First());

            ViewBag.ContractBandwidthPoolID = new SelectList(contractBandwidthPools,"ContractBandwidthPoolID", "Name", selectedContractBandwidthPool);
        }

        private async Task PopulateContractBandwidthsDropDownList(object selectedContractBandwidth = null)
        {
            var contractBandwidths = await AttachmentService.UnitOfWork.ContractBandwidthRepository.GetAsync();
            ViewBag.ContractBandwidthID = new SelectList(contractBandwidths.OrderBy(b => b.BandwidthMbps), "ContractBandwidthID", "BandwidthMbps", selectedContractBandwidth);
        }
    }
}
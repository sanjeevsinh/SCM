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
using Microsoft.AspNetCore.SignalR.Infrastructure;
using SCM.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace SCM.Controllers
{
    public class VifController : BaseViewController
    {
        public VifController(IVifService vifService, 
            IAttachmentService attachmentService, 
            IVrfService vrfService, 
            IMapper mapper,
            IConnectionManager signalRConnectionManager)
        {
            VifService = vifService;
            AttachmentService = attachmentService;
            VrfService = vrfService;
            Mapper = mapper;
            HubContext = signalRConnectionManager.GetHubContext<NetworkSyncHub>();
        }
        
        private IAttachmentService AttachmentService { get; set; } 
        private IVifService VifService { get; set; }
        private IVrfService VrfService { get; set; }
        private IMapper Mapper { get; set; }
        private IHubContext HubContext { get; set; }

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

            var vifs = await VifService.GetAllByAttachmentIDAsync(id.Value);

            var checkSyncResult = VifService.ShallowCheckNetworkSync(vifs);
            if (checkSyncResult.IsSuccess)
            {
                ViewData["SuccessMessage"] = "All vifs appear to be synchronised with the network.";
            }
            else
            {
                ViewData["ErrorMessage"] = checkSyncResult.GetHtmlListMessage();
            }

            ViewBag.Attachment = Mapper.Map<AttachmentViewModel>(attachment);

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

            ViewBag.Vif = Mapper.Map<VifViewModel>(vif);

            var attachment = await AttachmentService.GetByIDAsync(vif.AttachmentID);
            ViewBag.Attachment = Mapper.Map<AttachmentViewModel>(attachment);

            var multiPortVlans = await VifService.UnitOfWork.VlanRepository.GetAsync(q => q.VifID == id.Value, 
                includeProperties:"Vif.Attachment.Interfaces.Ports");

            return View(Mapper.Map<List<MultiPortVlanViewModel>>(multiPortVlans));
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
            ViewBag.Attachment = Mapper.Map<AttachmentViewModel>(attachment);

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
            ViewBag.Attachment = Mapper.Map<AttachmentViewModel>(attachment);

            await PopulateTenantsDropDownList(attachment.TenantID);
            PopulateContractBandwidthPoolsDropDownList(attachment, attachment.TenantID);
            await PopulateContractBandwidthsDropDownList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AttachmentID,IpAddress1,SubnetMask1,"
            + "IpAddress2,SubnetMask2,IpAddress3,SubnetMask3,"
            + "IpAddress4,SubnetMask4,IsLayer3,AutoAllocateVlanTag,"
            + "RequestedVlanTag,TenantID,ContractBandwidthPoolID," 
            + "ContractBandwidthID,TrustReceivedCosDscp")] VifRequestViewModel request)
        {

            var attachment = await AttachmentService.GetByIDAsync(request.AttachmentID);
            if (attachment == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var mappedRequest = Mapper.Map<VifRequest>(request);

                // Add AttachmentIsMultiPort property here to control program flow in the service logic

                mappedRequest.AttachmentIsMultiPort = attachment.IsMultiPort;

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

            ViewBag.Attachment = Mapper.Map<AttachmentViewModel>(attachment);

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
            ViewBag.Attachment = Mapper.Map<AttachmentViewModel>(attachment);

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

                var validateVrfDelete = await VrfService.ValidateDeleteAsync(item.VrfID.Value);
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
                    ViewData["ErrorMessage"] = string.Empty;

                    if (!syncResult.IsSuccess)
                    {
                        syncResult.NetworkSyncServiceResults.ForEach(f => inSync = f.StatusCode != NetworkSyncStatusCode.NotFound ? false : inSync);
                    }

                    if (!inSync)
                    {
                        ViewData["ErrorMessage"] += syncResult.GetHtmlListMessage();
                    }
                    else 
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
                ViewBag.Attachment = Mapper.Map<AttachmentViewModel>(attachment);

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
        public async Task<IActionResult> CheckSync(int? id)
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

            var checkSyncResult = await VifService.CheckNetworkSyncAsync(item);

            if (checkSyncResult.IsSuccess)
            {
                ViewData["SuccessMessage"] = "The vif is synchronised with the network.";
                item.RequiresSync = false;
            }
            else
            {
                if (checkSyncResult.NetworkSyncServiceResults.Single().StatusCode == NetworkSyncStatusCode.Success)
                {
                    ViewData["ErrorMessage"] = "The vif is not synchronised with the network. Press the 'Sync' button to update the network.";
                }

                ViewData["ErrorMessage"] = checkSyncResult.GetHtmlListMessage();
                item.RequiresSync = true;
            }

            await VifService.UpdateRequiresSyncAsync(item, !checkSyncResult.IsSuccess, true);

            var attachment = await AttachmentService.GetByIDAsync(item.AttachmentID);
            ViewBag.Attachment = Mapper.Map<AttachmentViewModel>(attachment);

            return View("Details", Mapper.Map<VifViewModel>(item));
        }

        [HttpPost]
        public async Task CheckSyncAllByAttachmentID(int? id)
        {
            if (id == null)
            {
                RedirectToAction("PageNotFound");
            }

            var attachment = await AttachmentService.GetByIDAsync(id.Value);

            if (attachment == null)
            {
                HubContext.Clients.Group($"Attachment_{id.Value}").onAllComplete("The attachment was not found.", false);
                return;
            }

            var vifs = await VifService.GetAllByAttachmentIDAsync(id.Value);

            if (vifs.Count() > 0)
            {

                var message = String.Empty;
                var progress = new Progress<ServiceResult>(UpdateClientProgress);
                var checkSyncResults = await VifService.CheckNetworkSyncAsync(vifs, progress);

                if (checkSyncResults.Where(q => q.IsSuccess).Count() == checkSyncResults.Count())
                {
                    message = "All VIFs are synchronised with the network.";
                    HubContext.Clients.Group($"Attachment_{id.Value}").onAllComplete(message, true);
                }
                else
                {
                    checkSyncResults.ToList().ForEach(q => message += q.GetHtmlListMessage());
                    HubContext.Clients.Group($"Attachment_{id.Value}").onAllComplete(message, false);
                }

                foreach (var r in checkSyncResults)
                {
                    var item = (Vif)r.Item;
                    await VifService.UpdateRequiresSyncAsync(item, !r.IsSuccess, true);
                }
            }
            else
            {
                HubContext.Clients.All.onAllComplete("No vifs were found", true);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Sync(int? id)
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

            await VifService.UpdateRequiresSyncAsync(item, !syncResult.IsSuccess, true);

            var attachment = await AttachmentService.GetByIDAsync(item.AttachmentID);
            ViewBag.Attachment = Mapper.Map<AttachmentViewModel>(attachment);

            return View("Details", Mapper.Map<VifViewModel>(item));
        }

        [HttpPost]
        public async Task SyncAllByAttachmentID(int? id)
        {
            if (id == null)
            {
                RedirectToAction("PageNotFound");
            }

            var attachment = await AttachmentService.GetByIDAsync(id.Value);

            if (attachment == null)
            {
                HubContext.Clients.Group($"Attachment_{id.Value}").onAllComplete("The attachment was not found.", false);
                return;
            }

            var vifs = await VifService.GetAllByAttachmentIDAsync(id.Value);

            if (vifs.Count() > 0)
            {
                var message = string.Empty;
                var progress = new Progress<ServiceResult>(UpdateClientProgress);
                var checkSyncResults = await VifService.SyncToNetworkAsync(vifs, progress);

                if (checkSyncResults.Where(q => q.IsSuccess).Count() == checkSyncResults.Count())
                {
                    message = "All VIFs are synchronised with the network.";
                    HubContext.Clients.Group($"Attachment_{id.Value}").onAllComplete(message, true);
                }
                else
                {
                    checkSyncResults.ToList().ForEach(q => message += q.GetHtmlListMessage());
                    HubContext.Clients.Group($"Attachment_{id.Value}").onAllComplete(message, false);
                }

                foreach (var r in checkSyncResults)
                {
                    var item = (Vif)r.Item;
                    await VifService.UpdateRequiresSyncAsync(item, !r.IsSuccess, true);
                }
            }
            else
            {
                HubContext.Clients.All.onAllComplete("No vifs were found", true);
            }
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

            var validationResult = await VrfService.ValidateDeleteAsync(item.VrfID.Value);
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

            await VifService.UpdateRequiresSyncAsync(item, true, true);

            var attachment = await AttachmentService.GetByIDAsync(item.AttachmentID);
            ViewBag.Attachment = Mapper.Map<AttachmentViewModel>(attachment);

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
            ViewBag.ContractBandwidthID = new SelectList(contractBandwidths.OrderBy(b => b.BandwidthMbps), 
                "ContractBandwidthID", "BandwidthMbps", selectedContractBandwidth);
        }

        /// <summary>
        /// Delegate method which is called when sync or checksync of an 
        /// individual vif has completed.
        /// </summary>
        /// <param name="result"></param>
        private void UpdateClientProgress(ServiceResult result)
        {
            var vif = (Vif)result.Item;
            var attachment = (Attachment)result.Context;

            // Update all clients which are subscribed to the Tenant context
            // supplied in the result object

            HubContext.Clients.Group($"Attachment_{attachment.AttachmentID}")
                .onSingleComplete(Mapper.Map<VifViewModel>(vif), result.IsSuccess);
        }
    }
}
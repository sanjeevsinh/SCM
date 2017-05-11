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
using System.Net;
using SCM.Hubs;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using Microsoft.AspNetCore.SignalR;

namespace SCM.Controllers
{
    public abstract class BaseVpnController : BaseViewController
    {
        public BaseVpnController(IVpnService vpnService, 
                           IRouteTargetService routeTargetService,
                           IAttachmentSetService attachmentSetService,
                           IAttachmentSetVrfService attachmentSetVrfService,
                           IAttachmentService attachmentService, 
                           IVifService vifService,
                           IMapper mapper,
                           IConnectionManager signalRConnectionManager)
        {
            VpnService = vpnService;
            RouteTargetService = routeTargetService;
            AttachmentService = attachmentService;
            VifService = vifService;
            AttachmentSetVrfService = attachmentSetVrfService;
            AttachmentSetService = attachmentSetService;
            HubContext = signalRConnectionManager.GetHubContext<NetworkSyncHub>();
            Mapper = mapper;
        }

        internal IVpnService VpnService { get; set; }
        internal IAttachmentService AttachmentService { get; set; }
        internal IVifService VifService { get; set; }
        internal IRouteTargetService RouteTargetService { get; set; }
        internal IAttachmentSetService AttachmentSetService { get; set; }
        internal IAttachmentSetVrfService AttachmentSetVrfService { get; set; }
        internal IMapper Mapper { get; set; }
        private IHubContext HubContext { get; set; }

        [HttpGet]
        public async Task<IActionResult> Details(VpnViewModel vpn)
        {
            var item = await VpnService.GetByIDAsync(vpn.VpnID);

            if (item == null)
            {
                return NotFound();
            }

            var mappedVpn = Mapper.Map<VpnViewModel>(item);
            if (vpn.AttachmentSetID != null)
            {
                var attachmentSet = await AttachmentSetService.GetByIDAsync(vpn.AttachmentSetID.Value);
                mappedVpn.AttachmentSet = Mapper.Map<AttachmentSetViewModel>(attachmentSet);
            }

            return View(mappedVpn);
        }

        [HttpPost]
        public async Task Sync(int? id)
        {
            if (id == null)
            {
                RedirectToAction("PageNotFound");

                return;
            }

            var item = await VpnService.GetByIDAsync(id.Value);
            if (item == null)
            {
                HubContext.Clients.Group($"Tenant_{item.TenantID}")
                    .onSingleComplete(null, false, "The vpn was not found.");

                return;
            }

            var mappedItem = Mapper.Map<VpnViewModel>(item);
            var validationOk = true;
            var validationMessage = "You must resolve the following issues first: ";

            var attachmentValidationResult = await AttachmentService.ValidateAsync(item);
            if (!attachmentValidationResult.IsSuccess)
            {
                validationOk = false;
                validationMessage += attachmentValidationResult.GetHtmlListMessage();
            }

            var vifValidationResult = await VifService.ValidateAsync(item);
            if (!vifValidationResult.IsSuccess)
            {
                validationOk = false;
                validationMessage += vifValidationResult.GetHtmlListMessage();
            }

            var routeTargetsValidationResult = RouteTargetService.Validate(item);
            if (!routeTargetsValidationResult.IsSuccess)
            {
                validationOk = false;
                validationMessage += routeTargetsValidationResult.GetHtmlListMessage();
            }

            var attachmentSetVrfsValidationResult = await AttachmentSetVrfService.CheckVrfsConfiguredCorrectlyAsync(item);
            if (!attachmentSetVrfsValidationResult.IsSuccess)
            {
                validationOk = false;
                validationMessage += attachmentSetVrfsValidationResult.GetHtmlListMessage();
            }

            if (!validationOk)
            {
                HubContext.Clients.Group($"Tenant_{item.Tenant.TenantID}")
                    .onSingleComplete(mappedItem, false, validationMessage);
            }
            else
            {
                var syncResult = await VpnService.SyncToNetworkAsync(item);

                if (syncResult.IsSuccess)
                {
                    HubContext.Clients.Group($"Tenant_{item.Tenant.TenantID}")
                        .onSingleComplete(mappedItem, true, $"VPN {item.Name} is synchronised with the metwork.");
                }
                else
                {
                    HubContext.Clients.Group($"Tenant_{item.Tenant.TenantID}")
                        .onSingleComplete(mappedItem, false, syncResult.GetHtmlListMessage());
                }

                await VpnService.UpdateVpnRequiresSyncAsync(item.VpnID, !syncResult.IsSuccess, true);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CheckSync(VpnViewModel vpn)
        {
            var item = await VpnService.GetByIDAsync(vpn.VpnID);
            if (item == null)
            {
                return NotFound();
            }

            var checkSyncResult = await VpnService.CheckNetworkSyncAsync(item);
            if (checkSyncResult.IsSuccess)
            {
                ViewData["SuccessMessage"] = "The VPN is synchronised with the network.";
            }
            else
            {
                if (checkSyncResult.NetworkSyncServiceResults.Single().StatusCode == NetworkSyncStatusCode.Success)
                {
                    ViewData["ErrorMessage"] = "The VPN is not synchronised with the network. Press the 'Sync' button to update the network.";
                }

                ViewData["ErrorMessage"] = checkSyncResult.GetHtmlListMessage();
            }

            await VpnService.UpdateVpnRequiresSyncAsync(item.VpnID, !checkSyncResult.IsSuccess, true);

            var mappedVpn = Mapper.Map<VpnViewModel>(item);

            // Check if AttachmentSetID property value is defined. If it is then retrieve the AttachmentSet context
            // and append to the view model.

            if (vpn.AttachmentSetID != null)
            {
                var attachmentSet = await AttachmentSetService.GetByIDAsync(vpn.AttachmentSetID.Value);
                mappedVpn.AttachmentSet = Mapper.Map<AttachmentSetViewModel>(attachmentSet);
            }

            return View("Details", mappedVpn);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFromNetwork(VpnViewModel vpn)
        {
            var item = await VpnService.GetByIDAsync(vpn.VpnID);
            if (item == null)
            {
                ViewData["VpnDeletedMessage"] = "The VPN has been deleted by another user. Return to the list.";

                return View("VpnDeleted");
            }

            await VpnService.UpdateVpnRequiresSyncAsync(item, true, false);

            var syncResult = await VpnService.DeleteFromNetworkAsync(item);
            if (syncResult.IsSuccess)
            {
                ViewData["SuccessMessage"] = "The VPN has been deleted from the network.";
            }
            else
            {
                ViewData["ErrorMessage"] = syncResult.GetHtmlListMessage();
            }

            item.RequiresSync = true;

            var mappedVpn = Mapper.Map<VpnViewModel>(item);
            if (vpn.AttachmentSetID != null)
            {
                var attachmentSet = await AttachmentSetService.GetByIDAsync(vpn.AttachmentSetID.Value);
                mappedVpn.AttachmentSet = Mapper.Map<AttachmentSetViewModel>(attachmentSet);
            }

            return View("Details", mappedVpn);
        }
    }
}
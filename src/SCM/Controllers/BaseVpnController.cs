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
        internal IHubContext HubContext { get; set; }

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
        public async Task<IActionResult> DeleteFromNetwork(VpnViewModel vpn)
        {
            var item = await VpnService.GetByIDAsync(vpn.VpnID);
            if (item == null)
            {
                ViewData["VpnDeletedMessage"] = "The VPN has been deleted by another user. Return to the list.";

                return View("VpnDeleted");
            }

            await VpnService.UpdateRequiresSyncAsync(item, true, false);

            var syncResult = await VpnService.DeleteFromNetworkAsync(item);
            if (syncResult.IsSuccess)
            {
                ViewData["SuccessMessage"] = "The VPN has been deleted from the network.";
            }
            else
            {
                ViewData["ErrorMessage"] = FormatAsHtmlList(syncResult.GetMessage());
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
        internal async Task<IEnumerable<ServiceResult>> GetFailedValidationResultsAsync(Vpn vpn)
        {
            var tasks = new List<Task<ServiceResult>>();

            tasks.Add(AttachmentService.ValidateAsync(vpn));
            tasks.Add(VifService.ValidateAsync(vpn));
            tasks.Add(AttachmentSetVrfService.CheckVrfsConfiguredCorrectlyAsync(vpn));

            var results = new List<ServiceResult>();
            results.AddRange(await Task.WhenAll(tasks));
            results.Add(RouteTargetService.Validate(vpn));

            return results.Where(q => !q.IsSuccess);
        }
    }
}
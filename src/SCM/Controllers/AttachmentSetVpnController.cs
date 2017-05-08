using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using AutoMapper;
using SCM.Models;
using SCM.Models.ViewModels;
using SCM.Services.SCMServices;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using SCM.Hubs;

namespace SCM.Controllers
{
    public class AttachmentSetVpnController : BaseVpnController
    {
        public AttachmentSetVpnController(IVpnService vpnService,
            IRouteTargetService routeTargetService,
            IAttachmentSetService attachmentSetService,
            IAttachmentSetVrfService attachmentSetVrfService,
            IAttachmentService attachmentService, 
            IVifService vifService, 
            IMapper mapper,
            IConnectionManager signalRConnectionManager) : 
            base (vpnService, 
                routeTargetService,
                attachmentSetService, 
                attachmentSetVrfService, 
                attachmentService, 
                vifService, 
                mapper)
        {
            HubContext = signalRConnectionManager.GetHubContext<VpnHub>();
        }

        private IHubContext HubContext { get; set; }

        [HttpGet]
        public async Task<IActionResult> GetAllByAttachmentSetID(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vpns = await VpnService.GetAllByAttachmentSetIDAsync(id.Value);

            var checkSyncResult = VpnService.ShallowCheckNetworkSync(vpns);
            if (checkSyncResult.IsSuccess)
            {
                ViewData["SuccessMessage"] = "All VPNs appear to be synchronised with the network.";
            }
            else
            {
                ViewData["ErrorMessage"] = checkSyncResult.GetHtmlListMessage();
            }

            ViewBag.AttachmentSet = await AttachmentSetService.GetByIDAsync(id.Value);

            return View(Mapper.Map<List<VpnViewModel>>(vpns));
        }

        [HttpPost]
        public async Task<IActionResult> CheckSyncAllByAttachmentSetID(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vpns = await VpnService.GetAllByAttachmentSetIDAsync(id.Value);

            if (vpns.Count() > 0)
            {
                var progress = new Progress<ServiceResult>(VpnCheckSyncUpdateClientsProgress);

                var checkSyncResults = await VpnService.CheckNetworkSyncAsync(vpns, progress);
                if (checkSyncResults.Where(q => q.IsSuccess).Count() == checkSyncResults.Count())
                {
                    ViewData["SuccessMessage"] = "All VPNs are synchronised with the network.";
                }
                else
                {
                    checkSyncResults.ToList().ForEach(q => ViewData["ErrorMessage"] += q.GetHtmlListMessage());
                }

                foreach (var r in checkSyncResults)
                {
                    var item = (Vpn)r.Item;
                    await VpnService.UpdateVpnRequiresSyncAsync(item, !r.IsSuccess, true);
                }
            }

            ViewBag.AttachmentSet = await AttachmentSetService.GetByIDAsync(id.Value);

            return View("GetAllByAttachmentSetID", Mapper.Map<List<VpnViewModel>>(vpns));
        }


        [HttpPost]
        public async Task SyncAllByAttachmentSetID(int? id)
        {
            if (id == null)
            {
                return;
            }

            var vpns = await VpnService.GetAllByAttachmentSetIDAsync(id.Value);

            if (vpns.Count() > 0)
            {
                var progress = new Progress<ServiceResult>(VpnSyncUpdateClientsProgress);
                var results = await VpnService.SyncToNetworkAsync(vpns, progress);
                string message = string.Empty;

                if (results.Where(q => q.IsSuccess).Count() == results.Count())
                {
                    message = "All VPNs are synchronised with the network.";
                    HubContext.Clients.All.syncAllComplete(message, true);
                }
                else
                {
                    results.ToList().ForEach(q => message += q.GetHtmlListMessage());
                    HubContext.Clients.All.syncAllComplete(message, false);
                }

                foreach (var r in results)
                {
                    var item = (Vpn)r.Item;
                    await VpnService.UpdateVpnRequiresSyncAsync(item, !r.IsSuccess, true);
                }
            }
        }

        void VpnSyncUpdateClientsProgress(ServiceResult result)
        {
            var vpn = (Vpn)result.Item;
            HubContext.Clients.All.updateSyncProgress(Mapper.Map<VpnViewModel>(vpn));
        }
        void VpnCheckSyncUpdateClientsProgress(ServiceResult result)
        {
            var vpn = (Vpn)result.Item;
            HubContext.Clients.All.updateCheckSyncProgress(Mapper.Map<VpnViewModel>(vpn));
        }
    }
}

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
            HubContext = signalRConnectionManager.GetHubContext<NetworkSyncHub>();
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
        public async Task CheckSyncAllByAttachmentSetID(int? id)
        {
            if (id == null)
            {
                return;
            }

            var attachmentSet = await AttachmentSetService.GetByIDAsync(id.Value);

            if (attachmentSet == null)
            {
                HubContext.Clients.Group($"AttachmentSet_{id.Value}").onAllComplete("The Attachment Set was not found.", false);
                return;
            }

            var vpns = await VpnService.GetAllByAttachmentSetIDAsync(id.Value);

            if (vpns.Count() > 0)
            {
                var progress = new Progress<ServiceResult>(UpdateClientProgress);
                var message = string.Empty;

                var checkSyncResults = await VpnService.CheckNetworkSyncAsync(vpns, attachmentSet, progress);
                if (checkSyncResults.Where(q => q.IsSuccess).Count() == checkSyncResults.Count())
                {
                    message = "All VPNs are synchronised with the network.";
                    HubContext.Clients.Group($"AttachmentSet_{id.Value}").onAllComplete(message, true);
                }
                else
                {
                    checkSyncResults.ToList().ForEach(q => message += q.GetHtmlListMessage());
                    HubContext.Clients.Group($"AttachmentSet_{id.Value}").onAllComplete(message, false);
                }

                foreach (var r in checkSyncResults)
                {
                    var item = (Vpn)r.Item;
                    await VpnService.UpdateVpnRequiresSyncAsync(item, !r.IsSuccess, true);
                }
            }
            else
            {
                HubContext.Clients.All.onAllComplete("No VPNs were found", true);
            }
        }

        [HttpPost]
        public async Task SyncAllByAttachmentSetID(int? id)
        {
            if (id == null)
            {
                HubContext.Clients.Group($"AttachmentSet_{id.Value}").onAllComplete("The Attachment Set ID cannot be null.", false);
                return;
            }

            var attachmentSet = await AttachmentSetService.GetByIDAsync(id.Value);

            if (attachmentSet == null)
            {
                HubContext.Clients.Group($"AttachmentSet_{id.Value}").onAllComplete("The Attachment Set was not found.", false);
                return;
            }

            var vpns = await VpnService.GetAllByAttachmentSetIDAsync(id.Value);

            if (vpns.Count() > 0)
            {
                var progress = new Progress<ServiceResult>(UpdateClientProgress);
                var results = await VpnService.SyncToNetworkAsync(vpns, attachmentSet, progress);
                var message = string.Empty;

                if (results.Where(q => q.IsSuccess).Count() == results.Count())
                {
                    message = "All VPNs are synchronised with the network.";
                    HubContext.Clients.Group($"AttachmentSet_{id.Value}").onAllComplete(message, true);
                }
                else
                {
                    results.ToList().ForEach(q => message += q.GetHtmlListMessage());
                    HubContext.Clients.Group($"AttachmentSet_{id.Value}").onAllComplete(message, false);
                }

                foreach (var r in results)
                {
                    var item = (Vpn)r.Item;
                    await VpnService.UpdateVpnRequiresSyncAsync(item, !r.IsSuccess, true);
                }
            }
            else
            {
                HubContext.Clients.All.onAllComplete("No VPNs were found", true);
            }
        }

        void UpdateClientProgress(ServiceResult result)
        {
            var vpn = (Vpn)result.Item;
            var attachmentSet = (AttachmentSet)result.Context;

            HubContext.Clients.Group($"AttachmentSet_{attachmentSet.AttachmentSetID}")
                .onSingleComplete(Mapper.Map<VpnViewModel>(vpn), result.IsSuccess);
        }
    }
}

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
                mapper,
                signalRConnectionManager)
        {
        }

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
                ViewData["ErrorMessage"] = FormatAsHtmlList(checkSyncResult.GetMessage());
            }

            ViewBag.AttachmentSet = await AttachmentSetService.GetByIDAsync(id.Value);

            return View(Mapper.Map<List<VpnViewModel>>(vpns));
        }

        [HttpPost]
        public async Task Sync(VpnViewModel vpn)
        {
            if (vpn.AttachmentSetID == null)
            {
                RedirectToAction("PageNotFound");
                return;
            }

            var attachmentSet = await AttachmentSetService.GetByIDAsync(vpn.AttachmentSetID.Value);
            if (attachmentSet == null)
            {
                RedirectToAction("PageNotFound");
            }

            var item = await VpnService.GetByIDAsync(vpn.VpnID);
            if (item == null)
            {
                HubContext.Clients.Group($"AttachmentSet_{attachmentSet.AttachmentSetID}")
                    .onSingleComplete(null, false, "The vpn was not found.");

                return;
            }

            var mappedItem = Mapper.Map<VpnViewModel>(item);
            var failedValidationResults = await GetFailedValidationResultsAsync(item);
      
            if (failedValidationResults.Count() > 0) 
            {
                var message = "You must resolve the following issues first: ";
                failedValidationResults.ToList().ForEach(q => message += q.GetMessage());
                HubContext.Clients.Group($"AttachmentSet_{attachmentSet.AttachmentSetID}")
                    .onSingleComplete(mappedItem, false, FormatAsHtmlList(message));
            }
            else
            {
                var syncResult = await VpnService.SyncToNetworkAsync(item);

                if (syncResult.IsSuccess)
                {
                    HubContext.Clients.Group($"AttachmentSet_{attachmentSet.AttachmentSetID}")
                        .onSingleComplete(mappedItem, true, $"VPN {item.Name} is synchronised with the network.");
                }
                else
                {
                    HubContext.Clients.Group($"AttachmentSet_{attachmentSet.AttachmentSetID}")
                        .onSingleComplete(mappedItem, false, FormatAsHtmlList(syncResult.GetMessage()));
                }

                await VpnService.UpdateVpnRequiresSyncAsync(item.VpnID, !syncResult.IsSuccess, true);
            }
        }

        [HttpPost]
        public async Task CheckSync(VpnViewModel vpn)
        {
            if (vpn.AttachmentSetID == null)
            {
                RedirectToAction("PageNotFound");
                return;
            }

            var attachmentSet = await AttachmentSetService.GetByIDAsync(vpn.AttachmentSetID.Value);
            if (attachmentSet == null)
            {
                RedirectToAction("PageNotFound");
            }

            var item = await VpnService.GetByIDAsync(vpn.VpnID);
            if (item == null)
            {
                HubContext.Clients.Group($"AttachmentSet_{attachmentSet.AttachmentSetID}")
                    .onSingleComplete(null, false, "The vpn was not found.");
                return;
            }

            var mappedItem = Mapper.Map<VpnViewModel>(item);
            var result = await VpnService.CheckNetworkSyncAsync(item);
            if (result.IsSuccess)
            {
                HubContext.Clients.Group($"AttachmentSet_{attachmentSet.AttachmentSetID}")
                    .onSingleComplete(mappedItem, true, $"VPN {item.Name} is synchronised with the network.");
            }
            else
            {
                if (result.NetworkSyncServiceResults.Single().StatusCode == NetworkSyncStatusCode.Success)
                {
                    HubContext.Clients.Group($"AttachmentSet_{attachmentSet.AttachmentSetID}")
                        .onSingleComplete(mappedItem, false,
                        $"VPN {item.Name} is not synchronised with the network. Press the 'Sync' button to update the network.");
                }
                else
                {
                    HubContext.Clients.Group($"AttachmentSet_{attachmentSet.AttachmentSetID}")
                        .onSingleComplete(mappedItem, false, FormatAsHtmlList(result.GetMessage()));
                }
            }

            await VpnService.UpdateVpnRequiresSyncAsync(item.VpnID, !result.IsSuccess, true);
        }

        [HttpPost]
        public async Task CheckSyncAllByAttachmentSetID(int? id)
        {
            if (id == null)
            {
                RedirectToAction("PageNotFound");
                return;
            }

            var attachmentSet = await AttachmentSetService.GetByIDAsync(id.Value);

            if (attachmentSet == null)
            {
                RedirectToAction("PageNotFound");
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
                    HubContext.Clients.Group($"AttachmentSet_{attachmentSet.AttachmentSetID}").onAllComplete(FormatAsHtmlList(message), true);
                }
                else
                {
                    checkSyncResults.ToList().ForEach(q => message += q.GetMessage());
                    HubContext.Clients.Group($"AttachmentSet_{attachmentSet.AttachmentSetID}").onAllComplete(FormatAsHtmlList(message), false);
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
                RedirectToAction("PageNotFound");
                return;
            }

            var attachmentSet = await AttachmentSetService.GetByIDAsync(id.Value);

            if (attachmentSet == null)
            {
                RedirectToAction("PageNotFound");
                return;
            }

            var vpns = await VpnService.GetAllByAttachmentSetIDAsync(id.Value);

            if (vpns.Count() > 0)
            {
                // Validate the VPNs first

                var failedValidationResults = await GetFailedValidationResultsAsync(vpns);

                if (failedValidationResults.Count() > 0)
                {
                    var validationMessage = "You must resolve the following issues first: ";
                    failedValidationResults.ToList().ForEach(q => validationMessage += q.GetMessage());
                    HubContext.Clients.Group($"AttachmentSet_{attachmentSet.AttachmentSetID}")
                        .onAllComplete(FormatAsHtmlList(validationMessage), false);

                    return;
                }
                
                var progress = new Progress<ServiceResult>(UpdateClientProgress);
                var results = await VpnService.SyncToNetworkAsync(vpns, attachmentSet, progress);
                var message = string.Empty;

                if (results.Where(q => q.IsSuccess).Count() == results.Count())
                {
                    message = "All VPNs are synchronised with the network.";
                    HubContext.Clients.Group($"AttachmentSet_{attachmentSet.AttachmentSetID}")
                        .onAllComplete(FormatAsHtmlList(message), true);
                }
                else
                {
                    results.ToList().ForEach(q => message += q.GetMessage());
                    HubContext.Clients.Group($"AttachmentSet_{attachmentSet.AttachmentSetID}")
                        .onAllComplete(FormatAsHtmlList(message), false);
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

        private async Task<IEnumerable<ServiceResult>> GetFailedValidationResultsAsync(IEnumerable<Vpn> vpns)
        {
            var results = new List<ServiceResult>();
            foreach (var vpn in vpns)
            {
                results.AddRange(await GetFailedValidationResultsAsync(vpn));
            }

            return results;
        }

        /// <summary>
        /// Delegate method which is called when sync or checksync of an 
        /// individual VPN has completed.
        /// </summary>
        /// <param name="result"></param>
        private void UpdateClientProgress(ServiceResult result)
        {
            var vpn = (Vpn)result.Item;
            var attachmentSet = (AttachmentSet)result.Context;

            // Update all clients which are subscribed to the Attachment Set context
            // supplied in the result object

            HubContext.Clients.Group($"AttachmentSet_{attachmentSet.AttachmentSetID}")
                .onSingleComplete(Mapper.Map<VpnViewModel>(vpn), result.IsSuccess);
        }
    }
}

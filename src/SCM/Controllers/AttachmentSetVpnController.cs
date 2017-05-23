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
            var successMessage = string.Empty;
            vpns.Where(q => q.Created).ToList().ForEach(q => successMessage += $"{q.Name} has been created.");

            var checkSyncResult = VpnService.ShallowCheckNetworkSync(vpns);
            if (checkSyncResult.IsSuccess)
            {
                successMessage += "All VPNs appear to be synchronised with the network.";
            }
            else
            {
                ViewData["ErrorMessage"] = FormatAsHtmlList(checkSyncResult.GetMessage());
            }

            ViewData["SuccessMessage"] = FormatAsHtmlList(successMessage);
            ViewBag.AttachmentSet = await AttachmentSetService.GetByIDAsync(id.Value);

            return View(Mapper.Map<List<VpnViewModel>>(vpns));
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

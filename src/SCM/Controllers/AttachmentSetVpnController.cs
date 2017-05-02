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
using SCM.Services;
using SCM.Services.SCMServices;
using SCM.Models.NetModels.IpVpnNetModels;

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
            IMapper mapper) : 
            base (vpnService, routeTargetService,attachmentSetService, attachmentSetVrfService, attachmentService, vifService, mapper)
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

            var checkSyncResults = await VpnService.CheckNetworkSyncAsync(vpns);
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

            ViewBag.AttachmentSet = await AttachmentSetService.GetByIDAsync(id.Value);

            return View("GetAllByAttachmentSetID", Mapper.Map<List<VpnViewModel>>(vpns));
        }


        [HttpPost]
        public async Task<IActionResult> SyncAllByAttachmentSetID(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vpns = await VpnService.GetAllByAttachmentSetIDAsync(id.Value);

            var checkSyncResults = await VpnService.SyncToNetworkAsync(vpns);

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

            ViewBag.AttachmentSet = await AttachmentSetService.GetByIDAsync(id.Value);

            return View("GetAllByAttachmentSetID", Mapper.Map<List<VpnViewModel>>(vpns));
        }
    }
}

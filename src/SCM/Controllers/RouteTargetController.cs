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
using SCM.Models.ServiceModels;

namespace SCM.Controllers
{
    public class RouteTargetController : BaseViewController
    {
        public RouteTargetController(IRouteTargetService routeTargetService, IVpnService vpnService, IMapper mapper)
        {
            RouteTargetService = routeTargetService;
            VpnService = vpnService;
            Mapper = mapper;
        }
        private IRouteTargetService RouteTargetService { get; set; }
        private IVpnService VpnService { get; set; }
        private IMapper Mapper { get; set; }

        [HttpGet]
        public async Task<IActionResult> GetAllByVpnID(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vpn = await VpnService.GetByIDAsync(id.Value);
            if (vpn == null)
            {
                return NotFound();
            }

            var validateResult = RouteTargetService.Validate(vpn);

            if (!validateResult.IsSuccess)
            {
                ModelState.AddModelError(string.Empty,validateResult.GetMessage());
            }
            else
            {
                ViewData["ValidationSuccessMessage"] = "The Route Targets for this VPN are configured correctly!";
            }

            var routeTargets = await RouteTargetService.GetAllByVpnIDAsync(id.Value);
            ViewBag.Vpn = vpn;

            return View(Mapper.Map<List<RouteTargetViewModel>>(routeTargets));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbResult = await RouteTargetService.UnitOfWork.RouteTargetRepository.GetAsync(q => q.RouteTargetID == id, includeProperties: "Vpn");
            var item = dbResult.SingleOrDefault();

            if (item == null)
            {
                return NotFound();
            }

            await PopulateVpnItem(item.VpnID);
            return View(Mapper.Map<RouteTargetViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await PopulateVpnItem(id.Value);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VpnID,AdministratorSubField,RequestedAssignedNumberSubField," 
            + "AutoAllocateAssignedNumberSubField,IsHubExport")] RouteTargetRequestViewModel request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var validationResult = await RouteTargetService.CheckVpnOkToAddOrRemoveRouteTargetAsync(request.VpnID);

                    if (!validationResult.IsSuccess)
                    {
                        validationResult.GetMessageList().ForEach(message => ModelState.AddModelError(string.Empty, message));
                    }
                    else
                    {
                        var result = await RouteTargetService.AddAsync(Mapper.Map<RouteTargetRequest>(request));
                        if (!result.IsSuccess)
                        {
                            result.GetMessageList().ForEach(message => ModelState.AddModelError(string.Empty, message));
                        }
                        else
                        {
                            return RedirectToAction("GetAllByVpnID", new { id = request.VpnID });
                        }
                    }
                }
            }
            catch (DbUpdateException /** ex **/ )
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            await PopulateVpnItem(request.VpnID);
            return View(request);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbResult = await RouteTargetService.UnitOfWork.RouteTargetRepository.GetAsync(q => q.RouteTargetID == id.Value, includeProperties: "Vpn");
            var routeTarget = dbResult.SingleOrDefault();

            if (routeTarget == null)
            {
                return NotFound();
            }

            await PopulateVpnItem(routeTarget.VpnID);
            return View(Mapper.Map<RouteTargetViewModel>(routeTarget));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var routeTarget = await RouteTargetService.GetByIDAsync(id.Value);
            if (routeTarget == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("GetAllByVpnID", new { id = id.Value });
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

            await PopulateVpnItem(routeTarget.VpnID);
            return View(Mapper.Map<RouteTargetViewModel>(routeTarget));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(RouteTargetViewModel routeTarget)
        {
            try
            {
                var dbResult = await RouteTargetService.UnitOfWork.RouteTargetRepository.GetAsync(q => q.RouteTargetID == routeTarget.RouteTargetID, 
                    AsTrackable: false);
                var currentRouteTarget = dbResult.SingleOrDefault();

                if (currentRouteTarget != null)
                {
                    var mappedRouteTarget = Mapper.Map<RouteTarget>(routeTarget);
                    var validationResult = await RouteTargetService.CheckVpnOkToAddOrRemoveRouteTargetAsync(mappedRouteTarget.VpnID);

                    if (!validationResult.IsSuccess)
                    {
                        ViewData["ErrorMessage"] = validationResult.GetHtmlListMessage();
                        await PopulateVpnItem(routeTarget.VpnID);

                        return View(Mapper.Map<RouteTargetViewModel>(currentRouteTarget));
                    }

                    await RouteTargetService.DeleteAsync(mappedRouteTarget);
                }

                return RedirectToAction("GetAllByVpnID", new { id = routeTarget.VpnID });
            }

            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { concurrencyError = true, id = routeTarget.RouteTargetID });
            }
        }

        private async Task PopulateVpnItem(int vpnID)
        {
            var dbResult = await RouteTargetService.UnitOfWork.VpnRepository.GetAsync(q => q.VpnID == vpnID, includeProperties: "VpnTopologyType.VpnProtocolType");
            var vpn = dbResult.SingleOrDefault();

            ViewBag.Vpn = vpn;
        }
    }
}

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

namespace SCM.Controllers
{
    public class RouteTargetController : BaseViewController
    {
        public RouteTargetController(IRouteTargetService routeTargetService, IMapper mapper)
        {
            RouteTargetService = routeTargetService;
            Mapper = mapper;
        }
        private IRouteTargetService RouteTargetService { get; set; }
        private IMapper Mapper { get; set; }

        [HttpGet]
        public async Task<IActionResult> GetAllByVpnID(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var routeTargets = await RouteTargetService.UnitOfWork.RouteTargetRepository.GetAsync(q => q.VpnID == id);
            await PopulateVpnItem(id.Value);
            var validateResult = await RouteTargetService.ValidateRouteTargetsAsync(id.Value);
            if (!validateResult.IsValid)
            {
                ModelState.AddModelError(string.Empty,validateResult.GetMessage());
            }
            else
            {
                ViewData["ValidationSuccessMessage"] = "The Route Targets for this VPN are configured correctly!";
            }

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
        public async Task<IActionResult> Create([Bind("VpnID,AdministratorSubField,AssignedNumberSubField,IsHubExport")] RouteTargetViewModel routeTarget)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var mappedRouteTarget = Mapper.Map<RouteTarget>(routeTarget);
                    var validationResult = await RouteTargetService.ValidateRouteTargetsAddRemoveAsync(mappedRouteTarget);

                    if (!validationResult.IsValid)
                    {
                        ModelState.AddModelError(string.Empty, validationResult.GetMessage());
                        await PopulateVpnItem(routeTarget.VpnID);
                        return View(routeTarget);
                    }

                    await RouteTargetService.AddAsync(mappedRouteTarget);
                    return RedirectToAction("GetAllByVpnID", new { id = routeTarget.VpnID });
                }
            }
            catch (DbUpdateException /** ex **/ )
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            await PopulateVpnItem(routeTarget.VpnID);
            return View(routeTarget);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("RouteTargetID,VpnID,AdministratorSubField,AssignedNumberSubField,IsHubExport,RowVersion")] RouteTargetViewModel routeTarget)
        {
            if (id != routeTarget.RouteTargetID)
            {
                return NotFound();
            }

            var dbResult = await RouteTargetService.UnitOfWork.RouteTargetRepository.GetAsync(q => q.RouteTargetID == id,
                includeProperties: "Vpn", AsTrackable: false);
            var currentRouteTarget = dbResult.SingleOrDefault();

            try
            {
                if (ModelState.IsValid)
                {
                    if (currentRouteTarget == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save changes. The routeTarget was deleted by another user.");
                        return View(routeTarget);
                    }

                    await RouteTargetService.UpdateAsync(Mapper.Map<RouteTarget>(routeTarget));
                    return RedirectToAction("GetAllByVpnID", new { id = routeTarget.VpnID });
                }
            }

            catch (DbUpdateConcurrencyException ex)
            {
                var exceptionEntry = ex.Entries.Single();

                var proposedAdministratorSubField = (int)exceptionEntry.Property("AdministratorSubField").CurrentValue;
                if (currentRouteTarget.AdministratorSubField != proposedAdministratorSubField)
                {
                    ModelState.AddModelError("AdministratorSubField", $"Current value: {currentRouteTarget.AdministratorSubField}");
                }

                var proposedAssignedNumberSubField = (int)exceptionEntry.Property("AssignedNumberSubField").CurrentValue;
                if (currentRouteTarget.AssignedNumberSubField != proposedAssignedNumberSubField)
                {
                    ModelState.AddModelError("AssignedNumberSubField", $"Current value: {currentRouteTarget.AssignedNumberSubField}");
                }

                var proposedIsHubExport = (bool)exceptionEntry.Property("IsHubExport").CurrentValue;
                if (currentRouteTarget.IsHubExport != proposedIsHubExport)
                {
                    ModelState.AddModelError("IsHubExport", $"Current value: {currentRouteTarget.IsHubExport}");
                }


                ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                    + "was modified by another user after you got the original value. The "
                    + "edit operation was cancelled and the current values in the database "
                    + "have been displayed. If you still want to edit this record, click "
                    + "the Save button again. Otherwise click the Back to List hyperlink.");

                ModelState.Remove("RowVersion");
            }

            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            await PopulateVpnItem(currentRouteTarget.VpnID);
            return View(Mapper.Map<RouteTargetViewModel>(currentRouteTarget));
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
                ViewData["ConcurrencyErrorMessage"] = "The record you attempted to delete "
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
                var dbResult = await RouteTargetService.UnitOfWork.RouteTargetRepository.GetAsync(q => q.RouteTargetID == routeTarget.RouteTargetID, AsTrackable: false);
                var currentRouteTarget = dbResult.SingleOrDefault();

                if (currentRouteTarget != null)
                {
                    var mappedRouteTarget = Mapper.Map<RouteTarget>(routeTarget);
                    var validationResult = await RouteTargetService.ValidateRouteTargetsAddRemoveAsync(mappedRouteTarget);

                    if (!validationResult.IsValid)
                    {
                        ModelState.AddModelError(string.Empty, validationResult.GetMessage());
                        await PopulateVpnItem(routeTarget.VpnID);
                        return View(routeTarget);
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

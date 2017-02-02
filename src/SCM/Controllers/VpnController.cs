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
    public class VpnController : BaseViewController
    {
        public VpnController(IVpnService vpnService, IMapper mapper)
        {
           VpnService = vpnService;
           Mapper = mapper;
        }
        private IVpnService VpnService { get; set; }
        private IMapper Mapper { get; set; }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var vpns = await VpnService.GetAllAsync();
            return View(Mapper.Map<List<VpnViewModel>>(vpns));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbResult = await VpnService.UnitOfWork.VpnRepository.GetAsync(q => q.VpnID == id, includeProperties: "Region,Plane,VpnTenancyType,VpnTopologyType.VpnProtocolType,Tenant");
            var item = dbResult.SingleOrDefault();

            if (item == null)
            {
                return NotFound();
            }
            return View(Mapper.Map<VpnViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> CreateStep1()
        {
            await PopulateProtocolTypesDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStep2([Bind("VpnProtocolTypeID,ProtocolType")] VpnProtocolTypeViewModel protocolType)
        {
            await PopulatePlanesDropDownList();
            await PopulateTenantsDropDownList();
            await PopulateRegionsDropDownList();
            await PopulateTopologyTypesDropDownList(protocolType.VpnProtocolTypeID);
            await PopulateTenancyTypesDropDownList();
            ViewBag.VpnProtocolType = protocolType;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,PlaneID,RegionID,VpnTenancyTypeID,VpnTopologyTypeID,TenantID,IsExtranet")] VpnViewModel vpn)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await VpnService.AddAsync(Mapper.Map<Vpn>(vpn));
                    return RedirectToAction("GetAll");
                }
            }
            catch (DbUpdateException /** ex **/ )
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            await PopulatePlanesDropDownList(vpn.Plane);
            await PopulateTenantsDropDownList(vpn.Tenant);
            await PopulateRegionsDropDownList(vpn.Region);
            await PopulateTopologyTypesDropDownList(vpn.VpnTopologyTypeID);
            await PopulateTenancyTypesDropDownList(vpn.VpnTenancyType);
            return View("CreateStep2", vpn);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbResult = await VpnService.UnitOfWork.VpnRepository.GetAsync(q => q.VpnID == id.Value, includeProperties:"VpnTopologyType.VpnProtocolType");
            var vpn = dbResult.SingleOrDefault();

            if (vpn == null)
            {
                return NotFound();
            }

            await PopulatePlanesDropDownList(vpn.Plane);
            await PopulateTenantsDropDownList(vpn.Tenant);
            await PopulateRegionsDropDownList(vpn.Region);
            await PopulateTopologyTypesDropDownList(vpn.VpnTopologyType.VpnProtocolTypeID,vpn.VpnTopologyType);
            await PopulateTenancyTypesDropDownList(vpn.VpnTenancyType);

            return View(Mapper.Map<VpnViewModel>(vpn));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("VpnID,Name,Description,PlaneID,RegionID,VpnTopologyTypeID,VpnTenancyTypeID,TenantID,IsExtranet,RowVersion")] VpnViewModel vpn)
        {
            if (id != vpn.VpnID)
            {
                return NotFound();
            }

            var dbResult = await VpnService.UnitOfWork.VpnRepository.GetAsync(q => q.VpnID == id, 
                includeProperties:"Plane,Region,VpnTenancyType,VpnTopologyType,Tenant", AsTrackable: false);
            var currentVpn = dbResult.SingleOrDefault();

            try
            {
                if (ModelState.IsValid)
                {
                    if (currentVpn == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save changes. The vpn was deleted by another user.");
                        return View(vpn);
                    }

                    var topologyType = await VpnService.UnitOfWork.VpnTopologyTypeRepository.GetByIDAsync(vpn.VpnTopologyTypeID);
                    if (currentVpn.VpnTopologyType.VpnProtocolTypeID != topologyType.VpnProtocolTypeID)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save changes. The vpn protocol type cannot be changed.");
                        return View(vpn);
                    }

                    await VpnService.UpdateAsync(Mapper.Map<Vpn>(vpn));
                    return RedirectToAction("GetAll");
                }
            }

            catch (DbUpdateConcurrencyException ex)
            {
                var exceptionEntry = ex.Entries.Single();

                var proposedName = (string)exceptionEntry.Property("Name").CurrentValue;
                if (currentVpn.Name != proposedName)
                {
                    ModelState.AddModelError("Name", $"Current value: {currentVpn.Name}");
                }

                var proposedDescription = (string)exceptionEntry.Property("Description").CurrentValue;
                if (currentVpn.Description != proposedDescription)
                {
                    ModelState.AddModelError("Description", $"Current value: {currentVpn.Description}");
                }

                var proposedPlaneID = (int?)exceptionEntry.Property("PlaneID").CurrentValue;
                if (currentVpn.PlaneID != proposedPlaneID)
                {
                    ModelState.AddModelError("PlaneID", $"Current value: {currentVpn.Plane.Name}");
                }

                var proposedRegionID = (int)exceptionEntry.Property("RegionID").CurrentValue;
                if (currentVpn.RegionID != proposedRegionID)
                {
                    ModelState.AddModelError("RegionID", $"Current value: {currentVpn.Region.Name}");
                }

                var proposedTenancyTypeID = (int)exceptionEntry.Property("VpnTenancyTypeID").CurrentValue;
                if (currentVpn.VpnTenancyTypeID != proposedTenancyTypeID)
                {
                    ModelState.AddModelError("VpnTenancyTypeID", $"Current value: {currentVpn.VpnTenancyType.TenancyType}");
                }

                var proposedTopologyTypeID = (int)exceptionEntry.Property("VpnTopologyTypeID").CurrentValue;
                if (currentVpn.VpnTopologyTypeID != proposedTopologyTypeID)
                {
                    ModelState.AddModelError("VpnTopologyTypeID", $"Current value: {currentVpn.VpnTopologyType.TopologyType}");
                }

                var proposedTenantID = (int)exceptionEntry.Property("TenantID").CurrentValue;
                if (currentVpn.TenantID != proposedTenantID)
                {
                    ModelState.AddModelError("TenantID", $"Current value: {currentVpn.Tenant.Name}");
                }

                var proposedIsExtranet = (bool)exceptionEntry.Property("IsExtranet").CurrentValue;
                if (currentVpn.IsExtranet != proposedIsExtranet)
                {
                    ModelState.AddModelError("IsExtranet", $"Current value: {currentVpn.IsExtranet}");
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

            await PopulatePlanesDropDownList(currentVpn.Plane);
            await PopulateTenantsDropDownList(currentVpn.Tenant);
            await PopulateRegionsDropDownList(currentVpn.Region);
            await PopulateTopologyTypesDropDownList(currentVpn.VpnTopologyType.VpnProtocolTypeID,currentVpn.VpnTopologyType);
            await PopulateTenancyTypesDropDownList(currentVpn.VpnTenancyType);
            return View(Mapper.Map<VpnViewModel>(currentVpn));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vpn = await VpnService.GetByIDAsync(id.Value);
            if (vpn == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("GetAll");
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

            return View(Mapper.Map<VpnViewModel>(vpn));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(VpnViewModel vpn)
        {  
            try
            {
                var dbResult = await VpnService.UnitOfWork.VpnRepository.GetAsync(q => q.VpnID == vpn.VpnID, AsTrackable:false);
                var currentVpn = dbResult.SingleOrDefault();

                if (currentVpn != null)
                {
                    await VpnService.DeleteAsync(Mapper.Map<Vpn>(vpn));
                }
                return RedirectToAction("GetAll");
            }

            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { concurrencyError = true, id = vpn.VpnID });
            }
        }

        private async Task PopulatePlanesDropDownList(object selectedPlane = null)
        {
            var planes = await VpnService.UnitOfWork.PlaneRepository.GetAsync();
            ViewBag.PlaneID = new SelectList(planes, "PlaneID", "Name", selectedPlane);
        }

        private async Task PopulateRegionsDropDownList(object selectedRegion = null)
        {
            var regions = await VpnService.UnitOfWork.RegionRepository.GetAsync();
            ViewBag.RegionID = new SelectList(regions, "RegionID", "Name", selectedRegion);
        }

        private async Task PopulateTenancyTypesDropDownList(object selectedTenancyType = null)
        {
            var tenancyTypes = await VpnService.UnitOfWork.VpnTenancyTypeRepository.GetAsync();
            ViewBag.VpnTenancyTypeID = new SelectList(tenancyTypes, "VpnTenancyTypeID", "TenancyType", selectedTenancyType);
        }

        private async Task PopulateTopologyTypesDropDownList(int protocolTypeID, object selectedTopologyType = null)
        {
            var topologyTypes = await VpnService.UnitOfWork.VpnTopologyTypeRepository.GetAsync(q => q.VpnProtocolTypeID == protocolTypeID);
            ViewBag.VpnTopologyTypeID = new SelectList(topologyTypes, "VpnTopologyTypeID", "TopologyType", selectedTopologyType);
        }

        private async Task PopulateTenantsDropDownList(object selectedTenant = null)
        {
            var tenants = await VpnService.UnitOfWork.TenantRepository.GetAsync();
            ViewBag.TenantID = new SelectList(tenants, "TenantID", "Name", selectedTenant);
        }

        private async Task PopulateProtocolTypesDropDownList(object selectedProtocolType = null)
        {
            var protocolTypes = await VpnService.UnitOfWork.VpnProtocolTypeRepository.GetAsync();
            ViewBag.VpnProtocolTypeID = new SelectList(protocolTypes, "VpnProtocolTypeID", "ProtocolType", selectedProtocolType);
        }
    }
}

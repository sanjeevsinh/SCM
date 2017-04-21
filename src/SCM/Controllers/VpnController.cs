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

namespace SCM.Controllers
{
    public class VpnController : BaseViewController
    {
        public VpnController(IVpnService vpnService, IRouteTargetService routeTargetService, IMapper mapper)
        {
           VpnService = vpnService;
           RouteTargetService = routeTargetService;
           Mapper = mapper;
        }
        private IVpnService VpnService { get; set; }
        private IRouteTargetService RouteTargetService { get; set; }
        private IMapper Mapper { get; set; }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var vpns = await VpnService.GetAllAsync();
            return View(Mapper.Map<List<VpnViewModel>>(vpns));
        }

        [HttpGet]
        public async Task<IActionResult> GetAttachmentsAndVifsByVpnID(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vpn = await VpnService.UnitOfWork.VpnRepository.GetByIDAsync(id);
            if (vpn == null)
            {
                return NotFound();
            }

            var interfaces = await VpnService.UnitOfWork.InterfaceRepository.GetAsync(q => q.Vrf.AttachmentSetVrfs.Where(a => a.AttachmentSet.VpnAttachmentSets
                .Where(v => v.VpnID == id.Value).Count() > 0).Count() > 0,
                includeProperties: "Device.Location.SubRegion.Region,Vrf.AttachmentSetVrfs.AttachmentSet,Device.Plane,Port,"
                + "InterfaceBandwidth,ContractBandwidthPool.ContractBandwidth,Tenant");

            var interfaceVlans = await VpnService.UnitOfWork.InterfaceVlanRepository.GetAsync(q => q.Vrf.AttachmentSetVrfs.Where(a => a.AttachmentSet.VpnAttachmentSets
               .Where(v => v.VpnID == id.Value).Count() > 0).Count() > 0,
               includeProperties: "Interface.Device.Location.SubRegion.Region,Interface.InterfaceBandwidth,Vrf.AttachmentSetVrfs.AttachmentSet,"
               + "Interface.Device.Plane,Interface.Port,Interface.InterfaceBandwidth,Tenant,ContractBandwidthPool.ContractBandwidth");

            var result = Mapper.Map<List<AttachmentOrVifViewModel>>(interfaces).Concat(Mapper.Map<List<AttachmentOrVifViewModel>>(interfaceVlans));
            result = result.OrderBy(q => q.AttachmentSetName);

            ViewBag.Vpn = vpn;

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await VpnService.GetByIDAsync(id.Value);

            if (item == null)
            {
                return NotFound();
            }

            return View(Mapper.Map<VpnViewModel>(item));
        }

        [HttpPost]
        public async Task<IActionResult> Sync(int? id)
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

            var validationOk = true;

            var vpnValidationResult = await VpnService.ValidateAsync(vpn);
            if (!vpnValidationResult.IsSuccess)
            {
                validationOk = false;
                ViewData["ErrorMessage"] = vpnValidationResult.GetHtmlListMessage();
            }

            var routeTargetsValidationResult = RouteTargetService.Validate(vpn);
            if (!routeTargetsValidationResult.IsSuccess)
            {
                validationOk = false;
                ViewData["ErrorMessage"] += routeTargetsValidationResult.GetHtmlListMessage();
            }

            if (validationOk)
            {
                var syncResult = await VpnService.SyncToNetworkAsync(vpn);
                if (syncResult.IsSuccess)
                {
                    ViewData["SuccessMessage"] = "The network is synchronised.";
                }
                else
                {
                    ViewData["ErrorMessage"] = syncResult.GetHtmlListMessage();
                }
            }

            return View("Details", Mapper.Map<VpnViewModel>(vpn));
        }

        [HttpPost]
        public async Task<IActionResult> CheckSync(int? id)
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

            var checkSyncResult = await VpnService.CheckNetworkSyncAsync(vpn);
            if (checkSyncResult.IsSuccess)
            {
                ViewData["SuccessMessage"] = "The VPN is synchronised with the network.";
            }
            else
            {
                if (checkSyncResult.IsSuccess)
                {
                    ViewData["ErrorMessage"] = "The VPN is not synchronised with the network. Press the 'Sync' button to update the network.";
                }
                else
                {
                    ViewData["ErrorMessage"] = checkSyncResult.GetHtmlListMessage();
                }
            }

            return View("Details", Mapper.Map<VpnViewModel>(vpn));
        }

        [HttpGet]
        public async Task<IActionResult> CreateStep1()
        {
            await PopulateProtocolTypesDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStep2([Bind("VpnProtocolTypeID")] VpnProtocolTypeViewModel protocolType)
        {
            await PopulatePlanesDropDownList();
            await PopulateTenantsDropDownList();
            await PopulateRegionsDropDownList();
            await PopulateTopologyTypesDropDownListByProtocolType(protocolType.VpnProtocolTypeID);
            await PopulateTenancyTypesDropDownList();
            ViewBag.VpnProtocolType = await GetProtocolTypeItem(protocolType.VpnProtocolTypeID);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,PlaneID,RegionID,VpnTenancyTypeID,VpnTopologyTypeID,TenantID,IsExtranet")] VpnViewModel vpn)
        {
            if (ModelState.IsValid)
            {
                var mappedVpn = Mapper.Map<Vpn>(vpn);
                var validationResult = await VpnService.ValidateNewAsync(mappedVpn);

                if (!validationResult.IsSuccess)
                {
                    validationResult.GetMessageList().ForEach(message => ModelState.AddModelError(string.Empty, message));
                }
                else
                {
                    var result = await VpnService.AddAsync(mappedVpn);

                    if (!result.IsSuccess)
                    {
                        result.GetMessageList().ForEach(message => ModelState.AddModelError(string.Empty, message));
                    }
                    else
                    {
                        return RedirectToAction("GetAll");
                    }
                }
            }
            
            ViewBag.VpnProtocolType = await GetProtocolTypeByTopologyType(vpn.VpnTopologyTypeID.Value);

            await PopulatePlanesDropDownList(vpn.PlaneID);
            await PopulateTenantsDropDownList(vpn.TenantID);
            await PopulateRegionsDropDownList(vpn.RegionID);
            await PopulateTopologyTypesDropDownList(vpn.VpnTopologyTypeID.Value, vpn.VpnTopologyTypeID);
            await PopulateTenancyTypesDropDownList(vpn.VpnTenancyTypeID);

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

            await PopulatePlanesDropDownList(vpn.PlaneID);
            await PopulateTenantsDropDownList(vpn.TenantID);
            await PopulateRegionsDropDownList(vpn.RegionID);
            await PopulateTopologyTypesDropDownList(vpn.VpnTopologyTypeID, vpn.VpnTopologyTypeID);
            await PopulateTenancyTypesDropDownList(vpn.VpnTenancyTypeID);

            return View(Mapper.Map<VpnViewModel>(vpn));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("VpnID,Name,Description,RegionID,VpnTenancyTypeID,VpnTopologyTypeID,"
            + "TenantID,PlaneID,RegionID,IsExtranet,RowVersion")] VpnViewModel vpn)
        {
            if (id != vpn.VpnID)
            {
                return NotFound();
            }

            var currentVpn = await VpnService.GetByIDAsync(id);

            if (currentVpn == null)
            {
                ModelState.AddModelError(string.Empty, "Unable to save changes. The item was deleted by another user.");
            }
            else 
            {
                // Name property must not be changed because this is used as a key 
                // in the network service model

                // PlaneID and VpnTopologyTypeID must not be changed
                // because these properties affect how the VPN is deployed to the network

                vpn.Name = currentVpn.Name;
                vpn.PlaneID = currentVpn.PlaneID;
                vpn.VpnTopologyTypeID = currentVpn.VpnTopologyTypeID;
                vpn.TenantID = currentVpn.TenantID;
                vpn.RequiresSync = currentVpn.RequiresSync;
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var mappedVpn = Mapper.Map<Vpn>(vpn);
                    var validationResult = await VpnService.ValidateChangesAsync(mappedVpn, currentVpn);

                    if (!validationResult.IsSuccess)
                    {
                        validationResult.GetMessageList().ForEach(message => ModelState.AddModelError(string.Empty, message));
                    }
                    else
                    { 
                        await VpnService.UpdateAsync(mappedVpn);
                        return RedirectToAction("GetAll");
                    }
                }
            }

            catch (DbUpdateConcurrencyException ex)
            {
                var exceptionEntry = ex.Entries.Single();

                var proposedDescription = (string)exceptionEntry.Property("Description").CurrentValue;
                if (currentVpn.Description != proposedDescription)
                {
                    ModelState.AddModelError("Description", $"Current value: {currentVpn.Description}");
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

            await PopulatePlanesDropDownList(currentVpn.PlaneID);
            await PopulateTenantsDropDownList(currentVpn.TenantID);
            await PopulateRegionsDropDownList(currentVpn.RegionID);
            await PopulateTopologyTypesDropDownList(currentVpn.VpnTopologyTypeID, vpn.VpnTopologyTypeID);
            await PopulateTenancyTypesDropDownList(currentVpn.VpnTenancyTypeID);

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
                ViewData["ErrorMessage"] = "The record you attempted to delete "
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
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                var item = await VpnService.UnitOfWork.VpnRepository.GetByIDAsync(id);

                if (item == null)
                {
                    return NotFound();
                }

                // Delete resource from the network first

                var syncResult = await VpnService.DeleteFromNetworkAsync(item);

                // Delete from network may return IsSuccess false if the resource was not found - this should be ignored
                // because it probably means the resource was either previously deleted from the network or it was 
                // never syncd to the network

                var inSync = true;
                if (!syncResult.IsSuccess)
                {
                    foreach (var r in syncResult.NetworkSyncServiceResults)
                    {
                        if (r.HttpStatusCode != HttpStatusCode.NotFound)
                        {
                            // Something went wrong, so flag for exit

                            inSync = false;
                            ViewData["ErrorMessage"] = syncResult.GetHtmlListMessage();
                        }
                    }
                }

                if (inSync)
                {
                    var result = await VpnService.DeleteAsync(item);
                    if (result.IsSuccess)
                    {
                        return RedirectToAction("GetAll");
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = result.GetHtmlListMessage();
                    }
                }

                return View(Mapper.Map<VpnViewModel>(item));
            }

            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { concurrencyError = true, id = id });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFromNetwork(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vpn = await VpnService.GetByIDAsync(id.Value);
            if (vpn == null)
            {

                ViewData["VpnDeletedMessage"] = "The VPN has been deleted by another user. Return to the list.";
                return View("VpnDeleted");
            }

            var syncResult = await VpnService.DeleteFromNetworkAsync(vpn);
            if (syncResult.IsSuccess)
            {
                ViewData["SuccessMessage"] = "The VPN has been deleted from the network.";
            }
            else
            {
                ViewData["ErrorMessage"] = syncResult.GetHtmlListMessage();
            }

            vpn.RequiresSync = true;
            return View("Delete", Mapper.Map<VpnViewModel>(vpn));
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

        private async Task PopulateTopologyTypesDropDownListByProtocolType(int protocolTypeID, object selectedTopologyType = null)
        {
            var topologyTypes = await VpnService.UnitOfWork.VpnTopologyTypeRepository.GetAsync(q => q.VpnProtocolTypeID == protocolTypeID);
            ViewBag.VpnTopologyTypeID = new SelectList(topologyTypes, "VpnTopologyTypeID", "TopologyType", selectedTopologyType);
        }

        private async Task PopulateTopologyTypesDropDownList(int topologyTypeID, object selectedTopologyType = null)
        {
            var topologyType = await VpnService.UnitOfWork.VpnTopologyTypeRepository.GetByIDAsync(topologyTypeID);
            await PopulateTopologyTypesDropDownListByProtocolType(topologyType.VpnProtocolTypeID);
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

        private async Task<VpnProtocolType> GetProtocolTypeItem(int protocolTypeID)
        {
            return await VpnService.UnitOfWork.VpnProtocolTypeRepository.GetByIDAsync(protocolTypeID);
        }

        private async Task<VpnProtocolType> GetProtocolTypeByTopologyType(int topologyTypeID)
        {
            var dbResult = await VpnService.UnitOfWork.VpnTopologyTypeRepository.GetAsync(q => q.VpnTopologyTypeID == topologyTypeID, includeProperties:"VpnProtocolType");
            return dbResult.Single().VpnProtocolType;

        }
    }
}

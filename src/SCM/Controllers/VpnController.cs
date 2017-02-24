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

            var dbResult = await VpnService.UnitOfWork.VpnRepository.GetAsync(q => q.VpnID == id, 
                includeProperties: "Region,Plane,VpnTenancyType,VpnTopologyType.VpnProtocolType,Tenant");
            var item = dbResult.SingleOrDefault();

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
            var dbResult = await VpnService.UnitOfWork.VpnRepository.GetAsync(q => q.VpnID == id,
                includeProperties: "Region,Plane,VpnTenancyType,VpnTopologyType.VpnProtocolType,Tenant");
            var item = dbResult.SingleOrDefault();

            if (item == null)
            {
                return NotFound();
            }

            var syncResult = await VpnService.SyncToNetworkAsync(id.Value);
     
            if (syncResult.IsSuccess)
            {
                ViewData["SyncSuccessMessage"] = "The network is synchronised.";
            }
            else
            {
                ViewData["SyncErrorMessage"] = syncResult.GetMessage();
            }

            return View("Details", Mapper.Map<VpnViewModel>(item));
        }

        [HttpPost]
        public async Task<IActionResult> CheckSync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var checkSyncResult = await VpnService.CheckNetworkSyncAsync(id.Value);
            if (checkSyncResult.InSync)
            {
                ViewData["SyncSuccessMessage"] = "The VPN is synchronised with the network.";
            }
            else
            {
                if (checkSyncResult.NetworkSyncServiceResult.IsSuccess)
                {
                    ViewData["SyncErrorMessage"] = "The VPN is not synchronised with the network. Press the 'Sync' button to update the network.";
                }
                else
                {
                    var message = checkSyncResult.NetworkSyncServiceResult.GetAllMessages();
                    ViewData["SyncErrorMessage"] = message;
                }
  
            }

            var dbResult = await VpnService.UnitOfWork.VpnRepository.GetAsync(q => q.VpnID == id,
            includeProperties: "Region,Plane,VpnTenancyType,VpnTopologyType.VpnProtocolType,Tenant");
            var item = dbResult.SingleOrDefault();

            if (item == null)
            {
                return NotFound();
            }

            return View("Details", Mapper.Map<VpnViewModel>(item));
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
            try
            {
                if (ModelState.IsValid)
                {
                    var mappedVpn = Mapper.Map<Vpn>(vpn);
                    var validationResult = await VpnService.ValidateCreateVpnAsync(mappedVpn);

                    if (!validationResult.IsSuccess)
                    {
                        ModelState.AddModelError(string.Empty, validationResult.GetMessage());
                    }
                    else
                    {
                        await VpnService.AddAsync(mappedVpn);
                        return RedirectToAction("GetAll");
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

            ViewBag.VpnProtocolType = await GetProtocolTypeByTopologyType(vpn.VpnTopologyTypeID);

            await PopulatePlanesDropDownList(vpn.PlaneID);
            await PopulateTenantsDropDownList(vpn.TenantID);
            await PopulateRegionsDropDownList(vpn.RegionID);
            await PopulateTopologyTypesDropDownList(vpn.VpnTopologyTypeID, vpn.VpnTopologyTypeID);
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
        public async Task<ActionResult> Edit(int id, [Bind("VpnID,Name,Description,RegionID,VpnTenancyTypeID,IsExtranet,RowVersion")] VpnViewModel vpn)
        {
            if (id != vpn.VpnID)
            {
                return NotFound();
            }

            var dbResult = await VpnService.UnitOfWork.VpnRepository.GetAsync(q => q.VpnID == id,
                includeProperties: "Plane,Region,VpnTenancyType,VpnTopologyType.VpnProtocolType,Tenant,VpnAttachmentSets.AttachmentSet.Tenant,"
                 + "VpnAttachmentSets.AttachmentSet.AttachmentSetVrfs.Vrf.Device.Location.SubRegion.Region",
                AsTrackable: false);

            var currentVpn = dbResult.SingleOrDefault();

            if (currentVpn == null)
            {
                return NotFound();
            }

            // Name property must not be changed because this is used as a key 
            // in the network service model

            // PlaneID and VpnTopologyTypeID must not be changed
            // because these properties affect how the VPN is deployed to the network

            vpn.Name = currentVpn.Name;
            vpn.PlaneID = currentVpn.PlaneID;
            vpn.VpnTopologyTypeID = currentVpn.VpnTopologyTypeID;
            vpn.TenantID = currentVpn.TenantID;

            try
            {
                if (ModelState.IsValid)
                {
                    var mappedVpn = Mapper.Map<Vpn>(vpn);
                    var validationResult = await VpnService.ValidateVpnChangesAsync(mappedVpn, currentVpn);

                    if (!validationResult.IsSuccess)
                    {
                        ModelState.AddModelError(string.Empty, validationResult.GetMessage());
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
        public async Task<IActionResult> Delete(int? id)
        {  
            try
            {
                var dbResult = await VpnService.UnitOfWork.VpnRepository.GetAsync(q => q.VpnID == id, AsTrackable:false);
                var currentVpn = dbResult.SingleOrDefault();

                if (currentVpn != null)
                {
                    var result = await VpnService.DeleteAsync(Mapper.Map<Vpn>(currentVpn));
                    if (!result.IsSuccess)
                    {
                        ViewData["DeleteErrorMessage"] = result.GetMessage();
                        return View(Mapper.Map<VpnViewModel>(currentVpn));
                    }
                }
                return RedirectToAction("GetAll");
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

            var syncResult = await VpnService.DeleteFromNetworkAsync(id.Value);
            if (syncResult.IsSuccess)
            {
                ViewData["SyncSuccessMessage"] = "The VPN has been deleted from the network.";
            }
            else
            {
                if (syncResult.NetworkHttpResponse.HttpStatusCode == HttpStatusCode.NotFound)
                {
                    ViewData["SyncErrorMessage"] = "The VPN has already been deleted from the network.";
                }
                else
                {
                    ViewData["SyncErrorMessage"] = "There was a problem deleting the VPN from the network. " + syncResult.GetAllMessages();
                }
            }

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

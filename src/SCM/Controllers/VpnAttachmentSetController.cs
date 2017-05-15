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
    public class VpnAttachmentSetController : BaseViewController
    {
        public VpnAttachmentSetController(IVpnAttachmentSetService vpnAttachmentSetService, 
            IVpnService vpnService, 
            IAttachmentSetService attachmentSetService, 
            IAttachmentSetVrfService attachmentSetVrfService, 
            IMapper mapper)
        {
           VpnAttachmentSetService = vpnAttachmentSetService;
           VpnService = vpnService;
           AttachmentSetService = attachmentSetService;
           AttachmentSetVrfService = attachmentSetVrfService;
           Mapper = mapper;
        }
        private IVpnAttachmentSetService VpnAttachmentSetService { get; set; }
        private IVpnService VpnService { get; set; }
        private IAttachmentSetService AttachmentSetService { get; set; }
        private IAttachmentSetVrfService AttachmentSetVrfService { get; set; }
        private IMapper Mapper { get; set; }

        [HttpGet]
        public async Task<IActionResult> GetAllByVpnID(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vpn = await GetVpn(id.Value);
            ViewBag.Vpn = vpn;

            var vpnAttachmentSets = await VpnAttachmentSetService.UnitOfWork.VpnAttachmentSetRepository.GetAsync(q => q.VpnID == id.Value,
                includeProperties:"AttachmentSet.Tenant");

            return View(Mapper.Map<List<VpnAttachmentSetViewModel>>(vpnAttachmentSets));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbResult = await VpnAttachmentSetService.UnitOfWork.VpnAttachmentSetRepository.GetAsync(q => q.VpnAttachmentSetID == id, 
                includeProperties: "AttachmentSet.Tenant,Vpn");
            var item = dbResult.SingleOrDefault();

            if (item == null)
            {
                return NotFound();
            }

            ViewBag.Vpn = await GetVpn(item.VpnID);
            return View(Mapper.Map<VpnAttachmentSetViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> CreateStep1(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await PopulateTenantsDropDownList(id.Value);
            var vpn = await GetVpn(id.Value);
            ViewBag.Vpn = vpn;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStep2([Bind("TenantID,VpnID")] VpnAttachmentSetRequestViewModel vpnAttachmentSetRequest)
        {
            await PopulateAttachmentSetsDropDownList(vpnAttachmentSetRequest);
            ViewBag.Vpn = await GetVpn(vpnAttachmentSetRequest.VpnID);
            ViewBag.VpnAttachmentSetSelection = vpnAttachmentSetRequest;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AttachmentSetID,VpnID,IsHub")] VpnAttachmentSetViewModel vpnAttachmentSet,
            [Bind("VpnID,TenantID")] VpnAttachmentSetRequestViewModel vpnAttachmentSetRequest)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var vpn = await VpnService.GetByIDAsync(vpnAttachmentSet.VpnID);
                    var attachmentSet = await AttachmentSetService.GetByIDAsync(vpnAttachmentSet.AttachmentSetID);

                    if (vpn == null || attachmentSet == null)
                    {
                        return NotFound();
                    }

                    var mappedAttachmentSet = Mapper.Map<VpnAttachmentSet>(vpnAttachmentSet);

                    var validationOk = true;
                    var validationResult = VpnAttachmentSetService.ValidateNew(mappedAttachmentSet, vpn, attachmentSet);
                    if (!validationResult.IsSuccess)
                    {
                        validationResult.GetMessageList().ForEach(message => ModelState.AddModelError(string.Empty, message));
                        validationOk = false;
                    }

                    // Check that the vrfs in the attachment set are correctly configured

                    var attachmentSetVrfValidationResult = await AttachmentSetVrfService.CheckVrfsConfiguredCorrectlyAsync(attachmentSet);
                    if (!attachmentSetVrfValidationResult.IsSuccess)
                    {
                        ModelState.AddModelError(string.Empty, "There is a problem with the VRFs for this Attachment Set. Resolve this issue first.");
                        attachmentSetVrfValidationResult.GetMessageList().ForEach(message => ModelState.AddModelError(string.Empty, message));
                        validationOk = false;
                    }

                    if (validationOk)
                    {
                        await VpnService.UpdateVpnRequiresSyncAsync(mappedAttachmentSet.VpnID, true, false);
                        await VpnAttachmentSetService.AddAsync(mappedAttachmentSet);

                        return RedirectToAction("GetAllByVpnID", new { id = vpnAttachmentSet.VpnID });
                    }
                }
            }
            catch (DbUpdateException  /** ex **/ )
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            ViewBag.Vpn = await GetVpn(vpnAttachmentSet.VpnID);
            ViewBag.VpnAttachmentSetSelection = vpnAttachmentSetRequest;
            await PopulateAttachmentSetsDropDownList(vpnAttachmentSetRequest);

            return View("CreateStep2", vpnAttachmentSet);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vpnAttachmentSet = await VpnAttachmentSetService.GetByIDAsync(id.Value);

            if (vpnAttachmentSet == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("GetAllByVpnID", new { id = vpnAttachmentSet.VpnID });
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

            ViewBag.Vpn = await GetVpn(vpnAttachmentSet.VpnID);
            return View(Mapper.Map<VpnAttachmentSetViewModel>(vpnAttachmentSet));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(VpnAttachmentSetViewModel vpnAttachmentSet)
        {  
            try
            {
                var currentVpnAttachmentSet = await VpnAttachmentSetService.GetByIDAsync(vpnAttachmentSet.VpnAttachmentSetID);

                if (currentVpnAttachmentSet != null)
                {
                    await VpnService.UpdateVpnRequiresSyncAsync(currentVpnAttachmentSet.VpnID, true, false);
                    await VpnAttachmentSetService.DeleteAsync(currentVpnAttachmentSet);
                }

                return RedirectToAction("GetAllByVpnID", new { id = vpnAttachmentSet.VpnID });
            }

            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { concurrencyError = true, id = vpnAttachmentSet.VpnAttachmentSetID });
            }
        }

        private async Task PopulateAttachmentSetsDropDownList(VpnAttachmentSetRequestViewModel vpnAttachmentSetRequest, object selectedAttachmentSet = null)
        {

            var dbResult = await VpnAttachmentSetService.UnitOfWork.VpnRepository.GetAsync(q => q.VpnID == vpnAttachmentSetRequest.VpnID, 
                includeProperties:"VpnTopologyType.VpnProtocolType");
            var vpn = dbResult.Single();
            var layer3 = vpn.VpnTopologyType.VpnProtocolType.ProtocolType == "IP" ? true : false;

            var attachmentSets = await VpnAttachmentSetService.UnitOfWork.AttachmentSetRepository.GetAsync(q => 
                        q.TenantID == vpnAttachmentSetRequest.TenantID 
                        && q.IsLayer3 == layer3);

            if (vpn.RegionID != null)
            {
                attachmentSets = attachmentSets.Where(q => q.RegionID == vpn.RegionID).ToList();
            }

            ViewBag.AttachmentSetID = new SelectList(attachmentSets, "AttachmentSetID", "Name", selectedAttachmentSet);
        }

        private async Task PopulateTenantsDropDownList(int vpnID, object selectedTenant = null)
        {
            var dbResult = await VpnAttachmentSetService.UnitOfWork.VpnRepository.GetAsync(q => q.VpnID == vpnID, 
                includeProperties: "VpnTenancyType");

            var vpn = dbResult.Single();
            IEnumerable<Tenant> tenants;

            // Get all Tenants if the VPN is multi-tenant, other get only the tenant owner

            if (vpn.VpnTenancyType.TenancyType == "Multi")
            {
                tenants = await VpnAttachmentSetService.UnitOfWork.TenantRepository.GetAsync();
            }
            else
            {
                tenants = await VpnAttachmentSetService.UnitOfWork.TenantRepository.GetAsync(q => q.TenantID == vpn.TenantID);
            }

            ViewBag.TenantID = new SelectList(tenants, "TenantID", "Name", selectedTenant);
        }

        private async Task<Vpn> GetVpn(int vpnID)
        {
            var dbResult = await VpnAttachmentSetService.UnitOfWork.VpnRepository.GetAsync(q => q.VpnID == vpnID, 
                includeProperties:"VpnTopologyType");

            return dbResult.SingleOrDefault();
           
        }
    }
}

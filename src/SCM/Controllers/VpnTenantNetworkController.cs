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
    public class VpnTenantNetworkController : BaseViewController
    {
        public VpnTenantNetworkController(IVpnTenantNetworkService vpnTenantNetworkService, 
            IVpnAttachmentSetService vpnAttachmentSetService, IMapper mapper)
        {
            VpnTenantNetworkService = vpnTenantNetworkService;
            VpnAttachmentSetService = vpnAttachmentSetService;
            Mapper = mapper;
        }
        private IVpnTenantNetworkService VpnTenantNetworkService { get; set; }
        private IVpnAttachmentSetService VpnAttachmentSetService { get; set; }
        private IMapper Mapper { get; set; }

        [HttpGet]
        public async Task<IActionResult> GetAllByVpnAttachmentSetID(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vpnTenantNetworks = await VpnTenantNetworkService.UnitOfWork.VpnTenantNetworkRepository.GetAsync(q => q.VpnAttachmentSetID == id,
                includeProperties:"TenantNetwork");
            var vpnAttachmentSet = await GetVpnAttachmentSetItem(id.Value);
            if (vpnAttachmentSet == null)
            {
                return NotFound();
            }

            ViewBag.VpnAttachmentSet = vpnAttachmentSet;
            return View(Mapper.Map<List<VpnTenantNetworkViewModel>>(vpnTenantNetworks));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbResult = await VpnTenantNetworkService.UnitOfWork.VpnTenantNetworkRepository.GetAsync(q => q.VpnTenantNetworkID == id, 
                includeProperties: "VpnAttachmentSet.Vpn,VpnAttachmentSet.AttachmentSet.Tenant,TenantNetwork");
            var item = dbResult.SingleOrDefault();

            if (item == null)
            {
                return NotFound();
            }
            return View(Mapper.Map<VpnTenantNetworkViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vpnAttachmentSet = await GetVpnAttachmentSetItem(id.Value);
            if (vpnAttachmentSet == null)
            {
                return NotFound();
            }

            ViewBag.VpnAttachmentSet = vpnAttachmentSet;
            await PopulateTenantNetworksDropDownList(id.Value);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenantNetworkID,VpnAttachmentSetID")] VpnTenantNetworkViewModel vpnTenantNetwork)
        {
            var vpnAttachmentSet = await VpnAttachmentSetService.GetByIDAsync(vpnTenantNetwork.VpnAttachmentSetID);
            if (vpnAttachmentSet == null)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var mappedVpnTenantNetwork = Mapper.Map<VpnTenantNetwork>(vpnTenantNetwork);
                    var validationResult = await VpnTenantNetworkService.ValidateNewAsync(mappedVpnTenantNetwork, vpnAttachmentSet);

                    if (!validationResult.IsSuccess)
                    {
                        validationResult.GetMessageList().ForEach(message => ModelState.AddModelError(string.Empty, message));
                    }
                    else
                    {
                        await VpnTenantNetworkService.AddAsync(mappedVpnTenantNetwork);

                        return RedirectToAction("GetAllByVpnAttachmentSetID", new { id = vpnTenantNetwork.VpnAttachmentSetID });
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

            ViewBag.VpnAttachmentSet = vpnAttachmentSet;
            await PopulateTenantNetworksDropDownList(vpnTenantNetwork.VpnAttachmentSetID);

            return View(vpnTenantNetwork);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbResult = await VpnTenantNetworkService.UnitOfWork.VpnTenantNetworkRepository.GetAsync(q => q.VpnTenantNetworkID == id.Value,
                includeProperties: "TenantNetwork,VpnAttachmentSet.Vpn");
            var vpnTenantNetwork = dbResult.SingleOrDefault();

            if (vpnTenantNetwork == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("GetAllByVpnAttachmentSetID", new { id = vpnTenantNetwork.VpnAttachmentSetID });
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

            return View(Mapper.Map<VpnTenantNetworkViewModel>(vpnTenantNetwork));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(VpnTenantNetworkViewModel vpnTenantNetwork)
        {  
            try
            {
                var dbResult = await VpnTenantNetworkService.UnitOfWork.VpnTenantNetworkRepository.GetAsync(q => q.VpnTenantNetworkID == vpnTenantNetwork.VpnTenantNetworkID, AsTrackable:false);
                var currentVpnTenantNetwork = dbResult.SingleOrDefault();

                if (currentVpnTenantNetwork != null)
                {
                    await VpnTenantNetworkService.DeleteAsync(Mapper.Map<VpnTenantNetwork>(vpnTenantNetwork));
                }
                return RedirectToAction("GetAllByVpnAttachmentSetID", new { id = vpnTenantNetwork.VpnAttachmentSetID });
            }

            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { concurrencyError = true, id = vpnTenantNetwork.VpnTenantNetworkID });
            }
        }

        private async Task PopulateTenantNetworksDropDownList(int vpnAttachmentSetID, object selectedTenantNetwork = null)
        {

            var dbResult1 = await VpnTenantNetworkService.UnitOfWork.VpnAttachmentSetRepository.GetAsync(q => q.VpnAttachmentSetID == vpnAttachmentSetID, 
                includeProperties:"AttachmentSet");
            var vpnAttachmentSet = dbResult1.SingleOrDefault();

            if (vpnAttachmentSet != null)
            {
                var dbResult2 = await VpnTenantNetworkService.UnitOfWork.TenantNetworkRepository.GetAsync(q => q.TenantID == vpnAttachmentSet.AttachmentSet.TenantID);
                var tenantNetworks = dbResult2.Select(p => new { TenantNetworkID = p.TenantNetworkID, TenantNetwork = string.Concat(p.IpPrefix, "/", p.Length) });

                ViewBag.TenantNetworkID = new SelectList(tenantNetworks, "TenantNetworkID", "TenantNetwork", selectedTenantNetwork);
            }
        }
        private async Task<VpnAttachmentSet> GetVpnAttachmentSetItem(int vpnAttachmentSetID)
        {
            var dbResult = await VpnTenantNetworkService.UnitOfWork.VpnAttachmentSetRepository.GetAsync(q => q.VpnAttachmentSetID == vpnAttachmentSetID,
                includeProperties: "AttachmentSet.Tenant,Vpn");

           return dbResult.SingleOrDefault();
        }
    }
}

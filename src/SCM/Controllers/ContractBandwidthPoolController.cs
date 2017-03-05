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
    public class ContractBandwidthPoolController : BaseViewController
    {
        public ContractBandwidthPoolController(IContractBandwidthPoolService contractBandwidthPoolService, IMapper mapper)
        {
           ContractBandwidthPoolService = contractBandwidthPoolService;
           Mapper = mapper;
        }
        private IContractBandwidthPoolService ContractBandwidthPoolService { get; set; }
        private IMapper Mapper { get; set; }

        [HttpGet]
        public async Task<IActionResult> GetAllByTenantID(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contractBandwidthPools = await ContractBandwidthPoolService.UnitOfWork.ContractBandwidthPoolRepository.GetAsync(q => q.TenantID == id,
                includeProperties:"ContractBandwidth");
            await PopulateTenantItem(id.Value);

            return View(Mapper.Map<List<ContractBandwidthPoolViewModel>>(contractBandwidthPools));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await ContractBandwidthPoolService.GetByIDAsync(id.Value);
            if (item == null)
            {
                return NotFound();
            }

            await PopulateTenantItem(item.TenantID);
            return View(Mapper.Map<ContractBandwidthPoolViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await PopulateContractBandwidthsDropDownList();
            await PopulateTenantItem(id.Value);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContractBandwidthID,Name,TrustReceivedCosDscp,TenantID")] ContractBandwidthPoolViewModel contractBandwidthPool)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await ContractBandwidthPoolService.AddAsync(Mapper.Map<ContractBandwidthPool>(contractBandwidthPool));

                    return RedirectToAction("GetAllByTenantID", new { id = contractBandwidthPool.TenantID });
                }
            }
            catch (DbUpdateException /** ex **/ )
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            await PopulateContractBandwidthsDropDownList();
            await PopulateTenantItem(contractBandwidthPool.TenantID); 
            return View(Mapper.Map<ContractBandwidthPoolViewModel>(contractBandwidthPool));
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ContractBandwidthPool contractBandwidthPool = await ContractBandwidthPoolService.GetByIDAsync(id.Value);

            if (contractBandwidthPool == null)
            {
                return NotFound();
            }

            await PopulateContractBandwidthsDropDownList();
            await PopulateTenantItem(contractBandwidthPool.TenantID);
            return View(Mapper.Map<ContractBandwidthPoolViewModel>(contractBandwidthPool));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("ContractBandwidthPoolID,TenantID,TrustReceivedCosDscp,ContractBandwidthID,RowVersion")] ContractBandwidthPoolViewModel contractBandwidthPool)
        {
            if (id != contractBandwidthPool.ContractBandwidthPoolID)
            {
                return NotFound();
            }

            var dbResult = await ContractBandwidthPoolService.UnitOfWork.ContractBandwidthPoolRepository.GetAsync(filter: d => d.ContractBandwidthPoolID == id,
                AsTrackable: false);
            var currentContractBandwidthPool = dbResult.SingleOrDefault();

            try
            {
                if (ModelState.IsValid)
                {
                    if (currentContractBandwidthPool == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save changes. The Contract Bandwidth Pool was deleted by another user.");

                        await PopulateContractBandwidthsDropDownList();
                        await PopulateTenantItem(contractBandwidthPool.TenantID);
                        return View(contractBandwidthPool);
                    }

                    await ContractBandwidthPoolService.UpdateAsync(Mapper.Map<ContractBandwidthPool>(contractBandwidthPool));

                    return RedirectToAction("GetAllByTenantID", new { id = contractBandwidthPool.TenantID });
                }
            }

            catch (DbUpdateConcurrencyException ex)
            {
                var exceptionEntry = ex.Entries.Single();

                var proposedName = (string)exceptionEntry.Property("Name").CurrentValue;
                if (currentContractBandwidthPool.Name != proposedName)
                {
                    ModelState.AddModelError("Name", $"Current value: {currentContractBandwidthPool.Name}");
                }

                var proposedContractBandwidthID = (int)exceptionEntry.Property("ContractBandwidthID").CurrentValue;
                if (currentContractBandwidthPool.ContractBandwidthID != proposedContractBandwidthID)
                {
                    ModelState.AddModelError("ContractBandwidthID", $"Current value: {currentContractBandwidthPool.ContractBandwidthID}");
                }

                var proposedTrustReceivedCosDscp = (bool)exceptionEntry.Property("TrustReceivedCosDscp").CurrentValue;
                if (currentContractBandwidthPool.TrustReceivedCosDscp != proposedTrustReceivedCosDscp)
                {
                    ModelState.AddModelError("TrustReceivedCosDscp", $"Current value: {currentContractBandwidthPool.TrustReceivedCosDscp}");
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

            await PopulateContractBandwidthsDropDownList();
            await PopulateTenantItem(currentContractBandwidthPool.TenantID);
            return View(Mapper.Map<ContractBandwidthPoolViewModel>(currentContractBandwidthPool));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contractBandwidthPool = await ContractBandwidthPoolService.GetByIDAsync(id.Value);
            if (contractBandwidthPool == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("GetAllByTenantID", new { id = contractBandwidthPool.TenantID });
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

            await PopulateContractBandwidthsDropDownList();
            await PopulateTenantItem(contractBandwidthPool.TenantID);
            return View(Mapper.Map<ContractBandwidthPoolViewModel>(contractBandwidthPool));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ContractBandwidthPoolViewModel contractBandwidthPool)
        {
            try
            {
                var dbResult = await ContractBandwidthPoolService.UnitOfWork.ContractBandwidthPoolRepository.GetAsync(filter: d => d.ContractBandwidthPoolID == contractBandwidthPool.ContractBandwidthPoolID, AsTrackable: false);
                var currentContractBandwidthPool = dbResult.SingleOrDefault();

                if (currentContractBandwidthPool != null)
                {
                    await ContractBandwidthPoolService.DeleteAsync(Mapper.Map<ContractBandwidthPool>(contractBandwidthPool));
                }

                return RedirectToAction("GetAllByTenantID", new { id = contractBandwidthPool.TenantID });
            }

            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { concurrencyError = true, id = contractBandwidthPool.ContractBandwidthPoolID });
            }
        }

        private async Task PopulateTenantItem(int tenantID)
        {
            var dbResult = await ContractBandwidthPoolService.UnitOfWork.TenantRepository.GetAsync(q => q.TenantID == tenantID);
            var tenant = dbResult.SingleOrDefault();
            if (tenant != null)
            {
                ViewBag.Tenant = tenant;
            }
        }

        private async Task PopulateContractBandwidthsDropDownList(object selectedContractBandwidth = null)
        {
            var contractBandwidths = await ContractBandwidthPoolService.UnitOfWork.ContractBandwidthRepository.GetAsync();
            ViewBag.ContractBandwidthID = new SelectList(contractBandwidths, "ContractBandwidthID", "BandwidthKbps", selectedContractBandwidth);
        }
    }
}

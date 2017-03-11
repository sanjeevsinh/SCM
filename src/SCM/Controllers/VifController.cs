using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SCM.Models.ServiceModels;
using SCM.Models.ViewModels;
using SCM.Services;
using SCM.Services.SCMServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SCM.Controllers
{
    public class VifController : BaseViewController
    {
        public VifController(IVifService vifService, IAttachmentService attachmentService, IMapper mapper)
        {
            VifService = vifService;
            AttachmentService = attachmentService;
            Mapper = mapper;
        }
        
        private IAttachmentService AttachmentService { get; set; } 
        private IVifService VifService { get; set; }
        private IMapper Mapper { get; set; }

        [HttpGet]
        public async Task<IActionResult> GetByID(int id)
        {
            var item = await VifService.GetByIDAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(Mapper.Map<VifViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllByAttachmentID(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.Attachment = await AttachmentService.GetByIDAsync(id.Value);
            var vifs = await VifService.GetAllByAttachmentIDAsync(id.Value);

            return View(Mapper.Map<List<VifViewModel>>(vifs));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await VifService.GetByIDAsync(id.Value);

            if (item == null)
            {
                return NotFound();
            }

            ViewBag.Attachment = await AttachmentService.GetByIDAsync(id.Value);
            return View(Mapper.Map<VifViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attachment =  await AttachmentService.GetByIDAsync(id.Value);
            ViewBag.Attachment = attachment;
            await PopulateTenantsDropDownList(attachment.TenantID);
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AttachmentID,IpAddress,IsLayer3,SubnetMask,VlanTag,VrfID,TenantID")] VifRequestViewModel request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var mappedRequest = Mapper.Map<VifRequest>(request);
                    
                    var validationResult = await VifService.ValidateAsync(mappedRequest);

                    if (!validationResult.IsSuccess)
                    {
                        validationResult.GetMessageList().ForEach(message => ModelState.AddModelError(string.Empty, message));
                    }
                    else
                    {
                        var result = await VifService.AddAsync(mappedRequest);
                        if (!result.IsSuccess)
                        {
                            result.GetMessageList().ForEach(message => ModelState.AddModelError(string.Empty, message));
                        }
                        else
                        {
                            return RedirectToAction("GetAllByAttachmentID", new { id = request.AttachmentID });
                        }
                    }
                }
            }
            catch (DbUpdateException)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            var attachment = await AttachmentService.GetByIDAsync(request.AttachmentID);
            ViewBag.Attachment = attachment;
            await PopulateTenantsDropDownList(attachment.TenantID);

            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vif = await VifService.GetByIDAsync(id.Value);
            if (vif == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("GetAllByAttachmentID", new { id = vif.AttachmentID });
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

            ViewBag.Attachment = await AttachmentService.GetByIDAsync(vif.AttachmentID);
            return View(Mapper.Map<VifViewModel>(vif));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Vif vif)
        {
            try
            {
                var currentVif = await VifService.GetByIDAsync(vif.ID);

                if (currentVif != null)
                {
                    await VifService.DeleteAsync(currentVif);
                }
                return RedirectToAction("GetAllByAttachmentID", new { id = Request.Query["AttachmentID"] });
            }

            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { concurrencyError = true, id = vif.ID });
            }
        }
        private async Task PopulateTenantsDropDownList(object selectedTenant = null)
        {
            var tenants = await VifService.UnitOfWork.TenantRepository.GetAsync();
            ViewBag.TenantID = new SelectList(tenants, "TenantID", "Name", selectedTenant);
        }
        private async Task PopulateContractBandwidthPoolsDropDownList(int tenantID, object selectedContractBandwidthPool = null)
        {
            var contractBandwidthPools = await VifService.UnitOfWork.ContractBandwidthPoolRepository.GetAsync(q => q.TenantID == tenantID);
            ViewBag.ContractBandwidthPoolID = new SelectList(contractBandwidthPools, "ContractBandwidthPoolID", "Name", selectedContractBandwidthPool);
        }
    }
}
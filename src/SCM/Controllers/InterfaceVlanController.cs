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
    public class InterfaceVlanController : BaseViewController
    {
        public InterfaceVlanController(IInterfaceVlanService interfaceVlanService, IMapper mapper)
        {
            InterfaceVlanService = interfaceVlanService;
            Mapper = mapper;
        }
        private IInterfaceVlanService InterfaceVlanService { get; set; }
        private IMapper Mapper { get; set; }

        [HttpGet]
        public async Task<IActionResult> GetByID(int id)
        {
            var item = await InterfaceVlanService.GetByIDAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(Mapper.Map<InterfaceVlanViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllByInterfaceID(int id)
        {
            var dbResult = await InterfaceVlanService.UnitOfWork.InterfaceRepository.GetAsync(q => q.ID == id, includeProperties:"Port");
            var iface = dbResult.SingleOrDefault();
            if (iface == null)
            {
                return NotFound();
            }

            var ifaceVlans = await InterfaceVlanService.UnitOfWork.InterfaceVlanRepository.GetAsync(q => q.InterfaceID == id,
                includeProperties: "Interface,Vrf");

            ViewBag.Interface = iface;
            return View(Mapper.Map<List<InterfaceVlanViewModel>>(ifaceVlans));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbResult = await InterfaceVlanService.UnitOfWork.InterfaceVlanRepository.GetAsync(q => q.InterfaceVlanID == id.Value,
                includeProperties: "Interface,Vrf");
            var item = dbResult.SingleOrDefault();

            if (item == null)
            {
                return NotFound();
            }

            await PopulateInterfaceItem(item.InterfaceID);
            return View(Mapper.Map<InterfaceVlanViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? id) 
        {
            if (id == null)
            {
                return NotFound();
            }

            await PopulateVrfsDropDownList(id.Value);
            await PopulateInterfaceItem(id.Value);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InterfaceID,IpAddress,IsLayer3,SubnetMask,VlanTag,VrfID")] InterfaceVlanViewModel ifaceVlan)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var mapped = Mapper.Map<InterfaceVlan>(ifaceVlan);
                    if (!await ValidateInterfaceVlan(mapped.InterfaceID))
                    {
                        ModelState.AddModelError(string.Empty, "A vlan cannot be created on an untagged interface.");
                    }
                    else
                    {
                        await InterfaceVlanService.AddAsync(mapped);
                        return RedirectToAction("GetAllByInterfaceID", new { id = ifaceVlan.InterfaceID });
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

            await PopulateVrfsDropDownList(ifaceVlan.InterfaceID);
            await PopulateInterfaceItem(ifaceVlan.InterfaceID);
            return View(ifaceVlan);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            InterfaceVlan ifaceVlan = await InterfaceVlanService.GetByIDAsync(id.Value);

            if (ifaceVlan == null)
            {
                return NotFound();
            }

            await PopulateInterfaceItem(ifaceVlan.InterfaceID);
            await PopulateVrfsDropDownList(ifaceVlan.InterfaceID);
            return View(Mapper.Map<InterfaceVlanViewModel>(ifaceVlan));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("InterfaceVlanID,InterfaceID,IsLayer3,IpAddress,SubnetMask,VlanTag,VrfID,RowVersion")] InterfaceVlanViewModel ifaceVlan)
        {
            if (id != ifaceVlan.InterfaceVlanID)
            {
                return NotFound();
            }

            var dbResult = await InterfaceVlanService.UnitOfWork.InterfaceVlanRepository.GetAsync(filter: d => d.InterfaceVlanID == id,
               includeProperties:"Vrf", AsTrackable: false);
            var currentInterfaceVlan = dbResult.SingleOrDefault();

            try
            {
                if (ModelState.IsValid)
                {
                    if (currentInterfaceVlan == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save changes. The vlan was deleted by another user.");
                        await PopulateInterfaceItem(ifaceVlan.InterfaceID);
                        return View(ifaceVlan);
                    }

                    await InterfaceVlanService.UpdateAsync(Mapper.Map<InterfaceVlan>(ifaceVlan));
                    return RedirectToAction("GetAllByInterfaceID", new { id = ifaceVlan.InterfaceID });
                }
            }

            catch (DbUpdateConcurrencyException ex)
            {
                var exceptionEntry = ex.Entries.Single();

                var proposedIpAddress = (string)exceptionEntry.Property("IpAddress").CurrentValue;
                if (currentInterfaceVlan.IpAddress != proposedIpAddress)
                {
                    ModelState.AddModelError("IpAddress", $"Current value: {currentInterfaceVlan.IpAddress}");
                }

                var proposedSubnetMask = (string)exceptionEntry.Property("SubnetMask").CurrentValue;
                if (currentInterfaceVlan.SubnetMask != proposedSubnetMask)
                {
                    ModelState.AddModelError("SubnetMask", $"Current value: {currentInterfaceVlan.SubnetMask}");
                }

                var proposedVlanTag = (int)exceptionEntry.Property("VlanTag").CurrentValue;
                if (currentInterfaceVlan.VlanTag != proposedVlanTag)
                {
                    ModelState.AddModelError("VlanTag", $"Current value: {currentInterfaceVlan.VlanTag}");
                }

                var proposedVrfID = (int)exceptionEntry.Property("VrfID").CurrentValue;
                if (currentInterfaceVlan.VrfID != proposedVrfID)
                {
                    ModelState.AddModelError("VrfID", $"Current value: {currentInterfaceVlan.Vrf.Name}");
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

            await PopulateInterfaceItem(currentInterfaceVlan.InterfaceID);
            await PopulateVrfsDropDownList(currentInterfaceVlan.InterfaceID);
            return View(Mapper.Map<InterfaceVlanViewModel>(currentInterfaceVlan));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ifaceVlan = await InterfaceVlanService.GetByIDAsync(id.Value);
            if (ifaceVlan == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("GetAllByInterfaceID", new { id = ifaceVlan.InterfaceID });
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

            await PopulateInterfaceItem(ifaceVlan.InterfaceID);
            return View(Mapper.Map<InterfaceVlanViewModel>(ifaceVlan));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(InterfaceVlanViewModel ifaceVlan)
        {
            try
            {
                var dbResult = await InterfaceVlanService.UnitOfWork.InterfaceVlanRepository.GetAsync(filter: d => d.InterfaceVlanID == ifaceVlan.InterfaceVlanID, 
                    AsTrackable: false);
                var currentInterfaceVlan = dbResult.SingleOrDefault();

                if (currentInterfaceVlan != null)
                {
                    await InterfaceVlanService.DeleteAsync(Mapper.Map<InterfaceVlan>(ifaceVlan));
                }
                return RedirectToAction("GetAllByInterfaceID", new { id = ifaceVlan.InterfaceID });
            }

            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { concurrencyError = true, id = ifaceVlan.InterfaceVlanID });
            }
        }
        private async Task PopulateInterfaceItem(int interfaceID)
        {
            var dbResult = await InterfaceVlanService.UnitOfWork.InterfaceRepository.GetAsync(q => q.ID == interfaceID, includeProperties:"Port");
            var iface = dbResult.Single();
            ViewBag.Interface = iface;
        }
       
        private async Task PopulateVrfsDropDownList(int interfaceID, object selectedVrf = null)
        {
            var dbResult = await InterfaceVlanService.UnitOfWork.InterfaceRepository.GetAsync(q => q.ID == interfaceID, 
                includeProperties:"Port");
            var iface = dbResult.SingleOrDefault();

            if (iface != null)
            {
                var vrfs = await InterfaceVlanService.UnitOfWork.VrfRepository.GetAsync(q => q.DeviceID == iface.Port.DeviceID);
                ViewBag.VrfID = new SelectList(vrfs, "VrfID", "Name", selectedVrf);
            }
        }
        
        /// <summary>
        /// Validates that an interface is tagged. If the interface is not tagged then vlans
        /// cannot be created.
        /// </summary>
        /// <param name="ifaceVlan"></param>
        /// <returns></returns>
        private async Task<bool> ValidateInterfaceVlan(int ifaceID)
        {
            var iface = await InterfaceVlanService.UnitOfWork.InterfaceRepository.GetByIDAsync(ifaceID);
            return iface.IsTagged;
        }
    }
}

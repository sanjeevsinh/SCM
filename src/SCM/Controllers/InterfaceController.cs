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
    public class InterfaceController : BaseViewController
    {
        public InterfaceController(IInterfaceService interfaceService, IMapper mapper)
        {
            InterfaceService = interfaceService;
            Mapper = mapper;
        }
        private IInterfaceService InterfaceService { get; set; }
        private IMapper Mapper { get; set; }

        [HttpGet]
        public async Task<IActionResult> GetByID(int id)
        {
            var item = await InterfaceService.GetByIDAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(Mapper.Map<InterfaceViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> GetByPortID(int id)
        {
            var port = await InterfaceService.UnitOfWork.PortRepository.GetByIDAsync(id);
            if (port == null)
            {
                return NotFound();
            }

            var dbResult = await InterfaceService.UnitOfWork.InterfaceRepository.GetAsync(q => q.ID == id,
                includeProperties: "Port,Vrf,InterfaceBandwidth");
            var iface = dbResult.SingleOrDefault();

            ViewBag.Port = port;
            return View(Mapper.Map<InterfaceViewModel>(iface));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbResult = await InterfaceService.UnitOfWork.InterfaceRepository.GetAsync(q => q.ID == id.Value,
                includeProperties: "InterfaceBandwidth,Vrf");
            var item = dbResult.SingleOrDefault();

            if (item == null)
            {
                return NotFound();
            }

            await PopulatePortItem(item.ID);
            return View(Mapper.Map<InterfaceViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? id) 
        {
            if (id == null)
            {
                return NotFound();
            }

            await PopulateInterfaceBandwidthsDropDownList();
            await PopulateVrfsDropDownList(id.Value);
            await PopulatePortItem(id.Value);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,IpAddress,SubnetMask,IsTagged,IsLayer3,VrfID,InterfaceBandwidthID")] InterfaceViewModel iface)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var dbResult = await InterfaceService.UnitOfWork.PortRepository.GetAsync(q => q.ID == iface.ID, includeProperties: "BundleInterfacePort.BundleInterface");
                    var port = dbResult.Single();
                    if (port.BundleInterfacePort != null)
                    {
                        ModelState.AddModelError(string.Empty, "You cannot create an interface for this port "
                            + "because the port is a member of bundle interface "
                            + port.BundleInterfacePort.BundleInterface.ID);

                        await PopulateInterfaceBandwidthsDropDownList();
                        await PopulateVrfsDropDownList(iface.ID);
                        await PopulatePortItem(iface.ID);
                        return View(iface);
                    }


                    await InterfaceService.AddAsync(Mapper.Map<Interface>(iface));
                    return RedirectToAction("GetByPortID", new { id = iface.ID });
                }
            }
            catch (DbUpdateException)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            await PopulateInterfaceBandwidthsDropDownList();
            await PopulateVrfsDropDownList(iface.ID);
            await PopulatePortItem(iface.ID);
            return View(iface);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Interface iface = await InterfaceService.GetByIDAsync(id.Value);

            if (iface == null)
            {
                return NotFound();
            }

            await PopulateInterfaceBandwidthsDropDownList();
            await PopulatePortItem(iface.ID);
            await PopulateVrfsDropDownList(iface.ID);
            return View(Mapper.Map<InterfaceViewModel>(iface));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("ID,IpAddress,SubnetMask,IsTagged,IsLayer3,VrfID,InterfaceBandwidthID,RowVersion")] InterfaceViewModel iface)
        {
            if (id != iface.ID)
            {
                return NotFound();
            }

            var dbResult = await InterfaceService.UnitOfWork.InterfaceRepository.GetAsync(q => q.ID == id, includeProperties:"InterfaceVlans",
               AsTrackable: false);
            var currentInterface = dbResult.SingleOrDefault();

            try
            {
                if (ModelState.IsValid)
                {
                    if (currentInterface == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save changes. The interface was deleted by another user.");
                        await PopulateInterfaceBandwidthsDropDownList();
                        await PopulatePortItem(iface.ID);
                        await PopulateVrfsDropDownList(iface.ID);
                        return View(iface);
                    }
                    else
                    {
                        if (!iface.IsTagged)
                        {
                            if (currentInterface.IsTagged && currentInterface.InterfaceVlans.Count > 0)
                            {
                                ModelState.AddModelError(string.Empty, "You cannot set this interface to untagged because "
                                + "there are interface vlans configured. Delete the interface vlans first.");
                                await PopulateInterfaceBandwidthsDropDownList();
                                await PopulatePortItem(iface.ID);
                                await PopulateVrfsDropDownList(iface.ID);
                                return View(iface);
                            }
                        }
                    }

                    await InterfaceService.UpdateAsync(Mapper.Map<Interface>(iface));
                    return RedirectToAction("GetByPortID", new { id = iface.ID });
                }
            }

            catch (DbUpdateConcurrencyException ex)
            {
                var exceptionEntry = ex.Entries.Single();

                var proposedIpAddress = (string)exceptionEntry.Property("IpAddress").CurrentValue;
                if (currentInterface.IpAddress != proposedIpAddress)
                {
                    ModelState.AddModelError("IpAddress", $"Current value: {currentInterface.IpAddress}");
                }

                var proposedSubnetMask = (string)exceptionEntry.Property("SubnetMask").CurrentValue;
                if (currentInterface.SubnetMask != proposedSubnetMask)
                {
                    ModelState.AddModelError("SubnetMask", $"Current value: {currentInterface.SubnetMask}");
                }

                var proposedInterfaceBandwidthID = (int)exceptionEntry.Property("InterfaceBandwidthID").CurrentValue;
                if (currentInterface.InterfaceBandwidthID != proposedInterfaceBandwidthID)
                {
                    ModelState.AddModelError("InterfaceBandwidthID", $"Current value: {currentInterface.InterfaceBandwidth.BandwidthKbps}");
                }

                var proposedVrfID = (int?)exceptionEntry.Property("VrfID").CurrentValue;
                if (currentInterface.VrfID != proposedVrfID)
                {
                    ModelState.AddModelError("VrfID", $"Current value: {currentInterface.Vrf.Name}");
                }

                var proposedIsTagged = (bool)exceptionEntry.Property("IsTagged").CurrentValue;
                if (currentInterface.IsTagged != proposedIsTagged)
                {
                    ModelState.AddModelError("IsTagged", $"Current value: {currentInterface.IsTagged}");
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

            await PopulateInterfaceBandwidthsDropDownList();
            await PopulatePortItem(currentInterface.ID);
            await PopulateVrfsDropDownList(currentInterface.ID);
            return View(Mapper.Map<InterfaceViewModel>(currentInterface));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var iface = await InterfaceService.GetByIDAsync(id.Value);
            if (iface == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("GetByPortID", new { id = iface.ID });
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

            await PopulatePortItem(iface.ID);
            return View(Mapper.Map<InterfaceViewModel>(iface));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(InterfaceViewModel iface)
        {
            try
            {
                var dbResult = await InterfaceService.UnitOfWork.InterfaceRepository.GetAsync(filter: d => d.ID == iface.ID, AsTrackable: false);
                var currentInterface = dbResult.SingleOrDefault();

                if (currentInterface != null)
                {
                    await InterfaceService.DeleteAsync(Mapper.Map<Interface>(iface));
                }
                return RedirectToAction("GetByPortID", new { id = iface.ID });
            }

            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { concurrencyError = true, id = iface.ID });
            }
        }
        private async Task PopulatePortItem(int portID)
        {
            var dbResult = await InterfaceService.UnitOfWork.PortRepository.GetAsync(q => q.ID == portID, includeProperties:"Device");
            var port = dbResult.Single();
            ViewBag.Port = port;
        }
        private async Task PopulateInterfaceBandwidthsDropDownList(object selectedInterfaceBandwidth = null)
        {
            var interfaceBandwidths = await InterfaceService.UnitOfWork.InterfaceBandwidthRepository.GetAsync();
            ViewBag.InterfaceBandwidthID = new SelectList(interfaceBandwidths, "InterfaceBandwidthID", "BandwidthKbps", selectedInterfaceBandwidth);
        }
        private async Task PopulateVrfsDropDownList(int portID, object selectedVrf = null)
        {
            var port = await InterfaceService.UnitOfWork.PortRepository.GetByIDAsync(portID);
            if (port != null)
            {
                var vrfs = await InterfaceService.UnitOfWork.VrfRepository.GetAsync(q => q.DeviceID == port.DeviceID);
                ViewBag.VrfID = new SelectList(vrfs, "VrfID", "Name", selectedVrf);
            }
        }
    }
}

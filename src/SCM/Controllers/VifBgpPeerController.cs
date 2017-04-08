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

namespace SCM.Controllers
{
    public class VifBgpPeerController : BaseViewController
    {
        public VifBgpPeerController(IBgpPeerService bgpPeerService, IVifService vifService, IMapper mapper)
        {
            BgpPeerService = bgpPeerService;
            VifService = vifService;
            Mapper = mapper;
        }

        private IBgpPeerService BgpPeerService { get; set; }
        private IVifService VifService { get; set; }
        private IMapper Mapper { get; set; }

        [HttpGet]
        public async Task<IActionResult> GetAllByVrfID(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bgpPeers = await BgpPeerService.UnitOfWork.BgpPeerRepository.GetAsync(q => q.VrfID == id);
            ViewBag.Vif = await VifService.GetByVrfIDAsync(id.Value);

            return View(Mapper.Map<List<BgpPeerViewModel>>(bgpPeers));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await BgpPeerService.GetByIDAsync(id.Value);
            if (item == null)
            {
                return NotFound();
            }

            ViewBag.Vif = await VifService.GetByVrfIDAsync(item.VrfID);

            return View(Mapper.Map<BgpPeerViewModel>(item));
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.Vif = await VifService.GetByVrfIDAsync(id.Value);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VrfID,IpAddress,AutonomousSystem,MaximumRoutes,IsBfdEnabled")] BgpPeerViewModel bgpPeer)
        {

            var vif = await VifService.GetByVrfIDAsync(bgpPeer.VrfID);
            if (vif == null)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    await VifService.UpdateRequiresSyncAsync(vif.ID, true, false, vif.Attachment.IsMultiPort);
                    await BgpPeerService.AddAsync(Mapper.Map<BgpPeer>(bgpPeer));

                    return RedirectToAction("GetAllByVrfID", new { id = bgpPeer.VrfID });
                }
            }
            catch (DbUpdateException /** ex **/ )
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            ViewBag.Vif = vif;

            return View(Mapper.Map<BgpPeerViewModel>(bgpPeer));
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BgpPeer bgpPeer = await BgpPeerService.GetByIDAsync(id.Value);

            if (bgpPeer == null)
            {
                return NotFound();
            }

            ViewBag.Vif = await VifService.GetByVrfIDAsync(bgpPeer.VrfID);

            return View(Mapper.Map<BgpPeerViewModel>(bgpPeer));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("BgpPeerID,VrfID,IpAddress,AutonomousSystem,MaximumRoutes,IsBfdEnabled,RowVersion")] BgpPeerViewModel bgpPeer)
        {
            if (id != bgpPeer.BgpPeerID)
            {
                return NotFound();
            }

            var vif = await VifService.GetByVrfIDAsync(bgpPeer.VrfID);
            if (vif == null)
            {
                return NotFound();
            }

            var dbResult = await BgpPeerService.UnitOfWork.BgpPeerRepository.GetAsync(filter: d => d.BgpPeerID == id,
                AsTrackable: false);
            var currentBgpPeer = dbResult.SingleOrDefault();

            try
            {
                if (ModelState.IsValid)
                {
                    if (currentBgpPeer == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save changes. The item was deleted by another user.");
                    }
                    else
                    {
                        await VifService.UpdateRequiresSyncAsync(vif.ID, true, false, vif.Attachment.IsMultiPort);
                        await BgpPeerService.UpdateAsync(Mapper.Map<BgpPeer>(bgpPeer));

                        return RedirectToAction("GetAllByVrfID", new { id = bgpPeer.VrfID });
                    }
                }
            }

            catch (DbUpdateConcurrencyException ex)
            {
                var exceptionEntry = ex.Entries.Single();

                var proposedAutonomousSystem = (int)exceptionEntry.Property("AutonomousSystem").CurrentValue;
                if (currentBgpPeer.AutonomousSystem != proposedAutonomousSystem)
                {
                    ModelState.AddModelError("AutonomousSystem", $"Current value: {currentBgpPeer.AutonomousSystem}");
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

            ViewBag.Vif = vif;

            return View(Mapper.Map<BgpPeerViewModel>(bgpPeer));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bgpPeer = await BgpPeerService.GetByIDAsync(id.Value);
            if (bgpPeer == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {

                    return RedirectToAction("GetAllByVrfID", new { id = bgpPeer.VrfID });
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

            ViewBag.Vif = await VifService.GetByVrfIDAsync(bgpPeer.VrfID);

            return View(Mapper.Map<BgpPeerViewModel>(bgpPeer));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(BgpPeerViewModel bgpPeer)
        {

            var vif = await VifService.GetByVrfIDAsync(bgpPeer.VrfID);
            if (vif == null)
            {
                return NotFound();
            }

            try
            {
                var dbResult = await BgpPeerService.UnitOfWork.BgpPeerRepository.GetAsync(filter: d => d.BgpPeerID == bgpPeer.BgpPeerID, AsTrackable: false);
                var currentBgpPeer = dbResult.SingleOrDefault();

                if (currentBgpPeer != null)
                {
                    await VifService.UpdateRequiresSyncAsync(vif.ID, true, false, vif.Attachment.IsMultiPort);
                    await BgpPeerService.DeleteAsync(Mapper.Map<BgpPeer>(bgpPeer));
                }

                return RedirectToAction("GetAllByVrfID", new { id = bgpPeer.VrfID });
            }

            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { concurrencyError = true, id = bgpPeer.BgpPeerID });
            }
        }
    }
}

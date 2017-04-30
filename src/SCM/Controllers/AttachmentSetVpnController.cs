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
    public class AttachmentSetVpnController : BaseVpnController
    {
        public AttachmentSetVpnController(IVpnService vpnService,
            IRouteTargetService routeTargetService,
            IAttachmentSetService attachmentSetService,
            IAttachmentSetVrfService attachmentSetVrfService,
            IAttachmentService attachmentService, 
            IVifService vifService, 
            IMapper mapper) : 
            base (vpnService, routeTargetService,attachmentSetService, attachmentSetVrfService, attachmentService, vifService, mapper)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAllByAttachmentSetID(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vpns = await VpnService.GetAllByAttachmentSetIDAsync(id.Value);
            ViewBag.AttachmentSet = await AttachmentSetService.GetByIDAsync(id.Value);

            return View(Mapper.Map<List<VpnViewModel>>(vpns));
        }
    }
}

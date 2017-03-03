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
    public class TenantAttachmentsController : BaseViewController
    {
        public TenantAttachmentsController(ITenantAttachmentsService tenantAttachmentsService, IMapper mapper)
        {
            TenantAttachmentsService = tenantAttachmentsService;
            Mapper = mapper;
        }
        private ITenantAttachmentsService TenantAttachmentsService { get; set; }
        private IMapper Mapper { get; set; }


        [HttpGet]
        public async Task<IActionResult> GetByTenantID(int id)
        {
            var tenant = await TenantAttachmentsService.UnitOfWork.TenantRepository.GetByIDAsync(id);
            if (tenant == null)
            {
                return NotFound();
            }

            var attachments = await TenantAttachmentsService.GetByTenantAsync(tenant);
            await PopulateTenantItem(id);

            return View(Mapper.Map<TenantAttachmentsViewModel>(attachments));
        }

        private async Task PopulateTenantItem(int tenantID)
        {
            var tenant = await TenantAttachmentsService.UnitOfWork.TenantRepository.GetByIDAsync(tenantID);
            ViewBag.Tenant = tenant;
        }

    }
}
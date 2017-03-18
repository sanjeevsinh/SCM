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
using System.Net;
using System.Linq.Expressions;
using SCM.Data;

namespace SCM.Controllers
{
    public class AttachmentAndVifController : BaseViewController
    {
        public AttachmentAndVifController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
        }
        private IUnitOfWork UnitOfWork { get; set; }
        private IMapper Mapper { get; set; }

        [HttpGet]
        public async Task<IActionResult> GetAllByVpnID(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vpn = await UnitOfWork.VpnRepository.GetByIDAsync(id);
            if (vpn == null)
            {
                return NotFound();
            }

            var interfaces = await UnitOfWork.InterfaceRepository.GetAsync(q => q.Vrf.AttachmentSetVrfs.Where(a => a.AttachmentSet.VpnAttachmentSets
                .Where(v => v.VpnID == id.Value).Count() > 0).Count() > 0,
                includeProperties: "Device.Location.SubRegion.Region,Vrf.AttachmentSetVrfs.AttachmentSet,Device.Plane,Port,"
                + "InterfaceBandwidth,ContractBandwidthPool.ContractBandwidth,Tenant");

            var interfaceVlans = await UnitOfWork.InterfaceVlanRepository.GetAsync(q => q.Vrf.AttachmentSetVrfs.Where(a => a.AttachmentSet.VpnAttachmentSets
               .Where(v => v.VpnID == id.Value).Count() > 0).Count() > 0,
               includeProperties: "Interface.Device.Location.SubRegion.Region,Interface.InterfaceBandwidth,Vrf.AttachmentSetVrfs.AttachmentSet,"
               + "Interface.Device.Plane,Interface.Port,Interface.InterfaceBandwidth,Tenant,ContractBandwidthPool.ContractBandwidth");

            var result = Mapper.Map<List<AttachmentAndVifViewModel>>(interfaces).Concat(Mapper.Map<List<AttachmentAndVifViewModel>>(interfaceVlans));
            result = result.OrderBy(q => q.AttachmentSetName);

            ViewBag.Vpn = vpn;

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id, [FromQuery]bool vif)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (vif)
            {
                var dbResult = await UnitOfWork.InterfaceVlanRepository.GetAsync(q => q.InterfaceVlanID == id.Value,
                    includeProperties: "Interface.Device.Location.SubRegion.Region,Interface.InterfaceBandwidth,Vrf.AttachmentSetVrfs.AttachmentSet,"
                    + "Interface.Device.Plane,Interface.Port,Interface.InterfaceBandwidth,Tenant,ContractBandwidthPool.ContractBandwidth");

                var item = dbResult.SingleOrDefault();

                return View(Mapper.Map<AttachmentAndVifViewModel>(item));
            }
            else
            {
                var dbResult = await UnitOfWork.InterfaceRepository.GetAsync(q => q.InterfaceID == id.Value,
                    includeProperties: "Device.Location.SubRegion.Region,Vrf.AttachmentSetVrfs.AttachmentSet,Device.Plane,Port,"
                    + "InterfaceBandwidth,ContractBandwidthPool.ContractBandwidth,Tenant");

                var item = dbResult.SingleOrDefault();

                return View(Mapper.Map<AttachmentAndVifViewModel>(item));
            }
        }
    }
}
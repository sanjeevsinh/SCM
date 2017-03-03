using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using SCM.Models.NetModels.Attachment;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace SCM.Services.SCMServices
{
    public class TenantAttachmentsService : BaseService, ITenantAttachmentsService
    {
        public TenantAttachmentsService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public async Task<TenantAttachments> GetByTenantAsync(Tenant tenant)
        {
            var tenantAttachments = new TenantAttachments();
            var ifaces = await UnitOfWork.InterfaceRepository.GetAsync(q => q.Port.TenantID == tenant.TenantID,
                includeProperties: "Port.Device.Location.SubRegion.Region,Port.Device.Plane,"
                + "Port.Interface.Vrf,Port.Interface.InterfaceBandwidth", AsTrackable: false);

            tenantAttachments.AttachmentInterfaces = Mapper.Map<List<AttachmentInterface>>(ifaces);

            var bundleIfaces = await UnitOfWork.BundleInterfaceRepository.GetAsync(q => q.BundleInterfaceID == tenant.TenantID,
                includeProperties: "Vrf", AsTrackable: false);

            tenantAttachments.AttachmentBundleInterfaces = Mapper.Map<List<AttachmentBundleInterface>>(bundleIfaces);

            return tenantAttachments;
        }
    }
}
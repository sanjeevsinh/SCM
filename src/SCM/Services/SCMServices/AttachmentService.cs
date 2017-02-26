using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using System.Threading.Tasks;

namespace SCM.Services.SCMServices
{
    public class AttachmentService : BaseService, IAttachmentService
    {
        public AttachmentService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<TenantAttachments> GetByTenantAsync(Tenant tenant)
        {
            var tenantAttachments = new TenantAttachments();
            var ifaces = await UnitOfWork.InterfaceRepository.GetAsync(q => q.Port.TenantID == tenant.TenantID, 
                includeProperties: "Vrf", AsTrackable: false);

            tenantAttachments.AttachmentInterfaces = Mapper.Map<List<AttachmentInterface>>(ifaces);

            var bundleIfaces = await UnitOfWork.BundleInterfaceRepository.GetAsync(q => q.BundleInterfaceID == tenant.TenantID,
                includeProperties: "Vrf", AsTrackable: false);

            tenantAttachments.AttachmentBundleInterfaces = Mapper.Map<List<AttachmentBundleInterface>>(bundleIfaces);

            return tenantAttachments;
        }

        public async Task<AttachmentRequest> GetAttachmentInterfaceByIDAsync(int id)
        {
            var iface = await UnitOfWork.InterfaceRepository.GetAsync(q => q.ID == id, includeProperties: "Vrf", AsTrackable: false);
            return Mapper.Map<AttachmentRequest>(iface);
        }

        public async Task<AttachmentRequest> GetAttachmentBundleInterfaceByIDAsync(int id)
        {
            var bundleIface = await UnitOfWork.BundleInterfaceRepository.GetAsync(q => q.BundleInterfaceID == id, includeProperties: "Vrf", AsTrackable: false);
            return Mapper.Map<AttachmentRequest>(bundleIface);
        }

        public async Task<int> AddAsync(AttachmentRequest attachmentRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<int> DeleteAttachmentInterfaceAsync(AttachmentInterface attachment)
        {
            throw new NotImplementedException();
        }
        public async Task<int> DeleteAttachmentBundleInterfaceAsync(AttachmentBundleInterface attachment)
        {
            throw new NotImplementedException();
        }
    }
}
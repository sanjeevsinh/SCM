using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using SCM.Models.ServiceModels;
using System.Threading.Tasks;
using AutoMapper;
using System.Linq.Expressions;

namespace SCM.Services.SCMServices
{
    public class AttachmentOrVifService : BaseService, IAttachmentOrVifService
    {
        public AttachmentOrVifService(IUnitOfWork unitOfWork, IMapper mapper,
            IAttachmentService attachmentService, IVifService vifService) : base(unitOfWork, mapper)
        {
            AttachmentService = attachmentService;
            VifService = vifService;
        }

        private IAttachmentService AttachmentService { get; set; }
        private IVifService VifService { get; set; }

        public async Task<IEnumerable<AttachmentOrVif>> GetAllByVpnIDAsync(int vpnID)
        {
            Expression<Func<Interface, bool>> ifaceEx = q => !q.IsMultiPort && q.Vrf.AttachmentSetVrfs.Where(a => a.AttachmentSet.VpnAttachmentSets
                .Where(v => v.VpnID == vpnID).Count() > 0).Count() > 0;

            var attachments = await AttachmentService.GetAsync(ifaceEx);

            Expression<Func<MultiPort, bool>> multiPortEx = q => q.Vrf.AttachmentSetVrfs.Where(a => a.AttachmentSet.VpnAttachmentSets
               .Where(v => v.VpnID == vpnID).Count() > 0).Count() > 0;

            var multiPortAttachments = await AttachmentService.GetAsync(multiPortEx);

            Expression<Func<InterfaceVlan, bool>> ifaceVlanEx = q => !q.Interface.IsMultiPort 
                && q.Vrf.AttachmentSetVrfs.Where(a => a.AttachmentSet.VpnAttachmentSets
                .Where(v => v.VpnID == vpnID).Count() > 0).Count() > 0;

            var vifs = await VifService.GetAsync(ifaceVlanEx);

            Expression<Func<MultiPortVlan, bool>> multiPortVlanEx = q => q.Vrf.AttachmentSetVrfs.Where(a => a.AttachmentSet.VpnAttachmentSets
                .Where(v => v.VpnID == vpnID).Count() > 0).Count() > 0;

            var multiPortVifs = await VifService.GetAsync(multiPortVlanEx);

            var result = Mapper.Map<List<AttachmentOrVif>>(attachments.Concat(multiPortAttachments))
                .Concat(Mapper.Map<List<AttachmentOrVif>>(vifs.Concat(multiPortVifs)));

            result = result.OrderBy(q => q.AttachmentSetName);

            return result;
        }

        public async Task<AttachmentOrVif> GetByIDAsync(int id, bool? vif = false, bool? attachmentIsMultiPort = false)
        {
            Object item;
            if (vif.GetValueOrDefault())
            {
                item = await VifService.GetByIDAsync(id, attachmentIsMultiPort);
            }
            else
            {
                item = await AttachmentService.GetByIDAsync(id, attachmentIsMultiPort);
            }

            return Mapper.Map<AttachmentOrVif>(item);
        }
    }
}
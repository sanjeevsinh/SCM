using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using SCM.Models.ServiceModels;
using System.Threading.Tasks;
using AutoMapper;

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
            var interfaces = await UnitOfWork.InterfaceRepository.GetAsync(q => q.Vrf.AttachmentSetVrfs.Where(a => a.AttachmentSet.VpnAttachmentSets
                .Where(v => v.VpnID == vpnID).Count() > 0).Count() > 0,
                includeProperties: "Device.Location.SubRegion.Region,Vrf.AttachmentSetVrfs.AttachmentSet,Device.Plane,Port,"
                + "InterfaceBandwidth,ContractBandwidthPool.ContractBandwidth,Tenant");

            var interfaceVlans = await UnitOfWork.InterfaceVlanRepository.GetAsync(q => q.Vrf.AttachmentSetVrfs.Where(a => a.AttachmentSet.VpnAttachmentSets
               .Where(v => v.VpnID == vpnID).Count() > 0).Count() > 0,
               includeProperties: "Interface.Device.Location.SubRegion.Region,Interface.InterfaceBandwidth,Vrf.AttachmentSetVrfs.AttachmentSet,"
               + "Interface.Device.Plane,Interface.Port,Interface.InterfaceBandwidth,Tenant,ContractBandwidthPool.ContractBandwidth");

            var result = Mapper.Map<List<AttachmentOrVif>>(interfaces).Concat(Mapper.Map<List<AttachmentOrVif>>(interfaceVlans));
            result = result.OrderBy(q => q.AttachmentSetName);

            return result;
        }

        public async Task<AttachmentOrVif> GetByIDAsync(int id, bool vif)
        {
            Object item;
            if (vif)
            {
                item = await GetInterfaceVlan(id);
            }
            else
            {
                item = await GetInterface(id);
            }

            return Mapper.Map<AttachmentOrVif>(item);
        }

        private async Task<Interface> GetInterface(int id)
        {

            var dbResult = await UnitOfWork.InterfaceRepository.GetAsync(q => q.InterfaceID == id,
                   includeProperties: "Device.Location.SubRegion.Region,Vrf.AttachmentSetVrfs.AttachmentSet,Device.Plane,Port,"
                   + "InterfaceBandwidth,ContractBandwidthPool.ContractBandwidth,Tenant");

            return dbResult.SingleOrDefault();
        }

        private async Task<InterfaceVlan> GetInterfaceVlan(int id)
        {

            var dbResult = await UnitOfWork.InterfaceVlanRepository.GetAsync(q => q.InterfaceVlanID == id,
                includeProperties: "Interface.Device.Location.SubRegion.Region,Interface.InterfaceBandwidth,Vrf.AttachmentSetVrfs.AttachmentSet,"
                + "Interface.Device.Plane,Interface.Port,Interface.InterfaceBandwidth,Tenant,ContractBandwidthPool.ContractBandwidth");

            return dbResult.SingleOrDefault();
        }
    }
}
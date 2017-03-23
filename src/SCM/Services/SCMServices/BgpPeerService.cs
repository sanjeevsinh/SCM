using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Data;

namespace SCM.Services.SCMServices
{
    public class BgpPeerService : BaseService, IBgpPeerService
    {
        public BgpPeerService(IUnitOfWork unitOfWork, IAttachmentService attachmentService, IVifService vifService) : base(unitOfWork)
        {
            AttachmentService = attachmentService;
            VifService = vifService;
        }

        private IAttachmentService AttachmentService { get; set; }
        private IVifService VifService { get; set; }

        public async Task<IEnumerable<BgpPeer>> GetAllAsync()
        {
            return await this.UnitOfWork.BgpPeerRepository.GetAsync();
        }

        public async Task<BgpPeer> GetByIDAsync(int id)
        {
            return await this.UnitOfWork.BgpPeerRepository.GetByIDAsync(id);
        }

        public async Task<int> AddAsync(BgpPeer bgpPeer)
        {
            this.UnitOfWork.BgpPeerRepository.Insert(bgpPeer);
            await UpdateInterfaceAndInterfaceVlanRequireSyncAsync(bgpPeer.VrfID);

            return await this.UnitOfWork.SaveAsync();
        }
 
        public async Task<int> UpdateAsync(BgpPeer bgpPeer)
        {
            this.UnitOfWork.BgpPeerRepository.Update(bgpPeer);
            await UpdateInterfaceAndInterfaceVlanRequireSyncAsync(bgpPeer.VrfID);

            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(BgpPeer bgpPeer)
        {
            this.UnitOfWork.BgpPeerRepository.Delete(bgpPeer);
            await UpdateInterfaceAndInterfaceVlanRequireSyncAsync(bgpPeer.VrfID);

            return await this.UnitOfWork.SaveAsync();
        }


        /// <summary>
        /// Helper to update the requireSync property of all interfaces and interface vlans which 
        /// are associated with a vrf.
        /// </summary>
        /// <param name="vrfID"></param>
        /// <returns></returns>
        private async Task UpdateInterfaceAndInterfaceVlanRequireSyncAsync(int vrfID)
        {
            var vrfQueryResult = await UnitOfWork.VrfRepository.GetAsync(q => q.VrfID == vrfID, includeProperties: "Interfaces,InterfaceVlans");
            var vrf = vrfQueryResult.Single();

            foreach (Interface iface in vrf.Interfaces)
            {
                await AttachmentService.UpdateRequiresSyncAsync(iface.InterfaceID, true);
            }

            foreach (InterfaceVlan ifaceVlan in vrf.InterfaceVlans)
            {
                await VifService.UpdateRequiresSyncAsync(ifaceVlan.InterfaceVlanID, true);
            }
        }
    }
}

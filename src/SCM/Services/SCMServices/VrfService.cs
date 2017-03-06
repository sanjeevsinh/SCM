using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using System.Threading.Tasks;

namespace SCM.Services.SCMServices
{
    public class VrfService : BaseService, IVrfService
    {
        public VrfService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<Vrf> GetByIDAsync(int key)
        {
            return await UnitOfWork.VrfRepository.GetByIDAsync(key);
        }

        public async Task<int> AddAsync(Vrf vrf)
        {
            this.UnitOfWork.VrfRepository.Insert(vrf);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> UpdateAsync(Vrf vrf)
        {
            this.UnitOfWork.VrfRepository.Update(vrf);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(Vrf vrf)
        {
            this.UnitOfWork.VrfRepository.Delete(vrf);
            return await this.UnitOfWork.SaveAsync();
        }
        /// <summary>
        /// Validate if a VRF can be deleted. A VRF cannot be deleted if one or more
        /// VPNs are bound to the VRF.
        /// </summary>
        /// <param name="vrfID"></param>
        /// <returns></returns>
        public async Task<ServiceResult> ValidateDelete(int vrfID)
        {
            var result = new ServiceResult();
            result.IsSuccess = true;

            var vpnAttachmentSets = await UnitOfWork.VpnAttachmentSetRepository.GetAsync(q => q.AttachmentSet.AttachmentSetVrfs
                    .Where(v => v.VrfID == vrfID).Count() > 0,
                    includeProperties: "Vpn,AttachmentSet");

            if (vpnAttachmentSets.Count() > 0)
            {
                result.Add("The VRF cannot be deleted. VPN services are bound to the VRF.");
                result.Add("Perform the following then try again: ");
                result.AddRange(vpnAttachmentSets.ToList().Select(q => $"Remove attachment set {q.AttachmentSet.Name} from {q.Vpn.Name}."));

                result.IsSuccess = false;
            }

            return result;
        }
    }
}
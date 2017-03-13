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

        public async Task<ServiceResult> AddAsync(Vrf vrf)
        {
            var result = new ServiceResult { IsSuccess = true };

            var dbResult = await UnitOfWork.RouteDistinguisherRangeRepository.GetAsync(q => q.Name == "Default");
            var rdRange = dbResult.SingleOrDefault();

            if (rdRange == null)
            {
                result.Add("The default route distinguisher range was not found.");
                result.IsSuccess = false;

                return result;
            }

            var usedRDs = await UnitOfWork.VrfRepository.GetAsync(q => q.RouteDistinguisherRangeID == rdRange.RouteDistinguisherRangeID);

            // Allocate a new unused RD from the RD range

            int? newRdAssignedNumberSubField = Enumerable.Range(rdRange.AssignedNumberSubFieldStart, rdRange.AssignedNumberSubFieldCount)
                .Except(usedRDs.Select(q => q.AssignedNumberSubField)).FirstOrDefault();

            if (newRdAssignedNumberSubField == null)
            {
                result.Add("Failed to allocate a free route distinguisher. Check the range requested or try another range.");
                result.IsSuccess = false;

                return result;
            }

            vrf.AdministratorSubField = rdRange.AdministratorSubField;
            vrf.AssignedNumberSubField = newRdAssignedNumberSubField.Value;
            var tenant = await UnitOfWork.TenantRepository.GetByIDAsync(vrf.TenantID);
            vrf.Name = $"{tenant.Name}_{vrf.RouteDistinguisherRange}_{vrf.AssignedNumberSubField}";

            this.UnitOfWork.VrfRepository.Insert(vrf);

            await this.UnitOfWork.SaveAsync();

            return result;
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
        public async Task<ServiceResult> ValidateDeleteAsync(int vrfID)
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
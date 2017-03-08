using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using System.Threading.Tasks;

namespace SCM.Services.SCMServices
{
    public class VpnTenantCommunityService : BaseService, IVpnTenantCommunityService
    {
        public VpnTenantCommunityService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<VpnTenantCommunity>> GetAllAsync()
        {
            return await this.UnitOfWork.VpnTenantCommunityRepository.GetAsync(includeProperties: "TenantCommunity,VpnAttachmentSet");
        }

        public async Task<VpnTenantCommunity> GetByIDAsync(int key)
        {
            return await UnitOfWork.VpnTenantCommunityRepository.GetByIDAsync(key);
        }

        public async Task<int> AddAsync(VpnTenantCommunity vpnTenantCommunity)
        {
            this.UnitOfWork.VpnTenantCommunityRepository.Insert(vpnTenantCommunity);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> UpdateAsync(VpnTenantCommunity vpnTenantCommunity)
        {
            this.UnitOfWork.VpnTenantCommunityRepository.Update(vpnTenantCommunity);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(VpnTenantCommunity vpnTenantCommunity)
        {
            this.UnitOfWork.VpnTenantCommunityRepository.Delete(vpnTenantCommunity);
            return await this.UnitOfWork.SaveAsync();
        }

        /// <summary>
        /// Validates a Tenant Community for binding to a VPN Attachment Set.
        /// </summary>
        /// <param name="vpnTenantCommunity"></param>
        /// <returns></returns>
        public async Task<ServiceResult> ValidateAsync (VpnTenantCommunity vpnTenantCommunity)
        {

            var validationResult = new ServiceResult { IsSuccess = true };
                  
            var dbResult = await UnitOfWork.VpnAttachmentSetRepository.GetAsync(q => q.VpnAttachmentSetID == vpnTenantCommunity.VpnAttachmentSetID,
                includeProperties:"Vpn", AsTrackable:false);
            var vpnAttachmentSet = dbResult.SingleOrDefault();

            if (vpnAttachmentSet == null)
            {
                validationResult.Add("The Attachment Set was not found.");
                validationResult.IsSuccess = false;
            }

            var vpn = vpnAttachmentSet.Vpn;

            if (vpn.IsExtranet)
            {
                var tenantCommunity = await UnitOfWork.TenantCommunityRepository.GetByIDAsync(vpnTenantCommunity.TenantCommunityID);
                if (!tenantCommunity.AllowExtranet)
                {
                    validationResult.Add("Tenant Community " + tenantCommunity.AutonomousSystemNumber + ":" + tenantCommunity.Number + " is not enabled for Extranet.");
                    validationResult.IsSuccess = false;
                }
            }
            else
            {

                var existingVpnTenantCommunityResult = await UnitOfWork.VpnTenantCommunityRepository.GetAsync(q => q.TenantCommunityID == vpnTenantCommunity.TenantCommunityID,
                    includeProperties: "TenantCommunity,VpnAttachmentSet.Vpn", AsTrackable: false);
                var existingVpnTenantCommunity = existingVpnTenantCommunityResult.SingleOrDefault();

                if (existingVpnTenantCommunity != null)
                {

                    validationResult.Add("Tenant Community " + existingVpnTenantCommunity.TenantCommunity.AutonomousSystemNumber
                        + ":" + existingVpnTenantCommunity.TenantCommunity.Number
                        + " is already bound to VPN " + existingVpnTenantCommunity.VpnAttachmentSet.Vpn.Name + ".");
                    validationResult.IsSuccess = false;
                }
            }

            return validationResult;
        }
    }
}
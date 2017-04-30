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
        public VpnTenantCommunityService(IUnitOfWork unitOfWork, IVpnService vpnService) : base(unitOfWork)
        {
            VpnService = vpnService;
        }
 
        private IVpnService VpnService { get; set; }

        public async Task<IEnumerable<VpnTenantCommunity>> GetAllAsync()
        {
            return await this.UnitOfWork.VpnTenantCommunityRepository.GetAsync(includeProperties: "VpnAttachmentSet.Vpn,"
                + "VpnAttachmentSet.AttachmentSet.Tenant,"
                + "TenantCommunity");
        }

        public async Task<VpnTenantCommunity> GetByIDAsync(int id)
        {
            var dbResult = await UnitOfWork.VpnTenantCommunityRepository.GetAsync(q => q.VpnTenantCommunityID == id,
                includeProperties: "VpnAttachmentSet.Vpn," 
                + "VpnAttachmentSet.AttachmentSet.Tenant,"
                + "TenantCommunity");

            return dbResult.SingleOrDefault();
        }

        public async Task<int> AddAsync(VpnTenantCommunity vpnTenantCommunity)
        {
            UnitOfWork.VpnTenantCommunityRepository.Insert(vpnTenantCommunity);
            var vpnAttachmentSet = await UnitOfWork.VpnAttachmentSetRepository.GetByIDAsync(vpnTenantCommunity.VpnAttachmentSetID);
            await VpnService.UpdateVpnRequiresSyncAsync(vpnAttachmentSet.VpnID, true, false);

            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> UpdateAsync(VpnTenantCommunity vpnTenantCommunity)
        {
            this.UnitOfWork.VpnTenantCommunityRepository.Update(vpnTenantCommunity);
            var vpnAttachmentSet = await UnitOfWork.VpnAttachmentSetRepository.GetByIDAsync(vpnTenantCommunity.VpnAttachmentSetID);
            await VpnService.UpdateVpnRequiresSyncAsync(vpnAttachmentSet.VpnID, true, false);

            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(VpnTenantCommunity vpnTenantCommunity)
        {
            this.UnitOfWork.VpnTenantCommunityRepository.Delete(vpnTenantCommunity);
            var vpnAttachmentSet = await UnitOfWork.VpnAttachmentSetRepository.GetByIDAsync(vpnTenantCommunity.VpnAttachmentSetID);
            await VpnService.UpdateVpnRequiresSyncAsync(vpnAttachmentSet.VpnID, true, false);

            return await this.UnitOfWork.SaveAsync();
        }

        /// <summary>
        /// Validates a Tenant Community for binding to a VPN Attachment Set.
        /// </summary>
        /// <param name="vpnTenantCommunity"></param>
        /// <returns></returns>
        public async Task<ServiceResult> ValidateNewAsync (VpnTenantCommunity vpnTenantCommunity, VpnAttachmentSet vpnAttachmentSet)
        {
            var validationResult = new ServiceResult { IsSuccess = true };
          
            var vpn = vpnAttachmentSet.Vpn;

            if (vpn.IsExtranet)
            {
                var tenantCommunity = await UnitOfWork.TenantCommunityRepository.GetByIDAsync(vpnTenantCommunity.TenantCommunityID);
                if (!tenantCommunity.AllowExtranet)
                {
                    validationResult.Add($"Tenant Community {tenantCommunity.AutonomousSystemNumber} : "
                        + $"{tenantCommunity.Number} is not enabled for Extranet.");

                    validationResult.IsSuccess = false;
                }
            }
            else
            {
                var existingVpnTenantCommunityResult = await UnitOfWork.VpnTenantCommunityRepository.GetAsync(q => 
                    q.TenantCommunityID == vpnTenantCommunity.TenantCommunityID,
                    includeProperties: "TenantCommunity,VpnAttachmentSet.Vpn", AsTrackable: false);

                var existingVpnTenantCommunity = existingVpnTenantCommunityResult.SingleOrDefault();

                if (existingVpnTenantCommunity != null)
                {
                    validationResult.Add($"Tenant Community {existingVpnTenantCommunity.TenantCommunity.AutonomousSystemNumber} : "
                        + $"{existingVpnTenantCommunity.TenantCommunity.Number} is already bound " 
                        + $"to VPN {existingVpnTenantCommunity.VpnAttachmentSet.Vpn.Name}.");

                    validationResult.IsSuccess = false;
                }
            }

            return validationResult;
        }
    }
}
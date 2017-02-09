using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using System.Threading.Tasks;

namespace SCM.Services.SCMServices
{
    public class VpnService : BaseService, IVpnService
    {
        public VpnService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<Vpn>> GetAllAsync()
        {
            return await this.UnitOfWork.VpnRepository.GetAsync(includeProperties: "Plane,VpnTenancyType,VpnTopologyType.VpnProtocolType,Tenant,Region");
        }

        public async Task<Vpn> GetByIDAsync(int key)
        {
            return await UnitOfWork.VpnRepository.GetByIDAsync(key);
        }

        public async Task<int> AddAsync(Vpn vpn)
        {
            this.UnitOfWork.VpnRepository.Insert(vpn);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> UpdateAsync(Vpn vpn)
        {
            this.UnitOfWork.VpnRepository.Update(vpn);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(Vpn vpn)
        {
            this.UnitOfWork.VpnRepository.Delete(vpn);
            return await this.UnitOfWork.SaveAsync();
        }

        /// <summary>
        /// Validate a new VPN
        /// </summary>
        /// <param name="vpn"></param>
        /// <returns></returns>
        public async Task<ServiceValidationResult> ValidateVpnAsync(Vpn vpn)
        {
            var validationResult = new ServiceValidationResult();
            validationResult.IsValid = true;

            if (vpn.IsExtranet)
            {
                var vpnTopologyType = await UnitOfWork.VpnTopologyTypeRepository.GetByIDAsync(vpn.VpnTopologyTypeID);

                if (vpnTopologyType.TopologyType != "Hub-and-Spoke")
                {
                    validationResult.Add("Extranet is supported only for Hub-and-Spoke IP VPN topologies.");
                    validationResult.IsValid = false;
                }
            }

            return validationResult;
        }

        public async Task<ServiceValidationResult> ValidateVpnChangesAsync(Vpn vpn)
        {
            var validationResult = new ServiceValidationResult();
            validationResult.IsValid = true;

            var dbResult = await UnitOfWork.VpnRepository.GetAsync(q => q.VpnID == vpn.VpnID, AsTrackable: false);
            var currentVpn = dbResult.SingleOrDefault();

            var attachmentSets = await UnitOfWork.VpnAttachmentSetRepository.GetAsync(q => q.VpnID == vpn.VpnID);
            if (currentVpn == null)
            {
                validationResult.Add("The VPN was not found.");
                validationResult.IsValid = false;

                return validationResult;
            }

            if (attachmentSets.Count() > 0)
            {
                if (vpn.PlaneID != currentVpn.PlaneID)
                {
                    validationResult.Add("The Plane cannot be changed because Attachment Sets are associated with this VPN.");
                    validationResult.IsValid = false;
                }
                if (vpn.RegionID != currentVpn.RegionID)
                {
                    validationResult.Add("The Region cannot be changed because Attachment Sets are associated with this VPN.");
                    validationResult.IsValid = false;
                }
                if (vpn.TenantID != currentVpn.TenantID)
                {
                    validationResult.Add("The Tenant Owner cannot be changed because Attachment Sets are associated with this VPN.");
                    validationResult.IsValid = false;
                }
                if (vpn.VpnTopologyTypeID != currentVpn.VpnTopologyTypeID)
                {
                    validationResult.Add("The Topology Type cannot be changed because Attachment Sets are associated with this VPN.");
                    validationResult.IsValid = false;
                }
                if (vpn.VpnTenancyTypeID != currentVpn.VpnTenancyTypeID)
                {
                    validationResult.Add("The Tenancy Type cannot be changed because Attachment Sets are associated with this VPN.");
                    validationResult.IsValid = false;
                }

                return validationResult;
            }

            return validationResult;
        }
    }
}
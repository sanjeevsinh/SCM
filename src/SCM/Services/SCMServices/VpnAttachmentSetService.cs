using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using System.Threading.Tasks;

namespace SCM.Services.SCMServices
{
    public class VpnAttachmentSetService : BaseService, IVpnAttachmentSetService
    {
        private IAttachmentSetVrfService AttachmentSetVrfService { get; set; }
        private IRouteTargetService RouteTargetService { get; set; }
        private IVpnService VpnService { get; set; }

        public VpnAttachmentSetService(IUnitOfWork unitOfWork,
            IAttachmentSetVrfService attachmentSetVrfService, 
            IRouteTargetService routeTargetService,
            IVpnService vpnService) : base(unitOfWork)
        {
            AttachmentSetVrfService = attachmentSetVrfService;
            RouteTargetService = routeTargetService;
            VpnService = vpnService;
        }

        public async Task<IEnumerable<VpnAttachmentSet>> GetAllAsync()
        {
            return await this.UnitOfWork.VpnAttachmentSetRepository.GetAsync();
        }

        public async Task<VpnAttachmentSet> GetByIDAsync(int id)
        {
            var dbResult = await UnitOfWork.VpnAttachmentSetRepository.GetAsync(q => q.VpnAttachmentSetID == id, 
                includeProperties: "AttachmentSet.Tenant,Vpn");

            return dbResult.SingleOrDefault();
        }

        public async Task<int> AddAsync(VpnAttachmentSet attachmentSetVpn)
        {
            this.UnitOfWork.VpnAttachmentSetRepository.Insert(attachmentSetVpn);
            await VpnService.UpdateVpnRequiresSyncAsync(attachmentSetVpn.VpnID, true, false);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> UpdateAsync(VpnAttachmentSet attachmentSetVpn)
        {
            this.UnitOfWork.VpnAttachmentSetRepository.Update(attachmentSetVpn);
            await VpnService.UpdateVpnRequiresSyncAsync(attachmentSetVpn.VpnID, true, false);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(VpnAttachmentSet attachmentSetVpn)
        {
            this.UnitOfWork.VpnAttachmentSetRepository.Delete(attachmentSetVpn);
            await VpnService.UpdateVpnRequiresSyncAsync(attachmentSetVpn.VpnID, true, false);
            return await this.UnitOfWork.SaveAsync();
        }

        /// <summary>
        /// Validate a new VPN Attachment Set request.
        /// </summary>
        /// <param name="vpnAttachmentSet"></param>
        /// <returns></returns>
        public ServiceResult ValidateNew(VpnAttachmentSet vpnAttachmentSet, Vpn vpn, AttachmentSet attachmentSet)
        {
            var validationResult = new ServiceResult { IsSuccess = true };

            // Validate the attachment set with the vpn binding

            var attachmentRedundancyName = attachmentSet.AttachmentRedundancy.Name;

            if (vpn.Plane != null)
            {
                if (attachmentRedundancyName == "Silver" || attachmentRedundancyName == "Gold")
                {
                    validationResult.Add($"A '{attachmentRedundancyName}' attachment set cannot be used with a planar-scoped VPN. "
                        + $"The VPN is scoped to the '{vpn.Plane.Name}' plane. "
                        + "The attachment set provides connectivity into both planes.");
                    validationResult.IsSuccess = false;
                }

                else if (attachmentRedundancyName == "Bronze")
                {
                    var attachmentPlane = attachmentSet.AttachmentSetVrfs.Single().Vrf.Device.Plane.Name;
                    if (attachmentPlane != vpn.Plane.Name)
                    {
                        validationResult.Add($"Bronze attachment '{attachmentSet.Name}' cannot be used with VPN '{vpn.Name}' because "
                        + $"the VPN is scoped to the '{vpn.Plane.Name}' plane. "
                        + $"The attachment set provides connectivity into the '{attachmentPlane}' plane.");
                        validationResult.IsSuccess = false;
                    }
                }

                else if (attachmentRedundancyName == "Custom")
                {
                    if (attachmentSet.AttachmentSetVrfs.Where(v => v.Vrf.Device.Plane.Name == vpn.Plane.Name).Count() != attachmentSet.AttachmentSetVrfs.Count())
                    {
                        validationResult.Add($"The VPN is scoped to the '{vpn.Plane.Name}' plane. "
                            + "One or more VRFs in the attachment set are not located in this plane.");
                        validationResult.IsSuccess = false;
                    }
                }
            }

            return validationResult;
        }
    }
}
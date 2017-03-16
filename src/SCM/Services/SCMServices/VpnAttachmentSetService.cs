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

        public VpnAttachmentSetService(IUnitOfWork unitOfWork,
            IAttachmentSetVrfService attachmentSetVrfService, 
            IRouteTargetService routeTargetService) : base(unitOfWork)
        {
            AttachmentSetVrfService = attachmentSetVrfService;
            RouteTargetService = routeTargetService;
        }

        public async Task<IEnumerable<VpnAttachmentSet>> GetAllAsync()
        {
            return await this.UnitOfWork.VpnAttachmentSetRepository.GetAsync();
        }

        public async Task<VpnAttachmentSet> GetByIDAsync(int key)
        {
            return await UnitOfWork.VpnAttachmentSetRepository.GetByIDAsync(key);
        }

        public async Task<int> AddAsync(VpnAttachmentSet attachmentSetVpn)
        {
            this.UnitOfWork.VpnAttachmentSetRepository.Insert(attachmentSetVpn);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> UpdateAsync(VpnAttachmentSet attachmentSetVpn)
        {
            this.UnitOfWork.VpnAttachmentSetRepository.Update(attachmentSetVpn);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(VpnAttachmentSet attachmentSetVpn)
        {
            this.UnitOfWork.VpnAttachmentSetRepository.Delete(attachmentSetVpn);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<ServiceResult> ValidateAsync(VpnAttachmentSet vpnAttachmentSet)
        {
            var validationResult = new ServiceResult { IsSuccess = true };

            var vpnDbResult = await UnitOfWork.VpnRepository.GetAsync(q => q.VpnID == vpnAttachmentSet.VpnID, 
                includeProperties:"Plane", AsTrackable:false);
            var vpn = vpnDbResult.SingleOrDefault();

            var attachmentSetResult = await UnitOfWork.AttachmentSetRepository.GetAsync(q => q.AttachmentSetID == vpnAttachmentSet.AttachmentSetID, 
                includeProperties: "AttachmentSetVrfs.Vrf.Device.Plane,AttachmentRedundancy");
            var attachmentSet = attachmentSetResult.SingleOrDefault();

            if (vpn == null)
            {
                validationResult.Add("The VPN was not found.");
                validationResult.IsSuccess = false;
                return validationResult;
            }

            if (attachmentSet == null)
            {
                validationResult.Add("The attachment set was not found");
                validationResult.IsSuccess = false;
                return validationResult;
            }

            // First check that the vrfs in the attachment set are correctly configured

            var attachmentSetVrfValidationResult = await AttachmentSetVrfService.CheckVrfsConfiguredCorrectlyAsync(attachmentSet);
            if (!attachmentSetVrfValidationResult.IsSuccess)
            {
                validationResult.Add($"The VRFs in attachment set '{attachmentSet.Name}' are not configured correctly. "
                    + "Resolve this issue and try again. ");
                validationResult.Add(attachmentSetVrfValidationResult.GetMessage());
                validationResult.IsSuccess = false;

                return validationResult;
            }

            // Validate the VPN route targets

            var routeTargetsValidationResult = await RouteTargetService.ValidateRouteTargetsAsync(vpn.VpnID);
            if (!routeTargetsValidationResult.IsSuccess)
            {
                validationResult.Add($"The route targets for VPN '{vpn.Name}' are not configured correctly. "
                    + "Resolve this issue and try again. ");
                validationResult.Add(routeTargetsValidationResult.GetMessage());
                validationResult.IsSuccess = false;

                return validationResult;
            }

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
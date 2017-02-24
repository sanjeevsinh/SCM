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
        public VpnAttachmentSetService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
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

        public async Task<ServiceResult> ValidateVpnAttachmentSetAsync(VpnAttachmentSet vpnAttachmentSet)
        {
            var validationResult = new ServiceResult();
            validationResult.IsSuccess = true;

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
                validationResult.Add("The Attachment Set was not found");
                validationResult.IsSuccess = false;
                return validationResult;
            }

            var attachmentRedundancyName = attachmentSet.AttachmentRedundancy.Name;

            if (vpn.Plane == null)
            {

                if (attachmentRedundancyName == "Silver" || attachmentRedundancyName == "Gold")
                {

                    var redPlaneAttachmentCount = attachmentSet.AttachmentSetVrfs.Where(v => v.Vrf.Device.Plane.Name == "Red").Count();
                    var bluePlaneAttachmentCount = attachmentSet.AttachmentSetVrfs.Where(v => v.Vrf.Device.Plane.Name == "Blue").Count();

                    if (redPlaneAttachmentCount == 0)
                    {
                        validationResult.Add("This Attachment Set does not contain any attachments into the Red Plane. "
                            + " There must be at least one Red Plane attachment in the set.");
                        validationResult.IsSuccess = false;
                    }

                    if (bluePlaneAttachmentCount == 0)
                    {
                        validationResult.Add("This Attachment Set does not contain any attachments into the Blue Plane. "
                            + " There must be at least one Blue Plane attachment in the set.");
                        validationResult.IsSuccess = false;
                    }
                }
            }
            else 
            {
                if (attachmentRedundancyName == "Gold")
                {
                    validationResult.Add("A Gold Attachment Set cannot be used with a Planar-Scoped VPN. The VPN is scoped to the '" + vpn.Plane.Name + "' Plane. " +
                        "A Gold Attachment Set provides connectivity into both Planes.");
                    validationResult.IsSuccess = false;
                }

                else if (attachmentRedundancyName == "Silver" || attachmentRedundancyName == "Custom")
                {

                    if (attachmentSet.AttachmentSetVrfs.Where(v => v.Vrf.Device.Plane.Name == vpn.Plane.Name).Count() != attachmentSet.AttachmentSetVrfs.Count())
                    {
                        validationResult.Add("One or more VRFs in the Attachment Set are not located in the required Plane ('" + vpn.Plane.Name + "') for the VPN.");
                        validationResult.IsSuccess = false;
                    }
                }
            }

            return validationResult;
        }
    }
}
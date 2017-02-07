using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using System.Threading.Tasks;

namespace SCM.Services.SCMServices
{
    public class AttachmentSetVrfService : BaseService, IAttachmentSetVrfService
    {
        public AttachmentSetVrfService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<AttachmentSetVrf>> GetAllAsync()
        {
            return await this.UnitOfWork.AttachmentSetVrfRepository.GetAsync();
        }

        public async Task<AttachmentSetVrf> GetByIDAsync(int key)
        {
            return await UnitOfWork.AttachmentSetVrfRepository.GetByIDAsync(key);
        }

        public async Task<int> AddAsync(AttachmentSetVrf attachmentSetVrf)
        {
            this.UnitOfWork.AttachmentSetVrfRepository.Insert(attachmentSetVrf);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> UpdateAsync(AttachmentSetVrf attachmentSetVrf)
        {
            this.UnitOfWork.AttachmentSetVrfRepository.Update(attachmentSetVrf);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(AttachmentSetVrf attachmentSetVrf)
        {
            this.UnitOfWork.AttachmentSetVrfRepository.Delete(attachmentSetVrf);
            return await this.UnitOfWork.SaveAsync();
        }

        /// <summary>
        /// Validates that a VRF can be added to or removed from an Attachment Set.
        /// </summary>
        /// <param name="attachmentSetVrf"></param>
        /// <returns></returns>
        public async Task<ServiceValidationResult> ValidateAddRemoveVrfAsync(AttachmentSetVrf attachmentSetVrf)
        {
            var validationResult = new ServiceValidationResult();
            validationResult.IsValid = true;

            var vpnAttachmentSets = await UnitOfWork.VpnAttachmentSetRepository.GetAsync(q => q.AttachmentSetID == attachmentSetVrf.AttachmentSetID, 
                includeProperties:"Vpn", AsTrackable: false);
            if (vpnAttachmentSets.Count() > 0)
            {
                validationResult.Add("VRFs cannot be added or removed because the Attachment Set is used in the following VPNs: ");
                validationResult.Add(string.Join(",", vpnAttachmentSets.Select(v => v.Vpn.Name).ToArray()) + ". ");
                validationResult.Add("Remove the Attachment Set from the VPNs first.");
                validationResult.IsValid = false;
            }

            return validationResult;
        }
 
        public async Task<ServiceValidationResult> ValidateVrfsAsync(AttachmentSet attachmentSet)
        {

            var validationResult = new ServiceValidationResult();
            validationResult.IsValid = true;

            IList<AttachmentSetVrf> attachmentSetVrfs = await this.UnitOfWork.AttachmentSetVrfRepository.GetAsync(q => q.AttachmentSetID == attachmentSet.AttachmentSetID,
                includeProperties: "Vrf.Device.Location.SubRegion.Region,Vrf.Device.Plane");

            var attachmentRedundancy = attachmentSet.AttachmentRedundancy.Name;

            if (attachmentRedundancy == "Bronze")
            {
                if (attachmentSetVrfs.Count != 1)
                {
                    validationResult.Add("One, and no more than one, VRF for a Bronze Attachment Set must be defined.");
                    validationResult.IsValid = false;
                }
            }
            else
            {

                if (attachmentRedundancy == "Silver")

                {

                    if (attachmentSetVrfs.Count != 2)
                    {
                        validationResult.Add("Two, and no more than two, VRFs for a Silver Attachment Set must be defined."
                            + "Each VRF must be in the same location.");
                        validationResult.IsValid = false;
                        return validationResult;
                    }

                    var locationA = attachmentSetVrfs[0].Vrf.Device.Location;
                    var locationB = attachmentSetVrfs[1].Vrf.Device.Location;
                    var planeA = attachmentSetVrfs[0].Vrf.Device.Plane;
                    var planeB = attachmentSetVrfs[1].Vrf.Device.Plane;

                    if (locationA.LocationID != locationB.LocationID)
                    {
                        validationResult.Add("The Location for each VRF in a Silver Attachment Set must be the same.");
                        validationResult.IsValid = false;
                    }

                    if (planeA.PlaneID == planeB.PlaneID)
                    {
                        validationResult.Add("The Plane for each VRF in a Silver Attachment Set must be different.");
                        validationResult.IsValid = false;
                    }
                }

                else if (attachmentRedundancy == "Gold")
                {

                    if (attachmentSetVrfs.Count != 2)
                    {
                        validationResult.Add("Two, and no more than two, VRFs for a Gold Attachment Set must be defined. "
                            + "Each VRF must be in a different location.");
                        validationResult.IsValid = false;
                        return validationResult;
                    }

                    var locationA = attachmentSetVrfs[0].Vrf.Device.Location;
                    var locationB = attachmentSetVrfs[1].Vrf.Device.Location;
                    var planeA = attachmentSetVrfs[0].Vrf.Device.Plane;
                    var planeB = attachmentSetVrfs[1].Vrf.Device.Plane;
                    var subRegionA = locationA.SubRegion;
                    var subRegionB = locationB.SubRegion;

                    if (subRegionA.SubRegionID != subRegionB.SubRegionID)
                    {
                        validationResult.Add("The Sub-Region for each VRF in a Gold Attachment Set must be the same.");
                        validationResult.IsValid = false;
                    }

                    if (locationA.LocationID == locationB.LocationID)
                    {
                        validationResult.Add("The Location for each VRF in a Gold Attachment Set must be different.");
                        validationResult.IsValid = false;
                    }

                    if (planeA.PlaneID == planeB.PlaneID)
                    {
                        validationResult.Add("The Plane for each VRF in a Gold Attachment Set must be different.");
                        validationResult.IsValid = false;
                    }
                }
                else if (attachmentRedundancy == "Custom")
                {

                    if (attachmentSetVrfs.Count == 0)
                    {
                        validationResult.Add("At least one VRF is required for a Custom Attachment Set.");
                        validationResult.IsValid = false;
                    }
                }
            }

            return validationResult;
        }
    }
}
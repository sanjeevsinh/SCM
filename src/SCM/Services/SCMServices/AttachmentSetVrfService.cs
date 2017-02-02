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
        public async Task<string> ValidateVrfs(AttachmentSet attachmentSet)
        {

            var validationMessage = string.Empty;
            IList<AttachmentSetVrf> attachmentSetVrfs = await this.UnitOfWork.AttachmentSetVrfRepository.GetAsync(q => q.AttachmentSetID == attachmentSet.AttachmentSetID,
                includeProperties: "Vrf.Device.Location.SubRegion.Region,Vrf.Device.Plane");

            var attachmentRedundancy = attachmentSet.AttachmentRedundancy.Name;

            if (attachmentRedundancy == "Bronze")
            {
                if (attachmentSetVrfs.Count != 1)
                {
                    validationMessage += "One, and no more than one, VRF for a Bronze Attachment Set must be defined.";
                }
            }
            else
            {

                if (attachmentRedundancy == "Silver")

                {

                    if (attachmentSetVrfs.Count != 2)
                    {
                        validationMessage += "Two, and no more than two, VRFs for a Silver Attachment Set must be defined.";
                        return validationMessage;
                    }

                    var locationA = attachmentSetVrfs[0].Vrf.Device.Location;
                    var locationB = attachmentSetVrfs[1].Vrf.Device.Location;
                    var planeA = attachmentSetVrfs[0].Vrf.Device.Plane;
                    var planeB = attachmentSetVrfs[1].Vrf.Device.Plane;

                    if (locationA.LocationID != locationB.LocationID)
                    {
                        validationMessage += "The Location for each VRF in a Silver Attachment Set must be the same.";
                    }

                    if (planeA.PlaneID == planeB.PlaneID)
                    {
                        validationMessage += "The Plane for each VRF in a Silver Attachment Set must be different.";
                    }
                }

                else if (attachmentRedundancy == "Gold")
                {

                    if (attachmentSetVrfs.Count != 2)
                    {
                        validationMessage += "Two, and no more than two, VRFs for a Gold Attachment Set must be defined.";
                        return validationMessage;
                    }

                    var locationA = attachmentSetVrfs[0].Vrf.Device.Location;
                    var locationB = attachmentSetVrfs[1].Vrf.Device.Location;
                    var planeA = attachmentSetVrfs[0].Vrf.Device.Plane;
                    var planeB = attachmentSetVrfs[1].Vrf.Device.Plane;
                    var subRegionA = locationA.SubRegion;
                    var subRegionB = locationB.SubRegion;

                    if (subRegionA.SubRegionID != subRegionB.SubRegionID)
                    {
                        validationMessage += "The Sub-Region for each VRF in a Gold Attachment Set must be the same.";
                    }

                    if (locationA.LocationID == locationB.LocationID)
                    {
                        validationMessage += "The Location for each VRF in a Gold Attachment Set must be different.";
                    }

                    if (planeA.PlaneID == planeB.PlaneID)
                    {
                        validationMessage += "The Plane for each VRF in a Gold Attachment Set must be different.";
                    }
                }
            }

            return validationMessage;
        }
    }
}
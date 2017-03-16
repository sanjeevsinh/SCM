using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using System.Threading.Tasks;

namespace SCM.Services.SCMServices
{
    public class AttachmentSetService : BaseService, IAttachmentSetService
    {
        public AttachmentSetService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<AttachmentSet>> GetAllAsync()
        {
            return await this.UnitOfWork.AttachmentSetRepository.GetAsync(includeProperties: "Tenant,SubRegion,Region,AttachmentRedundancy");
        }

        public async Task<AttachmentSet> GetByIDAsync(int key)
        {
            return await UnitOfWork.AttachmentSetRepository.GetByIDAsync(key);
        }

        public async Task<int> AddAsync(AttachmentSet attachmentSet)
        {
            this.UnitOfWork.AttachmentSetRepository.Insert(attachmentSet);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> UpdateAsync(AttachmentSet attachmentSet)
        {
            this.UnitOfWork.AttachmentSetRepository.Update(attachmentSet);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(AttachmentSet attachmentSet)
        {
            this.UnitOfWork.AttachmentSetRepository.Delete(attachmentSet);
            return await this.UnitOfWork.SaveAsync();
        }
        public async Task<ServiceResult> ValidateChangesAsync(AttachmentSet attachmentSet)
        {
            var validationResult = new ServiceResult();
            validationResult.IsSuccess = true;

            var dbResult = await UnitOfWork.AttachmentSetRepository.GetAsync(q => q.AttachmentSetID == attachmentSet.AttachmentSetID, 
                includeProperties: "AttachmentSetVrfs", AsTrackable: false);
            var currentAttachmentSet = dbResult.SingleOrDefault();
            if (currentAttachmentSet == null)
            {
                validationResult.Add("The attachment set was not found.");
                validationResult.IsSuccess = false;
                return validationResult;
            }

            if (currentAttachmentSet.AttachmentSetVrfs.Count() > 0)
            {
                if (attachmentSet.AttachmentRedundancyID != currentAttachmentSet.AttachmentRedundancyID)
                {
                    validationResult.Add("The attachment redundancy option cannot be changed because VRFs are defined.");
                    validationResult.IsSuccess = false;
                }
                if (attachmentSet.RegionID != currentAttachmentSet.RegionID)
                {
                    validationResult.Add("The region cannot be changed because VRFs are defined.");
                    validationResult.IsSuccess = false;
                }
                if (attachmentSet.SubRegionID != currentAttachmentSet.SubRegionID)
                {
                    validationResult.Add("The sub-region cannot be changed because VRFs are defined.");
                    validationResult.IsSuccess = false;
                }
                if (attachmentSet.TenantID != currentAttachmentSet.TenantID)
                {
                    validationResult.Add("The tenant cannot be changed because VRFs are defined.");
                    validationResult.IsSuccess = false;
                }
                if (attachmentSet.IsLayer3 != currentAttachmentSet.IsLayer3)
                {
                    validationResult.Add("Layer 3 cannot be changed because VRFs are defined.");
                    validationResult.IsSuccess = false;
                }
            }

            return validationResult;
        }
    }
}
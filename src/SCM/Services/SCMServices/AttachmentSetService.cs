﻿using System;
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

        public async Task<AttachmentSet> GetByIDAsync(int id)
        {
            var dbResult = await UnitOfWork.AttachmentSetRepository.GetAsync(q => q.AttachmentSetID == id,
                includeProperties: "Tenant,"
                + "SubRegion,"
                + "Region,"
                + "AttachmentRedundancy,"
                + "AttachmentSetVrfs.Vrf.Device.Plane", AsTrackable: false);

            return dbResult.SingleOrDefault();
        }

        public async Task<IEnumerable<AttachmentSet>> GetAllByVpnIDAsync(int id)
        {
            return await UnitOfWork.AttachmentSetRepository.GetAsync(q => q.VpnAttachmentSets
                .Select(r => r.VpnID == id)
                .Count() > 0, includeProperties: "Tenant,"
                + "SubRegion,"
                + "Region,"
                + "AttachmentRedundancy", AsTrackable: false);
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
        public async Task<ServiceResult> ValidateNewAsync(AttachmentSet attachmentSet)
        {
            return await ValidateAttachmentRedundancy(attachmentSet);
        }

        public async Task<ServiceResult> ValidateDeleteAsync(AttachmentSet attachmentSet)
        {
            var result = new ServiceResult { IsSuccess = true };

            var vpnAttachmentSets = await UnitOfWork.VpnAttachmentSetRepository.GetAsync(q => q.AttachmentSetID == attachmentSet.AttachmentSetID,
                includeProperties:"Vpn");

            if (vpnAttachmentSets.Count > 0)
            {
                result.IsSuccess = false;
                result.Add("You must complete the following first before the Attachment Set can be deleted: ");
                vpnAttachmentSets.ToList().ForEach(q => result.Add($"Remove the Attachment Set from VPN '{q.Vpn.Name}'"));
            }

            return result;
        }

        public async Task<ServiceResult> ValidateChangesAsync(AttachmentSet attachmentSet)
        {
            var validationResult = new ServiceResult { IsSuccess = true };

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

            var validateAttachmentRedundancyResult = await ValidateAttachmentRedundancy(attachmentSet);
            if (!validateAttachmentRedundancyResult.IsSuccess)
            {
                validationResult.AddRange(validateAttachmentRedundancyResult.GetMessageList());
                validationResult.IsSuccess = false;
            }

            return validationResult;
        }

        private async Task<ServiceResult> ValidateAttachmentRedundancy(AttachmentSet attachmentSet)
        {
            var validationResult = new ServiceResult { IsSuccess = true };

            var attachmentRedundancy = await UnitOfWork.AttachmentRedundancyRepository.GetByIDAsync(attachmentSet.AttachmentRedundancyID);
            if (attachmentRedundancy.Name == "Gold")
            {
                if (attachmentSet.SubRegionID == null)
                {
                    validationResult.IsSuccess = false;
                    validationResult.Add("A sub-region must be selected for gold attachment sets.");
                }
            }

            return validationResult;
        }
    }
}
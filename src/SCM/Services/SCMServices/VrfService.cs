using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SCM.Services.SCMServices
{
    public class VrfService : BaseService, IVrfService
    {
        public VrfService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<Vrf> GetByIDAsync(int key)
        {
            return await UnitOfWork.VrfRepository.GetByIDAsync(key);
        }

        public async Task<ServiceResult> AddAsync(Vrf vrf)
        {
            var result = new ServiceResult { IsSuccess = true };

            var tenant = await UnitOfWork.TenantRepository.GetByIDAsync(vrf.TenantID);
            if (tenant == null)
            {
                result.Add("Unable to create a VRF. The tenant was not found.");
                result.IsSuccess = false;

                return result;
            }

            var dbResult = await UnitOfWork.RouteDistinguisherRangeRepository.GetAsync(q => q.Name == "Default");
            var rdRange = dbResult.SingleOrDefault();

            if (rdRange == null)
            {
                result.Add("The default route distinguisher range was not found.");
                result.IsSuccess = false;

                return result;
            }

            var usedRDs = await UnitOfWork.VrfRepository.GetAsync(q => q.RouteDistinguisherRangeID == rdRange.RouteDistinguisherRangeID);

            // Allocate a new unused RD from the RD range

            int? newRdAssignedNumberSubField = Enumerable.Range(rdRange.AssignedNumberSubFieldStart, rdRange.AssignedNumberSubFieldCount)
                .Except(usedRDs.Select(q => q.AssignedNumberSubField)).FirstOrDefault();

            if (newRdAssignedNumberSubField == null)
            {
                result.Add("Failed to allocate a free route distinguisher. Please contact your administrator, or try another range.");
                result.IsSuccess = false;

                return result;
            }

            vrf.AdministratorSubField = rdRange.AdministratorSubField;
            vrf.AssignedNumberSubField = newRdAssignedNumberSubField.Value;
            vrf.RouteDistinguisherRangeID = rdRange.RouteDistinguisherRangeID;

            // Temp name for VRF needed to insert into the DB.

            vrf.Name = "temp";

            try
            {
                this.UnitOfWork.VrfRepository.Insert(vrf);

                // Save to generate unique Vrf ID

                await this.UnitOfWork.SaveAsync();

                // Generate unique VRF name - concatenate tenant name with VRF ID

                vrf.Name = $"{tenant.Name}-{vrf.VrfID}";
                this.UnitOfWork.VrfRepository.Update(vrf);

                await this.UnitOfWork.SaveAsync();
            }

            catch (DbUpdateException /** ex **/)
            {
                // Add logging for the exception here
                result.Add("Something went wrong during the database update. The issue has been logged."
                   + "Please try again, and contact your system admin if the problem persists.");

                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<int> UpdateAsync(Vrf vrf)
        {
            this.UnitOfWork.VrfRepository.Update(vrf);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(Vrf vrf)
        {
            this.UnitOfWork.VrfRepository.Delete(vrf);
            return await this.UnitOfWork.SaveAsync();
        }

        /// <summary>
        /// Validate if a VRF can be deleted. A VRF cannot be deleted if it is bound 
        /// to one or more attachment sets.
        /// </summary>
        /// <param name="vrfID"></param>
        /// <returns></returns>
        public async Task<ServiceResult> ValidateDeleteAsync(int vrfID)
        {
            var result = new ServiceResult();
            result.IsSuccess = true;

            var attachmentSets = await UnitOfWork.AttachmentSetRepository.GetAsync(q => q.AttachmentSetVrfs
                    .Where(v => v.VrfID == vrfID).Count() > 0);

            if (attachmentSets.Count() > 0)
            {
                result.Add("The VRF cannot be deleted. The VRF is bound to attachment sets.");
                result.Add("Perform the following then try again: ");
                result.AddRange(attachmentSets.ToList().Select(q => $"Remove the VRF from attachment set {q.Name}."));

                result.IsSuccess = false;
            }

            return result;
        }
    }
}
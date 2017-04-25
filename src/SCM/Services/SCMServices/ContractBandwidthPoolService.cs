using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Models.ServiceModels;
using SCM.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace SCM.Services.SCMServices
{
    public class ContractBandwidthPoolService : BaseService, IContractBandwidthPoolService
    {
        public ContractBandwidthPoolService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public async Task<IEnumerable<ContractBandwidthPool>> GetAllAsync()
        {
            return await this.UnitOfWork.ContractBandwidthPoolRepository.GetAsync();
        }

        public async Task<ContractBandwidthPool> GetByIDAsync(int id)
        {
            return await this.UnitOfWork.ContractBandwidthPoolRepository.GetByIDAsync(id);
        }

        public async Task<ServiceResult> AddAsync(AttachmentRequest request)
        {
            var contractBandwidthPool = Mapper.Map<ContractBandwidthPool>(request);
            return await AddContractBandwidthPoolAsync(contractBandwidthPool);
        }

        public async Task<ServiceResult> AddAsync(VifRequest request)
        {
            var contractBandwidthPool = Mapper.Map<ContractBandwidthPool>(request);
            return await AddContractBandwidthPoolAsync(contractBandwidthPool);
        }

        public async Task<int> UpdateAsync(ContractBandwidthPool contractBandwidthPool)
        {
            this.UnitOfWork.ContractBandwidthPoolRepository.Update(contractBandwidthPool);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(ContractBandwidthPool contractBandwidthPool)
        {
            this.UnitOfWork.ContractBandwidthPoolRepository.Delete(contractBandwidthPool);
            return await this.UnitOfWork.SaveAsync();
        }

        /// <summary>
        /// Validate a request for a new Contract Bandwidth Pool to be associated
        /// with an Attachment.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ServiceResult> ValidateNewAsync(AttachmentRequest request)
        {
            var result = new ServiceResult { IsSuccess = true };

            var contractBandwidth = await UnitOfWork.ContractBandwidthRepository.GetByIDAsync(request.ContractBandwidthID);

            if (contractBandwidth.BandwidthMbps > request.Bandwidth.BandwidthGbps * 1000)
            {
                result.Add($"The requested contract bandwidth of {contractBandwidth.BandwidthMbps} Mbps exceeds the "
                    + $"attachment bandwidth of {request.Bandwidth.BandwidthGbps} Gbps.");
                result.Add("Select a lower contract bandwidth or request a higher bandwidth attachment.");
                result.IsSuccess = false;
            }

            return result;
        }

        /// <summary>
        /// Validate a request for a new Contract Bandwidth Pool to be associated with 
        /// a VIF.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ServiceResult> ValidateNewAsync(VifRequest request)
        {
            var result = new ServiceResult { IsSuccess = true };

            var vifs = await UnitOfWork.VifRepository.GetAsync(q => q.AttachmentID == request.AttachmentID,
                    includeProperties: "ContractBandwidthPool.ContractBandwidth",
                    AsTrackable: false);

            if (request.ContractBandwidthPoolID != null)
            {
                // Request is to share an existing Contract Bandwidth Pool

                if (vifs.Where(q => q.ContractBandwidthPoolID == request.ContractBandwidthPoolID).Count() == 0)
                {
                    // The request is invalid - a request to share a Contract Bandwidth Pool must be for vifs
                    // associated with a common Attachment

                    result.Add("The selected Contract Bandwidth Pool is invalid because it is not associated with any vif "
                        + "of the current attachment.");
                    result.IsSuccess = false;

                    return result;
                }

                var contractBandwidthPool = await UnitOfWork.ContractBandwidthPoolRepository.GetByIDAsync(request.ContractBandwidthPoolID);
                if (contractBandwidthPool.TenantID != request.TenantID)
                {
                    // The request is invalid - a request to share a Contract Bandwidth Pool must be for vifs
                    // associated with a common Tenant

                    result.Add("The selected Contract Bandwidth Pool is invalid because it does not belong to the tenant "
                        + "specified in the request.");
                    result.IsSuccess = false;

                    return result;
                }
            }

            // Get arguments needed to check for sufficient bandwidth

            var attachmentResult = await UnitOfWork.AttachmentRepository.GetAsync(q => q.AttachmentID == request.AttachmentID, 
                includeProperties:"AttachmentBandwidth");
            var attachmentBandwidth = attachmentResult.Single().AttachmentBandwidth;

            // Calculate aggregate bandwidth used from distinct Contract Bandwidth Pool assignments

            var aggregateContractBandwidthMbps = vifs.GroupBy(q => q.ContractBandwidthPoolID)
                .Select(group => group.First())
                .Sum(q => q.ContractBandwidthPool.ContractBandwidth.BandwidthMbps);
     

            // If a Contract Bandwidth is specified then the required bandwidth must be added to the current aggregate bandwidth.
            // If a Contract Bandwidth Pool is specified then the required bandwidth is already accounted for since the bandwidth
            // requested is to be shared with other vifs

            int requestedBandwidthMbps = 0;
            if (request.ContractBandwidthID != null)
            { 
                var contractBandwidth = await UnitOfWork.ContractBandwidthRepository.GetByIDAsync(request.ContractBandwidthID);
                requestedBandwidthMbps = contractBandwidth.BandwidthMbps;
            }

            // Check sufficient bandwidth is available

            if ((aggregateContractBandwidthMbps + requestedBandwidthMbps) > attachmentBandwidth.BandwidthGbps * 1000)
            {
                result.Add("The selected contract bandwidth exceeds the remaining bandwidth of the attachment.");
                result.Add($"Remaining bandwidth : {(attachmentBandwidth.BandwidthGbps * 1000) - aggregateContractBandwidthMbps} Mbps.");
                result.Add($"Requested bandwidth : {requestedBandwidthMbps} Mbps.");

                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<ServiceResult> ValidateDeleteAsync(Vif vif)
        {
            var result = new ServiceResult { IsSuccess = true };

            var vifs = await UnitOfWork.VifRepository.GetAsync(q => q.AttachmentID == vif.AttachmentID,
                AsTrackable: false);

            if (vifs.Where(q => q.ContractBandwidthPoolID == vif.ContractBandwidthPoolID).Count() > 1)
            {
                result.Add("The Contract Bandwidth Pool cannot be deleted because it is shared by other VIFs.");
                result.IsSuccess = false;

                return result;
            }
 
            return result;
        }

        private async Task<ServiceResult> AddContractBandwidthPoolAsync(ContractBandwidthPool contractBandwidthPool)
        {
            var result = new ServiceResult
            {
                IsSuccess = true,
                Item = contractBandwidthPool
            };

            var tenant = await UnitOfWork.TenantRepository.GetByIDAsync(contractBandwidthPool.TenantID);
            if (tenant == null)
            {
                result.Add("Unable to create Contract Bandwidth Pool. The tenant was not found.");
                result.IsSuccess = false;

                return result;
            }

            contractBandwidthPool.Name = "temp";

            try
            {
                this.UnitOfWork.ContractBandwidthPoolRepository.Insert(contractBandwidthPool);
                await this.UnitOfWork.SaveAsync();
                contractBandwidthPool.Name = $"{tenant.Name}-{contractBandwidthPool.ContractBandwidthPoolID}";
                this.UnitOfWork.ContractBandwidthPoolRepository.Update(contractBandwidthPool);

                return result;
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
    }
}

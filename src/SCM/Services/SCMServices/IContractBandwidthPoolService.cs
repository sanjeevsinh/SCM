using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Data;
using SCM.Models.ServiceModels;

namespace SCM.Services.SCMServices
{
    public interface IContractBandwidthPoolService
    {
        IUnitOfWork UnitOfWork { get; }
        Task<IEnumerable<ContractBandwidthPool>> GetAllAsync();
        Task<ContractBandwidthPool> GetByIDAsync(int id);
        Task<ServiceResult> AddAsync(AttachmentRequest requqest);
        Task<ServiceResult> AddAsync(VifRequest requqest);
        Task<int> UpdateAsync(ContractBandwidthPool contractBandwidthPool);
        Task<int> DeleteAsync(ContractBandwidthPool contractBandwidthPool);
        Task<ServiceResult> ValidateAsync(AttachmentRequest request);
        Task<ServiceResult> ValidateAsync(VifRequest request);
        Task<ServiceResult> ValidateDeleteAsync(Vif vif);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Data;

namespace SCM.Services.SCMServices
{
    public interface IContractBandwidthPoolService
    {
        IUnitOfWork UnitOfWork { get; }
        Task<IEnumerable<ContractBandwidthPool>> GetAllAsync();
        Task<ContractBandwidthPool> GetByIDAsync(int id);
        Task<int> AddAsync(ContractBandwidthPool contractBandwidthPool);
        Task<int> UpdateAsync(ContractBandwidthPool contractBandwidthPool);
        Task<int> DeleteAsync(ContractBandwidthPool contractBandwidthPool);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Data;

namespace SCM.Services.SCMServices
{
    public class ContractBandwidthPoolService : BaseService, IContractBandwidthPoolService
    {
        public ContractBandwidthPoolService(IUnitOfWork unitOfWork) : base(unitOfWork)
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

        public async Task<int> AddAsync(ContractBandwidthPool contractBandwidthPool)
        {
            this.UnitOfWork.ContractBandwidthPoolRepository.Insert(contractBandwidthPool);
            return await this.UnitOfWork.SaveAsync();
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
    }
}

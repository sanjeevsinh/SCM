using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using System.Threading.Tasks;

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

        public async Task<int> AddAsync(Vrf vrf)
        {
            this.UnitOfWork.VrfRepository.Insert(vrf);
            return await this.UnitOfWork.SaveAsync();
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
    }
}
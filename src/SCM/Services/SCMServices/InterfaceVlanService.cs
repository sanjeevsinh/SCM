using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using System.Threading.Tasks;

namespace SCM.Services.SCMServices
{
    public class InterfaceVlanService : BaseService, IInterfaceVlanService
    {
        public InterfaceVlanService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<InterfaceVlan> GetByIDAsync(int key)
        {
            return await UnitOfWork.InterfaceVlanRepository.GetByIDAsync(key);
        }

        public async Task<int> AddAsync(InterfaceVlan ifaceVlan)
        {
            this.UnitOfWork.InterfaceVlanRepository.Insert(ifaceVlan);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> UpdateAsync(InterfaceVlan ifaceVlan)
        {
            this.UnitOfWork.InterfaceVlanRepository.Update(ifaceVlan);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(InterfaceVlan ifaceVlan)
        {
            this.UnitOfWork.InterfaceVlanRepository.Delete(ifaceVlan);
            return await this.UnitOfWork.SaveAsync();
        }
    }
}
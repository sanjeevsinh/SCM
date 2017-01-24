using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using System.Threading.Tasks;

namespace SCM.Services.SCMServices
{
    public class InterfaceService : BaseService, IInterfaceService
    {
        public InterfaceService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<Interface> GetByIDAsync(int key)
        {
            return await UnitOfWork.InterfaceRepository.GetByIDAsync(key);
        }

        public async Task<int> AddAsync(Interface iface)
        {
            this.UnitOfWork.InterfaceRepository.Insert(iface);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> UpdateAsync(Interface iface)
        {
            this.UnitOfWork.InterfaceRepository.Update(iface);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(Interface iface)
        {
            this.UnitOfWork.InterfaceRepository.Delete(iface);
            return await this.UnitOfWork.SaveAsync();
        }
    }
}
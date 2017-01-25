using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using System.Threading.Tasks;

namespace SCM.Services.SCMServices
{
    public class BundleInterfaceService : BaseService, IBundleInterfaceService
    {
        public BundleInterfaceService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<BundleInterface> GetByIDAsync(int key)
        {
            return await UnitOfWork.BundleInterfaceRepository.GetByIDAsync(key);
        }

        public async Task<int> AddAsync(BundleInterface bundleIface)
        {
            this.UnitOfWork.BundleInterfaceRepository.Insert(bundleIface);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> UpdateAsync(BundleInterface bundleIface)
        {
            this.UnitOfWork.BundleInterfaceRepository.Update(bundleIface);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(BundleInterface bundleIface)
        {
            this.UnitOfWork.BundleInterfaceRepository.Delete(bundleIface);
            return await this.UnitOfWork.SaveAsync();
        }
    }
}
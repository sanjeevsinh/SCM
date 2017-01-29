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

        public async Task<int> AddAsync(BundleInterface bundleIfacePort)
        {
            this.UnitOfWork.BundleInterfaceRepository.Insert(bundleIfacePort);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> UpdateAsync(BundleInterface bundleIfacePort)
        {
            this.UnitOfWork.BundleInterfaceRepository.Update(bundleIfacePort);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(BundleInterface bundleIfacePort)
        {
            this.UnitOfWork.BundleInterfaceRepository.Delete(bundleIfacePort);
            return await this.UnitOfWork.SaveAsync();
        }
    }
}
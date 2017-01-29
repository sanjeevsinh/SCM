using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using System.Threading.Tasks;

namespace SCM.Services.SCMServices
{
    public class BundleInterfacePortService : BaseService, IBundleInterfacePortService
    {
        public BundleInterfacePortService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<BundleInterfacePort> GetByIDAsync(int key)
        {
            return await UnitOfWork.BundleInterfacePortRepository.GetByIDAsync(key);
        }

        public async Task<int> AddAsync(BundleInterfacePort bundleIfacePort)
        {
            this.UnitOfWork.BundleInterfacePortRepository.Insert(bundleIfacePort);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> UpdateAsync(BundleInterfacePort bundleIfacePort)
        {
            this.UnitOfWork.BundleInterfacePortRepository.Update(bundleIfacePort);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(BundleInterfacePort bundleIfacePort)
        {
            this.UnitOfWork.BundleInterfacePortRepository.Delete(bundleIfacePort);
            return await this.UnitOfWork.SaveAsync();
        }
    }
}
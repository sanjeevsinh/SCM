using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using System.Threading.Tasks;

namespace SCM.Services.SCMServices
{
    public class BundleInterfaceVlanService : BaseService, IBundleInterfaceVlanService
    {
        public BundleInterfaceVlanService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<BundleInterfaceVlan> GetByIDAsync(int key)
        {
            return await UnitOfWork.BundleInterfaceVlanRepository.GetByIDAsync(key);
        }

        public async Task<int> AddAsync(BundleInterfaceVlan bundleIfaceVlan)
        {
            this.UnitOfWork.BundleInterfaceVlanRepository.Insert(bundleIfaceVlan);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> UpdateAsync(BundleInterfaceVlan bundleIfaceVlan)
        {
            this.UnitOfWork.BundleInterfaceVlanRepository.Update(bundleIfaceVlan);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(BundleInterfaceVlan bundleIfaceVlan)
        {
            this.UnitOfWork.BundleInterfaceVlanRepository.Delete(bundleIfaceVlan);
            return await this.UnitOfWork.SaveAsync();
        }
    }
}
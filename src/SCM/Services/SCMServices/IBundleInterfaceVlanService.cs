using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Data;

namespace SCM.Services.SCMServices
{
    public interface IBundleInterfaceVlanService
    {
        IUnitOfWork UnitOfWork { get; }
        Task<BundleInterfaceVlan> GetByIDAsync(int id);
        Task<int> AddAsync(BundleInterfaceVlan bundleIfaceVlan);
        Task<int> UpdateAsync(BundleInterfaceVlan bundleIfaceVlan);
        Task<int> DeleteAsync(BundleInterfaceVlan bundleIfaceVlan);
    }
}

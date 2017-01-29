using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Data;

namespace SCM.Services.SCMServices
{
    public interface IBundleInterfacePortService
    {
        IUnitOfWork UnitOfWork { get; }
        Task<BundleInterfacePort> GetByIDAsync(int id);
        Task<int> AddAsync(BundleInterfacePort bundleIfacePort);
        Task<int> UpdateAsync(BundleInterfacePort bundleIfacePort);
        Task<int> DeleteAsync(BundleInterfacePort bundleIfacePort);
    }
}

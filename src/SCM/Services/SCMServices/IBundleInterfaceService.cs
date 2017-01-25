using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Data;

namespace SCM.Services.SCMServices
{
    public interface IBundleInterfaceService
    {
        IUnitOfWork UnitOfWork { get; }
        Task<BundleInterface> GetByIDAsync(int id);
        Task<int> AddAsync(BundleInterface bundleIface);
        Task<int> UpdateAsync(BundleInterface bundleIface);
        Task<int> DeleteAsync(BundleInterface bundleIface);
    }
}

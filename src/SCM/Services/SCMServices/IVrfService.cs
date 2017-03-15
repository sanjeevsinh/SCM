using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Data;

namespace SCM.Services.SCMServices
{
    public interface IVrfService
    {
        IUnitOfWork UnitOfWork { get; }
        Task<Vrf> GetByIDAsync(int id);
        Task<ServiceResult> AddAsync(Vrf vrf);
        Task<int> UpdateAsync(Vrf vrf);
        Task<int> DeleteAsync(Vrf vrf);
        Task<ServiceResult> ValidateDeleteAsync(int vrfID);
    }
}

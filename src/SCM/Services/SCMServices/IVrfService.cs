using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Data;
using SCM.Models.ServiceModels;

namespace SCM.Services.SCMServices
{
    public interface IVrfService
    {
        IUnitOfWork UnitOfWork { get; }
        Task<Vrf> GetByIDAsync(int id);
        Task<ServiceResult> AddAsync(AttachmentRequest request);
        Task<ServiceResult> AddAsync(VifRequest request);
        Task<int> UpdateAsync(Vrf vrf);
        Task<int> DeleteAsync(Vrf vrf);
        Task<ServiceResult> ValidateDeleteAsync(int vrfID);
    }
}

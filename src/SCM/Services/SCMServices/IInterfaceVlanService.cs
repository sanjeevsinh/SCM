using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Data;

namespace SCM.Services.SCMServices
{
    public interface IInterfaceVlanService
    {
        IUnitOfWork UnitOfWork { get; }
        Task<InterfaceVlan> GetByIDAsync(int id);
        Task<int> AddAsync(InterfaceVlan ifaceVlan);
        Task<int> UpdateAsync(InterfaceVlan ifaceVlan);
        Task<int> DeleteAsync(InterfaceVlan ifaceVlan);
        Task<ServiceValidationResult> ValidateInterfaceVlan(InterfaceVlan ifaceVlan);
        Task<ServiceValidationResult> ValidateInterfaceVlanChanges(InterfaceVlan ifaceVlan, InterfaceVlan currentIfaceVlan);
    }
}

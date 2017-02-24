using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Data;

namespace SCM.Services.SCMServices
{
    public interface IInterfaceService
    {
        IUnitOfWork UnitOfWork { get; }
        Task<Interface> GetByIDAsync(int id);
        Task<int> AddAsync(Interface iface);
        Task<int> UpdateAsync(Interface iface);
        Task<int> DeleteAsync(Interface iface);
        Task<ServiceResult> ValidateInterface(Interface iface);
        Task<ServiceResult> ValidateInterfaceChanges(Interface iface, Interface currentIface);
    }
}

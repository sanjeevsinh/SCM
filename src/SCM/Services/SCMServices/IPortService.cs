using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Data;

namespace SCM.Services.SCMServices
{
    public interface IPortService
    {
        IUnitOfWork UnitOfWork { get; }
        Task<Port> GetByIDAsync(int id);
        Task<int> AddAsync(Port port);
        Task<int> UpdateAsync(Port port);
        Task<int> DeleteAsync(Port port);
    }
}

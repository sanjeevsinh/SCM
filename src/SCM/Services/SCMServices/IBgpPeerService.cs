using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Data;

namespace SCM.Services.SCMServices
{
    public interface IBgpPeerService
    {
        IUnitOfWork UnitOfWork { get; }
        Task<IEnumerable<BgpPeer>> GetAllAsync();
        Task<BgpPeer> GetByIDAsync(int id);
        Task<int> AddAsync(BgpPeer bgpPeer);
        Task<int> UpdateAsync(BgpPeer bgpPeer);
        Task<int> DeleteAsync(BgpPeer bgpPeer);
    }
}

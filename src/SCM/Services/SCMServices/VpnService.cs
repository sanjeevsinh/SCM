using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using System.Threading.Tasks;

namespace SCM.Services.SCMServices
{
    public class VpnService : BaseService, IVpnService
    {
        public VpnService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<Vpn>> GetAllAsync()
        {
            return await this.UnitOfWork.VpnRepository.GetAsync(includeProperties: "Plane,VpnTenancyType,VpnTopologyType.VpnProtocolType,Tenant,Region");
        }

        public async Task<Vpn> GetByIDAsync(int key)
        {
            return await UnitOfWork.VpnRepository.GetByIDAsync(key);
        }

        public async Task<int> AddAsync(Vpn vpn)
        {
            this.UnitOfWork.VpnRepository.Insert(vpn);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> UpdateAsync(Vpn vpn)
        {
            this.UnitOfWork.VpnRepository.Update(vpn);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(Vpn vpn)
        {
            this.UnitOfWork.VpnRepository.Delete(vpn);
            return await this.UnitOfWork.SaveAsync();
        }
    }
}
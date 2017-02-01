using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using System.Threading.Tasks;

namespace SCM.Services.SCMServices
{
    public class RouteTargetService : BaseService, IRouteTargetService
    {
        public RouteTargetService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<RouteTarget>> GetAllAsync()
        {
            return await this.UnitOfWork.RouteTargetRepository.GetAsync(includeProperties: "Plane,RouteTargetTenancyType,RouteTargetTopologyType.RouteTargetProtocolType,Tenant,Region");
        }

        public async Task<RouteTarget> GetByIDAsync(int key)
        {
            return await UnitOfWork.RouteTargetRepository.GetByIDAsync(key);
        }

        public async Task<int> AddAsync(RouteTarget routeTarget)
        {
            this.UnitOfWork.RouteTargetRepository.Insert(routeTarget);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> UpdateAsync(RouteTarget routeTarget)
        {
            this.UnitOfWork.RouteTargetRepository.Update(routeTarget);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(RouteTarget routeTarget)
        {
            this.UnitOfWork.RouteTargetRepository.Delete(routeTarget);
            return await this.UnitOfWork.SaveAsync();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Data;

namespace SCM.Services.SCMServices
{
    public interface IRouteTargetService
    {
        IUnitOfWork UnitOfWork { get; }

        Task<IEnumerable<RouteTarget>> GetAllAsync();
        Task<RouteTarget> GetByIDAsync(int id);
        Task<int> AddAsync(RouteTarget routeTarget);
        Task<int> UpdateAsync(RouteTarget routeTarget);
        Task<int> DeleteAsync(RouteTarget routeTarget);
        Task<ServiceValidationResult> ValidateRouteTargetsAddRemoveAsync(RouteTarget routeTarget);
        Task<ServiceValidationResult> ValidateRouteTargetsAsync(int vpnID);
    }
}

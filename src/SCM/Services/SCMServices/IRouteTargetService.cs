using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Data;
using SCM.Models.ServiceModels;

namespace SCM.Services.SCMServices
{
    public interface IRouteTargetService
    {
        IUnitOfWork UnitOfWork { get; }

        Task<IEnumerable<RouteTarget>> GetAllAsync();
        Task<IEnumerable<RouteTarget>> GetAllByVpnIDAsync(int id);
        Task<RouteTarget> GetByIDAsync(int id);
        Task<ServiceResult> AddAsync(RouteTargetRequest request);
        Task<int> DeleteAsync(RouteTarget routeTarget);
        ServiceResult Validate(Vpn vpn);
        Task<ServiceResult> CheckVpnOkToAddOrRemoveRouteTargetAsync(int vpnID);
        Task<IEnumerable<RouteTarget>> AllocateAllVpnRouteTargetsAsync(string vpnTopologyType, ServiceResult result);
    }
}

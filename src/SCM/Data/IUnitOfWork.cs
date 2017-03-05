using SCM.Models;
using System.Threading.Tasks;

namespace SCM.Data
{
    public interface IUnitOfWork
    {
        GenericRepository<AttachmentSet> AttachmentSetRepository { get; }
        GenericRepository<AttachmentSetVrf> AttachmentSetVrfRepository { get; }
        GenericRepository<VpnAttachmentSet> VpnAttachmentSetRepository { get; }
        GenericRepository<AttachmentRedundancy> AttachmentRedundancyRepository { get; }
        GenericRepository<BgpPeer> BgpPeerRepository { get; }
        GenericRepository<BundleInterface> BundleInterfaceRepository { get; }
        GenericRepository<BundleInterfacePort> BundleInterfacePortRepository { get; }
        GenericRepository<BundleInterfaceVlan> BundleInterfaceVlanRepository { get; }
        GenericRepository<ContractBandwidth> ContractBandwidthRepository { get; }
        GenericRepository<ContractBandwidthPool> ContractBandwidthPoolRepository { get; }
        GenericRepository<Device> DeviceRepository { get; }
        GenericRepository<Interface> InterfaceRepository { get; }
        GenericRepository<InterfaceVlan> InterfaceVlanRepository { get; }
        GenericRepository<Location> LocationRepository { get; }
        GenericRepository<InterfaceBandwidth> InterfaceBandwidthRepository { get; }
        GenericRepository<PortBandwidth> PortBandwidthRepository { get; }
        GenericRepository<Port> PortRepository { get; }
        GenericRepository<Region> RegionRepository { get; }
        GenericRepository<RouteTarget> RouteTargetRepository { get; }
        GenericRepository<SubRegion> SubRegionRepository { get; }
        GenericRepository<Tenant> TenantRepository { get; }
        GenericRepository<TenantNetwork> TenantNetworkRepository { get; }
        GenericRepository<TenantCommunity> TenantCommunityRepository { get; }
        GenericRepository<Vpn> VpnRepository { get; }
        GenericRepository<VpnTenantNetwork> VpnTenantNetworkRepository { get; }
        GenericRepository<VpnTenantCommunity> VpnTenantCommunityRepository { get; }
        GenericRepository<VpnProtocolType> VpnProtocolTypeRepository { get; }
        GenericRepository<VpnTenancyType> VpnTenancyTypeRepository { get; }
        GenericRepository<VpnTopologyType> VpnTopologyTypeRepository { get; }
        GenericRepository<Plane> PlaneRepository { get; }
        GenericRepository<Vrf> VrfRepository { get; }
        Task<int> SaveAsync();
    }
}
using SCM.Models;
using System.Threading.Tasks;

namespace SCM.Data
{
    public interface IUnitOfWork
    {
        GenericRepository<Attachment> AttachmentRepository { get; }
        GenericRepository<AttachmentSet> AttachmentSetRepository { get; }
        GenericRepository<AttachmentSetVrf> AttachmentSetVrfRepository { get; }
        GenericRepository<VpnAttachmentSet> VpnAttachmentSetRepository { get; }
        GenericRepository<AttachmentRedundancy> AttachmentRedundancyRepository { get; }
        GenericRepository<BgpPeer> BgpPeerRepository { get; }
        GenericRepository<ContractBandwidth> ContractBandwidthRepository { get; }
        GenericRepository<ContractBandwidthPool> ContractBandwidthPoolRepository { get; }
        GenericRepository<Device> DeviceRepository { get; }
        GenericRepository<Interface> InterfaceRepository { get; }
        GenericRepository<Vlan> VlanRepository { get; }
        GenericRepository<Location> LocationRepository { get; }
        GenericRepository<AttachmentBandwidth> AttachmentBandwidthRepository { get; }
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
        GenericRepository<RouteTargetRange> RouteTargetRangeRepository { get; }
        GenericRepository<RouteDistinguisherRange> RouteDistinguisherRangeRepository { get; }
        GenericRepository<VlanTagRange> VlanTagRangeRepository { get; }
        GenericRepository<Vif> VifRepository { get; }
        Task<int> SaveAsync();
    }
}
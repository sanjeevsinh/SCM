using AutoMapper;
using SCM.Models;

namespace SCM.Models.ViewModels
{
    public class AutoMapperViewModelProfileConfiguration : Profile
    {
       public AutoMapperViewModelProfileConfiguration()
        {
            CreateMap<Tenant, TenantViewModel>().ReverseMap();
            CreateMap<Port, PortViewModel>().ReverseMap();
            CreateMap<Device, DeviceViewModel>().ReverseMap();
            CreateMap<Plane, PlaneViewModel>().ReverseMap();
            CreateMap<Location, LocationViewModel>().ReverseMap();
            CreateMap<PortBandwidth, PortBandwidthViewModel>().ReverseMap();
            CreateMap<Interface, InterfaceViewModel>().ReverseMap();
            CreateMap<InterfaceBandwidth, InterfaceBandwidthViewModel>().ReverseMap();
            CreateMap<Vrf, VrfViewModel>().ReverseMap();
            CreateMap<InterfaceVlan, InterfaceVlanViewModel>().ReverseMap();
            CreateMap<BundleInterface, BundleInterfaceViewModel>().ReverseMap();
            CreateMap<BundleInterfacePort, BundleInterfacePortViewModel>().ReverseMap();
            CreateMap<BundleInterfaceVlan, BundleInterfaceVlanViewModel>().ReverseMap();
            CreateMap<Vpn, VpnViewModel>().ReverseMap();
            CreateMap<Region, RegionViewModel>().ReverseMap();
            CreateMap<VpnTopologyType, VpnTopologyTypeViewModel>().ReverseMap();
            CreateMap<VpnProtocolType, VpnProtocolTypeViewModel>().ReverseMap();
            CreateMap<VpnTenancyType, VpnTenancyTypeViewModel>().ReverseMap();
            CreateMap<RouteTarget, RouteTargetViewModel>().ReverseMap();
            CreateMap<AttachmentSet, AttachmentSetViewModel>().ReverseMap();
            CreateMap<SubRegion, SubRegionViewModel>().ReverseMap();
            CreateMap<ContractBandwidth, ContractBandwidthViewModel>().ReverseMap();
            CreateMap<AttachmentRedundancy, AttachmentRedundancyViewModel>().ReverseMap();
            CreateMap<AttachmentSetVrf, AttachmentSetVrfViewModel>().ReverseMap();
            CreateMap<VpnAttachmentSet, VpnAttachmentSetViewModel>().ReverseMap();
            CreateMap<BgpPeer, BgpPeerViewModel>().ReverseMap();
            CreateMap<TenantNetwork, TenantNetworkViewModel>().ReverseMap();
            CreateMap<TenantCommunity, TenantCommunityViewModel>().ReverseMap();
            CreateMap<VpnTenantNetwork, VpnTenantNetworkViewModel>().ReverseMap();
            CreateMap<VpnTenantCommunity, VpnTenantCommunityViewModel>().ReverseMap();
            CreateMap<TenantAttachments, TenantAttachmentsViewModel>();
            CreateMap<AttachmentInterface, AttachmentInterfaceViewModel>();
            CreateMap<AttachmentBundleInterface, AttachmentBundleInterfaceViewModel>();
        }
    }
}
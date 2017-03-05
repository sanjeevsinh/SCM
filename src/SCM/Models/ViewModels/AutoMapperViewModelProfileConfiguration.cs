using AutoMapper;
using SCM.Models.ServiceModels;
using System.Linq;

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
            CreateMap<ContractBandwidthPool, ContractBandwidthPoolViewModel>().ReverseMap();
            CreateMap<AttachmentRedundancy, AttachmentRedundancyViewModel>().ReverseMap();
            CreateMap<AttachmentSetVrf, AttachmentSetVrfViewModel>()
                .ForMember(dest => dest.Attachment, conf => conf.ResolveUsing(new AttachmentSetVrfTypeResolver()))
                .ReverseMap();
            CreateMap<VpnAttachmentSet, VpnAttachmentSetViewModel>().ReverseMap();
            CreateMap<BgpPeer, BgpPeerViewModel>().ReverseMap();
            CreateMap<TenantNetwork, TenantNetworkViewModel>().ReverseMap();
            CreateMap<TenantCommunity, TenantCommunityViewModel>().ReverseMap();
            CreateMap<VpnTenantNetwork, VpnTenantNetworkViewModel>().ReverseMap();
            CreateMap<VpnTenantCommunity, VpnTenantCommunityViewModel>().ReverseMap();
            CreateMap<Attachment, AttachmentViewModel>().ReverseMap();
            CreateMap<AttachmentRequestViewModel, AttachmentRequest>();
        }

        public class AttachmentSetVrfTypeResolver : IValueResolver<AttachmentSetVrf, AttachmentSetVrfViewModel, AttachmentViewModel>
        {
            public AttachmentViewModel Resolve(AttachmentSetVrf source, AttachmentSetVrfViewModel destination, AttachmentViewModel destMember, ResolutionContext context)
            {
                var mapper = context.Mapper;
                var vrf = source.Vrf;

                if (vrf.Interfaces.Count == 1)
                {
                    var attachment = mapper.Map<Attachment>(vrf.Interfaces.Single());
                    return mapper.Map<AttachmentViewModel>(attachment);
                }
                else if (vrf.BundleInterfaces.Count == 1)
                {
                    var attachment = mapper.Map<Attachment>(vrf.BundleInterfaces.Single());
                    return mapper.Map<AttachmentViewModel>(attachment);
                }

                return null;
            }
        }
    }
}
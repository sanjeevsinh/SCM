using AutoMapper;
using SCM.Models.ServiceModels;
using System.Linq;
using System;

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
            CreateMap<BundleInterfacePort, BundleInterfacePortViewModel>().ReverseMap();
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
            CreateMap<AttachmentSetVrf, AttachmentSetVrfViewModel>().ConvertUsing(new AttachmentSetVrfTypeConverter());
            CreateMap<AttachmentSetVrfViewModel, AttachmentSetVrf>();
            CreateMap<VpnAttachmentSet, VpnAttachmentSetViewModel>().ReverseMap();
            CreateMap<BgpPeer, BgpPeerViewModel>().ReverseMap();
            CreateMap<TenantNetwork, TenantNetworkViewModel>().ReverseMap();
            CreateMap<TenantCommunity, TenantCommunityViewModel>().ReverseMap();
            CreateMap<VpnTenantNetwork, VpnTenantNetworkViewModel>().ReverseMap();
            CreateMap<VpnTenantCommunity, VpnTenantCommunityViewModel>().ReverseMap();
            CreateMap<AttachmentAndVifs, AttachmentViewModel>().ReverseMap();
            CreateMap<Vif, VifViewModel>()
                .ForMember(dest => dest.AttachmentIsMultiPort, conf => conf.MapFrom(src => src.Attachment.IsMultiPort))
                .ReverseMap();
            CreateMap<MultiPortVif, MultiPortVifViewModel>()
                .ForMember(dest => dest.MemberAttachmentName, conf => conf.MapFrom(src => src.MemberAttachment.Name))
                .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vrf.Name));
            CreateMap<AttachmentRequestViewModel, AttachmentRequest>();
            CreateMap<VifRequestViewModel, VifRequest>();
            CreateMap<AttachmentSetVrfRequestViewModel, AttachmentSetVrfRequest>();
            CreateMap<AttachmentOrVif, AttachmentOrVifViewModel>();
            CreateMap<RouteTargetRequestViewModel, RouteTargetRequest>();
        }

        public class AttachmentSetVrfTypeConverter : ITypeConverter<AttachmentSetVrf, AttachmentSetVrfViewModel>
        {
            public AttachmentSetVrfViewModel Convert(AttachmentSetVrf source, AttachmentSetVrfViewModel destination, ResolutionContext context)
            {
                var mapper = context.Mapper;
                var result = new AttachmentSetVrfViewModel();
                var vrf = source.Vrf;

                if (vrf.Interfaces.Count == 1)
                {
                    var attachment = mapper.Map<Attachment>(vrf.Interfaces.Single());
                    result.AttachmentOrVifName = attachment.Name;
                    result.ContractBandwidthPoolName = attachment.ContractBandwidthPool.Name;
                    result.DeviceName = attachment.Device.Name;
                    result.RegionName = attachment.Location.SubRegion.Region.Name;
                    result.SubRegionName = attachment.Location.SubRegion.Name;
                    result.PlaneName = attachment.Device.Plane.Name;
                    result.LocationSiteName = attachment.Location.SiteName;
                }
                else if (vrf.InterfaceVlans.Count == 1)
                {
                    var vif = mapper.Map<Vif>(vrf.InterfaceVlans.Single());
                    result.AttachmentOrVifName = vif.Name;
                    result.ContractBandwidthPoolName = vif.ContractBandwidthPool.Name;
                    result.DeviceName = vrf.Device.Name;
                    result.RegionName = vrf.Device.Location.SubRegion.Region.Name;
                    result.SubRegionName = vrf.Device.Location.SubRegion.Name;
                    result.PlaneName = vrf.Device.Plane.Name;
                    result.LocationSiteName = vrf.Device.Location.SiteName;
                }

                result.AttachmentSet = mapper.Map<AttachmentSetViewModel>(source.AttachmentSet);
                result.AttachmentSetID = source.AttachmentSetID;
                result.AttachmentSetVrfID = source.AttachmentSetVrfID;
                result.Preference = source.Preference;
                result.Vrf = mapper.Map<VrfViewModel>(source.Vrf);
                result.VrfID = source.VrfID;

                return result;
            }    
        }
     
    }
}
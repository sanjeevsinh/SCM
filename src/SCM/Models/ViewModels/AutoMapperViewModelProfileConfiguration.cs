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
            CreateMap<AttachmentBandwidth, AttachmentBandwidthViewModel>().ReverseMap();
            CreateMap<Vrf, VrfViewModel>().ReverseMap();
            CreateMap<Vlan, VlanViewModel>().ReverseMap();
            CreateMap<Port, BundleInterfacePortViewModel>().ReverseMap();
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
            CreateMap<Attachment, AttachmentViewModel>()
                .ForMember(dest => dest.Location, conf => conf.MapFrom(src => src.Device.Location))
                .ForMember(dest => dest.Name, conf => conf.ResolveUsing(new AttachmentNameResolver()))
                .ForMember(dest => dest.Plane, conf => conf.MapFrom(src => src.Device.Plane))
                .ForMember(dest => dest.Region, conf => conf.MapFrom(src => src.Device.Location.SubRegion.Region))
                .ForMember(dest => dest.SubRegion, conf => conf.MapFrom(src => src.Device.Location.SubRegion))
                .ReverseMap();
            CreateMap<Vif, VifViewModel>().ReverseMap();
            CreateMap<AttachmentRequestViewModel, AttachmentRequest>();
            CreateMap<VifRequestViewModel, VifRequest>();
            CreateMap<AttachmentSetVrfRequestViewModel, AttachmentSetVrfRequest>();
            CreateMap<RouteTargetRequestViewModel, RouteTargetRequest>();
        }

        public class AttachmentNameResolver : IValueResolver<Attachment, AttachmentViewModel, string>
        {
            public string Resolve(Attachment source, AttachmentViewModel destination, string destMember, ResolutionContext context)
            {
                if (source.IsBundle)
                {
                    return $"Bundle{source.ID}";
                }
                else if (source.IsMultiPort)
                {
                    return $"MultiPort{source.ID}";
                }
                else
                {
                    var port = source.Interfaces.Single().Ports.Single();
                    return $"{port.Type} {port.Name}";
                }
            }
        }

        public class AttachmentSetVrfTypeConverter : ITypeConverter<AttachmentSetVrf, AttachmentSetVrfViewModel>
        {
            public AttachmentSetVrfViewModel Convert(AttachmentSetVrf source, AttachmentSetVrfViewModel destination, ResolutionContext context)
            {
                var mapper = context.Mapper;
                var result = new AttachmentSetVrfViewModel();
                var vrf = source.Vrf;

                result.DeviceName = vrf.Device.Name;
                result.RegionName = vrf.Device.Location.SubRegion.Region.Name;
                result.SubRegionName = vrf.Device.Location.SubRegion.Name;
                result.PlaneName = vrf.Device.Plane.Name;
                result.LocationSiteName = vrf.Device.Location.SiteName;
                result.AttachmentSet = mapper.Map<AttachmentSetViewModel>(source.AttachmentSet);
                result.AttachmentSetID = source.AttachmentSetID;
                result.AttachmentSetVrfID = source.AttachmentSetVrfID;
                result.Preference = source.Preference;
                result.Vrf = mapper.Map<VrfViewModel>(vrf);
                result.VrfID = vrf.VrfID;

                AttachmentViewModel attachment = null;
                VifViewModel vif = null;

                if (vrf.Attachments.Count == 1)
                {
                    attachment = mapper.Map<AttachmentViewModel>(vrf.Attachments.Single());
                }
                else if (vrf.Vifs.Count == 1)
                {
                    vif = mapper.Map<VifViewModel>(vrf.Vifs.Single());
                }

                if (attachment != null) {

                    result.AttachmentOrVifName = attachment.Name;
                    result.ContractBandwidthPoolName = attachment.ContractBandwidthPool.Name;
                }
                else if (vif != null)
                { 
                    result.AttachmentOrVifName = vif.Name;
                    result.ContractBandwidthPoolName = vif.ContractBandwidthPool.Name;
                }

                return result;
            }    
        }
    }
}
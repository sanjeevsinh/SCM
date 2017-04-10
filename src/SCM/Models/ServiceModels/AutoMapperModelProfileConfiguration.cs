using AutoMapper;
using System.Linq;

namespace SCM.Models.ServiceModels
{
    public class AutoMapperServiceModelProfileConfiguration : Profile
    {
        public AutoMapperServiceModelProfileConfiguration()
        {
            CreateMap<Interface, Attachment>()
                .ForMember(dest => dest.Bandwidth, conf => conf.MapFrom(src => src.InterfaceBandwidth))
                .ForMember(dest => dest.BandwidthID, conf => conf.MapFrom(src => src.InterfaceBandwidthID))
                .ForMember(dest => dest.ID, conf => conf.MapFrom(src => src.InterfaceID))
                .ForMember(dest => dest.InterfaceType, conf => conf.MapFrom(src => src.Port != null ? src.Port.Type : null))
                .ForMember(dest => dest.InterfaceName, conf => conf.MapFrom(src => src.Port != null ? src.Port.Name : null))
                .ForMember(dest => dest.Location, conf => conf.MapFrom(src => src.Device.Location))
                .ForMember(dest => dest.LocationID, conf => conf.MapFrom(src => src.Device.LocationID))
                .ForMember(dest => dest.Name, conf => conf.ResolveUsing(new AttachmentInterfaceNameResolver()))
                .ForMember(dest => dest.Plane, conf => conf.MapFrom(src => src.Device.Plane))
                .ForMember(dest => dest.PlaneID, conf => conf.MapFrom(src => src.Device.PlaneID))
                .ForMember(dest => dest.Region, conf => conf.MapFrom(src => src.Device.Location.SubRegion.Region))
                .ForMember(dest => dest.RegionID, conf => conf.MapFrom(src => src.Device.Location.SubRegion.RegionID))
                .ForMember(dest => dest.SubRegion, conf => conf.MapFrom(src => src.Device.Location.SubRegion))
                .ForMember(dest => dest.SubRegionID, conf => conf.MapFrom(src => src.Device.Location.SubRegionID))
                .ForMember(dest => dest.Tenant, conf => conf.MapFrom(src => src.Tenant))
                .ForMember(dest => dest.TenantID, conf => conf.MapFrom(src => src.TenantID))
                .Include<Interface, AttachmentAndVifs>();

            CreateMap<Interface, AttachmentAndVifs>()
                .ForMember(dest => dest.Vifs, conf => conf.MapFrom(src => src.InterfaceVlans));

            CreateMap<MultiPort, Attachment>()
                .ForMember(dest => dest.Bandwidth, conf => conf.MapFrom(src => src.InterfaceBandwidth))
                .ForMember(dest => dest.BandwidthID, conf => conf.MapFrom(src => src.InterfaceBandwidthID))
                .ForMember(dest => dest.ID, conf => conf.MapFrom(src => src.MultiPortID))
                .ForMember(dest => dest.IsMultiPort, conf => conf.UseValue(true))
                .ForMember(dest => dest.Location, conf => conf.MapFrom(src => src.Device.Location))
                .ForMember(dest => dest.LocationID, conf => conf.MapFrom(src => src.Device.LocationID))
                .ForMember(dest => dest.MultiPortMembers, conf => conf.MapFrom(src => src.Ports))
                .ForMember(dest => dest.Name, conf => conf.MapFrom(src => $"MultiPort{src.Identifier}"))
                .ForMember(dest => dest.Plane, conf => conf.MapFrom(src => src.Device.Plane))
                .ForMember(dest => dest.PlaneID, conf => conf.MapFrom(src => src.Device.PlaneID))
                .ForMember(dest => dest.Region, conf => conf.MapFrom(src => src.Device.Location.SubRegion.Region))
                .ForMember(dest => dest.RegionID, conf => conf.MapFrom(src => src.Device.Location.SubRegion.RegionID))
                .ForMember(dest => dest.SubRegion, conf => conf.MapFrom(src => src.Device.Location.SubRegion))
                .ForMember(dest => dest.SubRegionID, conf => conf.MapFrom(src => src.Device.Location.SubRegionID))
                .Include<MultiPort, AttachmentAndVifs>();

            CreateMap<MultiPort, AttachmentAndVifs>()
                .ForMember(dest => dest.Vifs, conf => conf.MapFrom(src => src.MultiPortVlans));

            CreateMap<AttachmentRequest, Interface>()
                .ForMember(dest => dest.IsBundle, conf => conf.MapFrom(src => src.BundleRequired))
                .ForMember(dest => dest.IsMultiPort, conf => conf.MapFrom(src => src.MultiPortRequired));

            CreateMap<AttachmentRequest, MultiPort>();

            CreateMap<AttachmentRequest, Vrf>();

            CreateMap<VifRequest, InterfaceVlan>()
                .ForMember(dest => dest.InterfaceID, conf => conf.MapFrom(src => src.AttachmentID))
                .ForMember(dest => dest.VlanTag, conf => conf.MapFrom(src => src.AllocatedVlanTag));

            CreateMap<InterfaceVlan, Vif>()
                .ForMember(dest => dest.ID, conf => conf.MapFrom(src => src.InterfaceVlanID))
                .ForMember(dest => dest.Name, conf => conf.ResolveUsing(new InterfaceVlanVifNameResolver()))
                .ForMember(dest => dest.Tenant, conf => conf.MapFrom(src => src.Tenant))
                .ForMember(dest => dest.TenantID, conf => conf.MapFrom(src => src.TenantID))
                .ForMember(dest => dest.AttachmentID, conf => conf.MapFrom(src => src.InterfaceID))
                .ForMember(dest => dest.Attachment, conf => conf.MapFrom(src => src.Interface));

            CreateMap<VifRequest, MultiPortVlan>()
                .ForMember(dest => dest.MultiPortID, conf => conf.MapFrom(src => src.AttachmentID))
                .ForMember(dest => dest.VlanTag, conf => conf.MapFrom(src => src.AllocatedVlanTag));

            CreateMap<MultiPortVlan, Vif>()
                .ForMember(dest => dest.ID, conf => conf.MapFrom(src => src.MultiPortVlanID))
                .ForMember(dest => dest.Name, conf => conf.MapFrom(src => $"MultiPort{src.MultiPort.Identifier}.{src.VlanTag}"))
                .ForMember(dest => dest.Tenant, conf => conf.MapFrom(src => src.Tenant))
                .ForMember(dest => dest.TenantID, conf => conf.MapFrom(src => src.TenantID))
                .ForMember(dest => dest.AttachmentID, conf => conf.MapFrom(src => src.MultiPortID))
                .ForMember(dest => dest.Attachment, conf => conf.MapFrom(src => src.MultiPort))
                .ForMember(dest => dest.MultiPortVifs, conf => conf.MapFrom(src => src.InterfaceVlans));

            CreateMap<InterfaceVlan, MultiPortVif>()
                .ForMember(dest => dest.MemberAttachment, conf => conf.MapFrom(src => src.Interface));

            CreateMap<VifRequest, Vrf>();

            CreateMap<RouteTargetRequest, RouteTarget>()
                .ForMember(dest => dest.AssignedNumberSubField, conf => conf.MapFrom(src => src.AllocatedAssignedNumberSubField));

            CreateMap<Attachment, AttachmentOrVif>()
                .ForMember(dest => dest.AttachmentName, conf => conf.MapFrom(src => src.Name))
                .ForMember(dest => dest.AttachmentSetName, conf => conf.MapFrom(src => src.Vrf.AttachmentSetVrfs.Count > 0 ? src.Vrf.AttachmentSetVrfs.First().AttachmentSet.Name : null))
                .ForMember(dest => dest.AttachmentIsBundle, conf => conf.MapFrom(src => src.IsBundle))
                .ForMember(dest => dest.AttachmentIsMultiPort, conf => conf.MapFrom(src => src.IsMultiPort))
                .ForMember(dest => dest.ContractBandwidthPoolName, conf => conf.MapFrom(src => src.ContractBandwidthPool.Name))
                .ForMember(dest => dest.ContractBandwidthValue, conf => conf.MapFrom(src => src.ContractBandwidthPool.ContractBandwidth.BandwidthMbps))
                .ForMember(dest => dest.DeviceName, conf => conf.MapFrom(src => src.Device.Name))
                .ForMember(dest => dest.InterfaceBandwidthValue, conf => conf.MapFrom(src => src.Bandwidth.BandwidthGbps))
                .ForMember(dest => dest.LocationName, conf => conf.MapFrom(src => src.Location.SiteName))
                .ForMember(dest => dest.PlaneName, conf => conf.MapFrom(src => src.Plane.Name))
                .ForMember(dest => dest.TenantName, conf => conf.MapFrom(src => src.Tenant.Name))
                .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vrf.Name));

            CreateMap<Vif, AttachmentOrVif>()
                .ForMember(dest => dest.AttachmentName, conf => conf.MapFrom(src => src.Attachment.Name))
                .ForMember(dest => dest.AttachmentSetName, conf => conf.MapFrom(src => src.Vrf.AttachmentSetVrfs.Count > 0 ? src.Vrf.AttachmentSetVrfs.First().AttachmentSet.Name : null))
                .ForMember(dest => dest.AttachmentIsBundle, conf => conf.MapFrom(src => src.Attachment.IsBundle))
                .ForMember(dest => dest.AttachmentIsMultiPort, conf => conf.MapFrom(src => src.Attachment.IsMultiPort))
                .ForMember(dest => dest.ContractBandwidthPoolName, conf => conf.MapFrom(src => src.ContractBandwidthPool.Name))
                .ForMember(dest => dest.ContractBandwidthValue, conf => conf.MapFrom(src => src.ContractBandwidthPool.ContractBandwidth.BandwidthMbps))
                .ForMember(dest => dest.DeviceName, conf => conf.MapFrom(src => src.Attachment.Device.Name))
                .ForMember(dest => dest.InterfaceBandwidthValue, conf => conf.MapFrom(src => src.Attachment.Bandwidth.BandwidthGbps))
                .ForMember(dest => dest.LocationName, conf => conf.MapFrom(src => src.Attachment.Location.SiteName))
                .ForMember(dest => dest.IsVif, conf => conf.UseValue(true))
                .ForMember(dest => dest.PlaneName, conf => conf.MapFrom(src => src.Attachment.Plane.Name))
                .ForMember(dest => dest.TenantName, conf => conf.MapFrom(src => src.Tenant.Name))
                .ForMember(dest => dest.VifName, conf => conf.MapFrom(src => src.Name))
                .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vrf.Name));
        }

        public class AttachmentInterfaceNameResolver : IValueResolver<Interface, Attachment, string>
        {
            public string Resolve(Interface source, Attachment destination, string destMember, ResolutionContext context)
            {
                if (source.IsBundle)
                {
                    return $"Bundle {source.BundleID.ToString()}";
                }
                else
                {
                    return $"{source.Port.Type} {source.Port.Name}";
                }
            }
        }

        public class InterfaceVlanVifNameResolver : IValueResolver<InterfaceVlan, Vif, string>
        {
            public string Resolve(InterfaceVlan source, Vif destination, string destMember, ResolutionContext context)
            {
                if (source.Interface.IsBundle)
                {
                    return $"Bundle {source.Interface.BundleID.ToString()}.{source.VlanTag}";
                }
                else
                {
                    return $"{source.Interface.Port.Type} {source.Interface.Port.Name}.{source.VlanTag}";
                }
            }
        }
    }
}
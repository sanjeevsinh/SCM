using AutoMapper;

namespace SCM.Models.ServiceModels
{
    public class AutoMapperServiceModelProfileConfiguration : Profile
    {
       public AutoMapperServiceModelProfileConfiguration()
        {
            CreateMap<Interface, Attachment>()
                .ForMember(dest => dest.Bandwidth, conf => conf.MapFrom(src => src.InterfaceBandwidth))
                .ForMember(dest => dest.BandwidthID, conf => conf.MapFrom(src => src.InterfaceBandwidthID))
                .ForMember(dest => dest.Device, conf => conf.MapFrom(src => src.Port.Device))
                .ForMember(dest => dest.DeviceID, conf => conf.MapFrom(src => src.Port.DeviceID))
                .ForMember(dest => dest.Location, conf => conf.MapFrom(src => src.Port.Device.Location))
                .ForMember(dest => dest.LocationID, conf => conf.MapFrom(src => src.Port.Device.LocationID))
                .ForMember(dest => dest.Plane, conf => conf.MapFrom(src => src.Port.Device.Plane))
                .ForMember(dest => dest.PlaneID, conf => conf.MapFrom(src => src.Port.Device.PlaneID))
                .ForMember(dest => dest.Region, conf => conf.MapFrom(src => src.Port.Device.Location.SubRegion.Region))
                .ForMember(dest => dest.RegionID, conf => conf.MapFrom(src => src.Port.Device.Location.SubRegion.RegionID))
                .ForMember(dest => dest.SubRegion, conf => conf.MapFrom(src => src.Port.Device.Location.SubRegion))
                .ForMember(dest => dest.SubRegionID, conf => conf.MapFrom(src => src.Port.Device.Location.SubRegionID))
                .ForMember(dest => dest.Tenant, conf => conf.MapFrom(src => src.Port.Tenant))
                .ForMember(dest => dest.TenantID, conf => conf.MapFrom(src => src.Port.TenantID));

            CreateMap<AttachmentRequest, Interface>()
                .ForMember(dest => dest.InterfaceBandwidthID, conf => conf.MapFrom(src => src.BandwidthID));

            CreateMap<AttachmentRequest, Vrf>()
                .ForMember(dest => dest.AdministratorSubField, conf => conf.MapFrom(src => src.VrfAdministratorSubField))
                .ForMember(dest => dest.AssignedNumberSubField, conf => conf.MapFrom(src => src.VrfAssignedNumberSubField))
                .ForMember(dest => dest.Name, conf => conf.MapFrom(src => src.VrfName));
        }
    }
}
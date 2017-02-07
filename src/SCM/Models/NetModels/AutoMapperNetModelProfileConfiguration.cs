using AutoMapper;
using SCM.Models;

namespace SCM.Models.NetModels
{
    public class AutoMapperNetModelProfileConfiguration : Profile
    {
       public AutoMapperNetModelProfileConfiguration()
        {
            CreateMap<Interface, UntaggedAttachmentInterfaceNetModel>()
                .ForMember(dest => dest.InterfaceType, conf => conf.MapFrom(src => src.Port.Type))
                .ForMember(dest => dest.InterfaceID, conf => conf.MapFrom(src => src.Port.Name))
                .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3))
                .ForMember(dest => dest.InterfaceBandwidth, conf => conf.MapFrom(src => src.InterfaceBandwidth.BandwidthKbps));

            CreateMap<Interface, TaggedAttachmentInterfaceNetModel>()
                .ForMember(dest => dest.InterfaceType, conf => conf.MapFrom(src => src.Port.Type))
                .ForMember(dest => dest.InterfaceID, conf => conf.MapFrom(src => src.Port.Name))
                .ForMember(dest => dest.InterfaceBandwidth, conf => conf.MapFrom(src => src.InterfaceBandwidth.BandwidthKbps));

            CreateMap<Interface, Layer3VrfNetModel>()
                .ForMember(dest => dest.Ipv4Address, conf => conf.MapFrom(src => src.IpAddress))
                .ForMember(dest => dest.Ipv4SubnetMask, conf => conf.MapFrom(src => src.SubnetMask));

            CreateMap<Vrf, Layer3VrfNetModel>()
                .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Name))
                .ForMember(dest => dest.AdministratorSubField, conf => conf.MapFrom(src => src.AdministratorSubField))
                .ForMember(dest => dest.AssignedNumberField, conf => conf.MapFrom(src => src.AssignedNumberSubField))
                .ForMember(dest => dest.EnableBgp, conf => conf.MapFrom(src => src.BgpPeers.Count > 0));
        }
    }
}
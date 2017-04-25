using AutoMapper;
using System.Linq;

namespace SCM.Models.ServiceModels
{
    public class AutoMapperServiceModelProfileConfiguration : Profile
    {
        public AutoMapperServiceModelProfileConfiguration()
        {

            CreateMap<AttachmentRequest, Attachment>()
                .ForMember(dest => dest.IsBundle, conf => conf.MapFrom(src => src.BundleRequired))
                .ForMember(dest => dest.IsMultiPort, conf => conf.MapFrom(src => src.MultiPortRequired));

            CreateMap<AttachmentRequest, Vrf>();

            CreateMap<AttachmentRequest, ContractBandwidthPool>();

            CreateMap<VifRequest, Vif>()
                .ForMember(dest => dest.VlanTag, conf => conf.MapFrom(src => src.AllocatedVlanTag));

            CreateMap<VifRequest, Vrf>();

            CreateMap<VifRequest, ContractBandwidthPool>();

            CreateMap<RouteTargetRequest, RouteTarget>()
                .ForMember(dest => dest.AssignedNumberSubField, conf => conf.MapFrom(src => src.AllocatedAssignedNumberSubField));
        }
    }
}
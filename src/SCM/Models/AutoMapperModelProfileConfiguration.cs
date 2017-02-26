using AutoMapper;
using SCM.Models;

namespace SCM.Models
{
    public class AutoMapperModelProfileConfiguration : Profile
    {
       public AutoMapperModelProfileConfiguration()
        {
            CreateMap<Interface, AttachmentInterface>()
                .ForMember(dest => dest.PortType, conf => conf.MapFrom(src => src.Port.Type));
        }
    }
}
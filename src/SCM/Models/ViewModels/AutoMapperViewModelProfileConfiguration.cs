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
        }
    }
}
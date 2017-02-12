using AutoMapper;
using SCM.Models;
using System.Collections.Generic;
using System;

namespace SCM.Models.NetModels
{
    public class AutoMapperNetModelProfileConfiguration : Profile
    {
        public AutoMapperNetModelProfileConfiguration()
        {
            CreateMap<Port, UntaggedAttachmentInterfaceNetModel>()
                .ForMember(dest => dest.InterfaceType, conf => conf.MapFrom(src => src.Interface.Port.Type))
                .ForMember(dest => dest.InterfaceID, conf => conf.MapFrom(src => src.Interface.Port.Name))
                .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.Interface.IsLayer3))
                .ForMember(dest => dest.InterfaceBandwidth, conf => conf.MapFrom(src => src.Interface.InterfaceBandwidth.BandwidthKbps))
                .ForMember(dest => dest.Layer3, conf => conf.MapFrom(src => src.Interface));

            CreateMap<BundleInterface, UntaggedAttachmentBundleInterfaceNetModel>()
                .ForMember(dest => dest.BundleInterfaceMembers, conf => conf.MapFrom(src => src.BundleInterfacePorts))
                .ForMember(dest => dest.BundleInterfaceID, conf => conf.MapFrom(src => src.Name))
                .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3))
                .ForMember(dest => dest.InterfaceBandwidth, conf => conf.MapFrom(src => src.InterfaceBandwidth.BandwidthKbps))
                .ForMember(dest => dest.Layer3, conf => conf.MapFrom(src => src));

            CreateMap<BundleInterface, TaggedAttachmentBundleInterfaceNetModel>()
                .ForMember(dest => dest.BundleInterfaceMembers, conf => conf.MapFrom(src => src.BundleInterfacePorts))
                .ForMember(dest => dest.BundleInterfaceID, conf => conf.MapFrom(src => src.Name))
                .ForMember(dest => dest.InterfaceBandwidth, conf => conf.MapFrom(src => src.InterfaceBandwidth.BandwidthKbps))
                .ForMember(dest => dest.Vifs, conf => conf.MapFrom(src => src.BundleInterfaceVlans));

            CreateMap<BundleInterfacePort, BundleInterfaceMemberNetModel>()
                .ForMember(dest => dest.InterfaceType, conf => conf.MapFrom(src => src.Port.Type))
                .ForMember(dest => dest.InterfaceID, conf => conf.MapFrom(src => src.Port.Name));

            CreateMap<Port, TaggedAttachmentInterfaceNetModel>()
                .ForMember(dest => dest.InterfaceType, conf => conf.MapFrom(src => src.Interface.Port.Type))
                .ForMember(dest => dest.InterfaceID, conf => conf.MapFrom(src => src.Interface.Port.Name))
                .ForMember(dest => dest.InterfaceBandwidth, conf => conf.MapFrom(src => src.Interface.InterfaceBandwidth.BandwidthKbps))
                .ForMember(dest => dest.Vifs, conf => conf.MapFrom(src => src.Interface.InterfaceVlans));

            CreateMap<Vrf, VrfNetModel>()
                .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Name))
                .ForMember(dest => dest.AdministratorSubField, conf => conf.MapFrom(src => src.AdministratorSubField))
                .ForMember(dest => dest.AssignedNumberSubField, conf => conf.MapFrom(src => src.AssignedNumberSubField));

            CreateMap<InterfaceVlan, VifNetModel>()
                .ForMember(dest => dest.VlanID, conf => conf.MapFrom(src => src.VlanTag))
                .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3))
                .ForMember(dest => dest.Layer3Vrf, conf => conf.MapFrom(src => src));

            CreateMap<BundleInterfaceVlan, VifNetModel>()
                .ForMember(dest => dest.VlanID, conf => conf.MapFrom(src => src.VlanTag))
                .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3))
                .ForMember(dest => dest.Layer3Vrf, conf => conf.MapFrom(src => src));

            CreateMap<BgpPeer, BgpPeerNetModel>()
                .ForMember(dest => dest.PeerIpv4Address, conf => conf.MapFrom(src => src.IpAddress))
                .ForMember(dest => dest.PeerAutonomousSystem, conf => conf.MapFrom(src => src.AutonomousSystem));

            CreateMap<Interface, Layer3NetModel>().ConvertUsing(new InterfaceTypeConverter());

            CreateMap<InterfaceVlan, Layer3NetModel>().ConvertUsing(new InterfaceVlanTypeConverter());

            CreateMap<BundleInterface, Layer3NetModel>().ConvertUsing(new BundleInterfaceTypeConverter());

            CreateMap<BundleInterfaceVlan, Layer3NetModel>().ConvertUsing(new BundleInterfaceVlanTypeConverter());

            CreateMap<Device, PeAttachmentNetModel>().ConvertUsing(new DeviceTypeConverter());
        }

        public class DeviceTypeConverter : ITypeConverter<Device, PeAttachmentNetModel>
        {
            public PeAttachmentNetModel Convert(Device source, PeAttachmentNetModel destination, ResolutionContext context)
            {
                var result = new PeAttachmentNetModel();
                var Mapper = context.Mapper;
                var untaggedAttachmentInterfaces = new List<UntaggedAttachmentInterfaceNetModel>();
                var taggedAttachmentInterfaces = new List<TaggedAttachmentInterfaceNetModel>();
                var untaggedAttachmentBundleInterfaces = new List<UntaggedAttachmentBundleInterfaceNetModel>();
                var taggedAttachmentBundleInterfaces = new List<TaggedAttachmentBundleInterfaceNetModel>();

                result.PEName = source.Name;
                result.Vrfs = Mapper.Map<IList<VrfNetModel>>(source.Vrfs);

                if (source.Ports != null)
                {
                    foreach (Port port in source.Ports)
                    {
                        if (port.Interface != null)
                        {
                            if (port.Interface.IsTagged)
                            {
                                taggedAttachmentInterfaces.Add(Mapper.Map<TaggedAttachmentInterfaceNetModel>(port));
                            }
                            else
                            {
                                untaggedAttachmentInterfaces.Add(Mapper.Map<UntaggedAttachmentInterfaceNetModel>(port));
                            }
                        }
                    }
                }

                if (source.BundleInterfaces != null)
                {

                    foreach (BundleInterface bundleInterface in source.BundleInterfaces)
                    {
                        if (bundleInterface.IsTagged)
                        {
                            taggedAttachmentBundleInterfaces.Add(Mapper.Map<TaggedAttachmentBundleInterfaceNetModel>(bundleInterface));
                        }
                        else
                        {
                            untaggedAttachmentBundleInterfaces.Add(Mapper.Map<UntaggedAttachmentBundleInterfaceNetModel>(bundleInterface));
                        }
                    }
                }

                result.TaggedAttachmentInterfaces = taggedAttachmentInterfaces;
                result.UntaggedAttachmentInterfaces = untaggedAttachmentInterfaces;
                result.TaggedAttachmentBundleInterfaces = taggedAttachmentBundleInterfaces;
                result.UntaggedAttachmentBundleInterfaces = untaggedAttachmentBundleInterfaces;

                return result;
            }
        }

        public class InterfaceTypeConverter : ITypeConverter<Interface, Layer3NetModel>
        {
            public Layer3NetModel Convert(Interface source, Layer3NetModel destination, ResolutionContext context)
            {
                var Mapper = context.Mapper;

                if (source.IsLayer3)
                {
                    var result = new Layer3NetModel();

                    result.VrfName = source.Vrf.Name;
                    result.IpAddress = source.IpAddress;
                    result.IpSubnetMask = source.SubnetMask;

                    return result;
                }

                return null;
            }
        }

        public class BundleInterfaceTypeConverter : ITypeConverter<BundleInterface, Layer3NetModel>
        {
            public Layer3NetModel Convert(BundleInterface source, Layer3NetModel destination, ResolutionContext context)
            {
                var Mapper = context.Mapper;

                if (source.IsLayer3)
                {
                    var result = new Layer3NetModel();

                    result.VrfName = source.Vrf.Name;
                    result.IpAddress = source.IpAddress;
                    result.IpSubnetMask = source.SubnetMask;

                    return result;
                }

                return null;
            }
        }

        public class InterfaceVlanTypeConverter : ITypeConverter<InterfaceVlan, Layer3NetModel>
        {
            public Layer3NetModel Convert(InterfaceVlan source, Layer3NetModel destination, ResolutionContext context)
            {
                var Mapper = context.Mapper;

                if (source.IsLayer3)
                {
                    var result = new Layer3NetModel();

                    result.VrfName = source.Vrf.Name;
                    result.IpAddress = source.IpAddress;
                    result.IpSubnetMask = source.SubnetMask;

                    return result;
                }

                return null;
            }
        }
        public class BundleInterfaceVlanTypeConverter : ITypeConverter<BundleInterfaceVlan, Layer3NetModel>
        {
            public Layer3NetModel Convert(BundleInterfaceVlan source, Layer3NetModel destination, ResolutionContext context)
            {
                var Mapper = context.Mapper;

                if (source.IsLayer3)
                {
                    var result = new Layer3NetModel();

                    result.VrfName = source.Vrf.Name;
                    result.IpAddress = source.IpAddress;
                    result.IpSubnetMask = source.SubnetMask;

                    return result;
                }

                return null;
            }
        }
    }
}
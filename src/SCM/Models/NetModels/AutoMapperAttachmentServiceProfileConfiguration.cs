using AutoMapper;
using SCM.Models;
using System.Collections.Generic;
using System;

namespace SCM.Models.NetModels.Attachment
{
    public class AutoMapperAttachmentServiceProfileConfiguration : Profile
    {
        public AutoMapperAttachmentServiceProfileConfiguration()
        {
            CreateMap<Port, UntaggedAttachmentInterfaceNetModel>()
                .ForMember(dest => dest.InterfaceType, conf => conf.MapFrom(src => src.Interface.Port.Type))
                .ForMember(dest => dest.InterfaceID, conf => conf.MapFrom(src => src.Interface.Port.Name))
                .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.Interface.IsLayer3))
                .ForMember(dest => dest.InterfaceBandwidth, conf => conf.MapFrom(src => src.Interface.InterfaceBandwidth.BandwidthGbps))
                .ForMember(dest => dest.Layer3, conf => conf.MapFrom(src => src.Interface));

            CreateMap<BundleInterface, UntaggedAttachmentBundleInterfaceNetModel>()
                .ForMember(dest => dest.BundleInterfaceMembers, conf => conf.MapFrom(src => src.BundleInterfacePorts))
                .ForMember(dest => dest.BundleInterfaceID, conf => conf.MapFrom(src => src.Name))
                .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3))
                .ForMember(dest => dest.InterfaceBandwidth, conf => conf.MapFrom(src => src.InterfaceBandwidth.BandwidthGbps))
                .ForMember(dest => dest.Layer3, conf => conf.MapFrom(src => src));

            CreateMap<BundleInterface, TaggedAttachmentBundleInterfaceNetModel>()
                .ForMember(dest => dest.BundleInterfaceMembers, conf => conf.MapFrom(src => src.BundleInterfacePorts))
                .ForMember(dest => dest.BundleInterfaceID, conf => conf.MapFrom(src => src.Name))
                .ForMember(dest => dest.InterfaceBandwidth, conf => conf.MapFrom(src => src.InterfaceBandwidth.BandwidthGbps))
                .ForMember(dest => dest.Vifs, conf => conf.MapFrom(src => src.BundleInterfaceVlans));

            CreateMap<BundleInterfacePort, BundleInterfaceMemberNetModel>()
                .ForMember(dest => dest.InterfaceType, conf => conf.MapFrom(src => src.Port.Type))
                .ForMember(dest => dest.InterfaceID, conf => conf.MapFrom(src => src.Port.Name));

            CreateMap<Port, TaggedAttachmentInterfaceNetModel>()
                .ForMember(dest => dest.InterfaceType, conf => conf.MapFrom(src => src.Interface.Port.Type))
                .ForMember(dest => dest.InterfaceID, conf => conf.MapFrom(src => src.Interface.Port.Name))
                .ForMember(dest => dest.InterfaceBandwidth, conf => conf.MapFrom(src => src.Interface.InterfaceBandwidth.BandwidthGbps))
                .ForMember(dest => dest.Vifs, conf => conf.MapFrom(src => src.Interface.InterfaceVlans));

            CreateMap<InterfaceVlan, VifNetModel>()
                .ForMember(dest => dest.VlanID, conf => conf.MapFrom(src => src.VlanTag))
                .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3))
                .ForMember(dest => dest.Layer3Vrf, conf => conf.MapFrom(src => src));

            CreateMap<BundleInterfaceVlan, VifNetModel>()
                .ForMember(dest => dest.VlanID, conf => conf.MapFrom(src => src.VlanTag))
                .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3))
                .ForMember(dest => dest.Layer3Vrf, conf => conf.MapFrom(src => src));

            CreateMap<Interface, Layer3NetModel>()
                .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vrf.Name))
                .ForMember(dest => dest.EnableBgp, conf => conf.MapFrom(src => src.Vrf.BgpPeers.Count > 0))
                .ForMember(dest => dest.BgpPeers, conf => conf.MapFrom(src => src.Vrf.BgpPeers));

            CreateMap<InterfaceVlan, Layer3NetModel>()
                .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vrf.Name))
                .ForMember(dest => dest.EnableBgp, conf => conf.MapFrom(src => src.Vrf.BgpPeers.Count > 0))
                .ForMember(dest => dest.BgpPeers, conf => conf.MapFrom(src => src.Vrf.BgpPeers));

            CreateMap<BundleInterface, Layer3NetModel>()
                .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vrf.Name))
                .ForMember(dest => dest.EnableBgp, conf => conf.MapFrom(src => src.Vrf.BgpPeers.Count > 0))
                .ForMember(dest => dest.BgpPeers, conf => conf.MapFrom(src => src.Vrf.BgpPeers));

            CreateMap<BundleInterfaceVlan, Layer3NetModel>()
                .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vrf.Name))
                .ForMember(dest => dest.EnableBgp, conf => conf.MapFrom(src => src.Vrf.BgpPeers.Count > 0))
                .ForMember(dest => dest.BgpPeers, conf => conf.MapFrom(src => src.Vrf.BgpPeers));

            CreateMap<Vrf, VrfNetModel>()
                .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Name))
                .ForMember(dest => dest.AdministratorSubField, conf => conf.MapFrom(src => src.AdministratorSubField))
                .ForMember(dest => dest.AssignedNumberSubField, conf => conf.MapFrom(src => src.AssignedNumberSubField));

            CreateMap<BgpPeer, BgpPeerNetModel>()
                .ForMember(dest => dest.PeerIpv4Address, conf => conf.MapFrom(src => src.IpAddress))
                .ForMember(dest => dest.PeerAutonomousSystem, conf => conf.MapFrom(src => src.AutonomousSystem));

            CreateMap<Device, AttachmentServiceNetModel>().ConvertUsing(new DeviceTypeConverter());
        }

        public class DeviceTypeConverter : ITypeConverter<Device, AttachmentServiceNetModel>
        {
            public AttachmentServiceNetModel Convert(Device source, AttachmentServiceNetModel destination, ResolutionContext context)
            {
                var result = new AttachmentServiceNetModel();
                var Mapper = context.Mapper;
                var untaggedAttachmentInterfaces = new List<UntaggedAttachmentInterfaceNetModel>();
                var taggedAttachmentInterfaces = new List<TaggedAttachmentInterfaceNetModel>();
                var untaggedAttachmentBundleInterfaces = new List<UntaggedAttachmentBundleInterfaceNetModel>();
                var taggedAttachmentBundleInterfaces = new List<TaggedAttachmentBundleInterfaceNetModel>();

                result.PEName = source.Name;
                result.Vrfs = Mapper.Map<List<VrfNetModel>>(source.Vrfs);

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
    }
}
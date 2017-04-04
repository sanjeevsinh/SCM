using AutoMapper;
using SCM.Models.ServiceModels;
using System.Collections.Generic;
using System;

namespace SCM.Models.NetModels.AttachmentNetModels
{
    public class AutoMapperAttachmentServiceProfileConfiguration : Profile
    {
        public AutoMapperAttachmentServiceProfileConfiguration()
        {
            CreateMap<Interface, UntaggedAttachmentInterfaceNetModel>()
                .ForMember(dest => dest.InterfaceType, conf => conf.MapFrom(src => src.Port.Type))
                .ForMember(dest => dest.InterfaceID, conf => conf.MapFrom(src => src.Port.Name))
                .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3))
                .ForMember(dest => dest.InterfaceBandwidth, conf => conf.MapFrom(src => src.InterfaceBandwidth.BandwidthGbps))
                .ForMember(dest => dest.ContractBandwidth, conf => conf.MapFrom(src => src.ContractBandwidthPool.ContractBandwidth.BandwidthMbps))
                .ForMember(dest => dest.TrustReceivedCosDscp, conf => conf.MapFrom(src => src.ContractBandwidthPool.TrustReceivedCosDscp))
                .ForMember(dest => dest.Layer3, conf => conf.MapFrom(src => src.IsLayer3 ? src : null));

            CreateMap<Interface, TaggedAttachmentInterfaceNetModel>()
                .ForMember(dest => dest.InterfaceType, conf => conf.MapFrom(src => src.Port.Type))
                .ForMember(dest => dest.InterfaceID, conf => conf.MapFrom(src => src.Port.Name))
                .ForMember(dest => dest.InterfaceBandwidth, conf => conf.MapFrom(src => src.InterfaceBandwidth.BandwidthGbps))
                .ForMember(dest => dest.Vifs, conf => conf.MapFrom(src => src.InterfaceVlans));

            CreateMap<Interface, UntaggedAttachmentBundleInterfaceNetModel>()
                .ForMember(dest => dest.BundleInterfaceMembers, conf => conf.MapFrom(src => src.BundleInterfacePorts))
                .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3))
                .ForMember(dest => dest.InterfaceBandwidth, conf => conf.MapFrom(src => src.InterfaceBandwidth.BandwidthGbps))
                .ForMember(dest => dest.ContractBandwidth, conf => conf.MapFrom(src => src.ContractBandwidthPool.ContractBandwidth.BandwidthMbps))
                .ForMember(dest => dest.TrustReceivedCosDscp, conf => conf.MapFrom(src => src.ContractBandwidthPool.TrustReceivedCosDscp))
                .ForMember(dest => dest.Layer3, conf => conf.MapFrom(src => src.IsLayer3 ? src : null));

            CreateMap<Interface, TaggedAttachmentBundleInterfaceNetModel>()
                .ForMember(dest => dest.BundleInterfaceMembers, conf => conf.MapFrom(src => src.BundleInterfacePorts))
                .ForMember(dest => dest.InterfaceBandwidth, conf => conf.MapFrom(src => src.InterfaceBandwidth.BandwidthGbps))
                .ForMember(dest => dest.Vifs, conf => conf.MapFrom(src => src.InterfaceVlans));

            CreateMap<BundleInterfacePort, BundleInterfaceMemberNetModel>()
                .ForMember(dest => dest.InterfaceType, conf => conf.MapFrom(src => src.Port.Type))
                .ForMember(dest => dest.InterfaceID, conf => conf.MapFrom(src => src.Port.Name));

            CreateMap<Port, MultiPortMemberNetModel>()
                .ForMember(dest => dest.InterfaceType, conf => conf.MapFrom(src => src.Type))
                .ForMember(dest => dest.InterfaceID, conf => conf.MapFrom(src => src.Name))
                .ForMember(dest => dest.InterfaceBandwidth, conf => conf.MapFrom(src => src.Interface.InterfaceBandwidth.BandwidthGbps));

            CreateMap<MultiPort, UntaggedAttachmentMultiPortNetModel>()
                .ForMember(dest => dest.Name, conf => conf.MapFrom(src => $"MultiPort{src.Identifier}"))
                .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3))
                .ForMember(dest => dest.ContractBandwidth, conf => conf.MapFrom(src => src.ContractBandwidthPool.ContractBandwidth.BandwidthMbps))
                .ForMember(dest => dest.TrustReceivedCosDscp, conf => conf.MapFrom(src => src.ContractBandwidthPool.TrustReceivedCosDscp));

            CreateMap<MultiPort, TaggedAttachmentMultiPortNetModel>()
                .ForMember(dest => dest.Name, conf => conf.MapFrom(src => $"MultiPort{src.Identifier}"))
                .ForMember(dest => dest.MultiPortMembers, conf => conf.MapFrom(src => src.Ports))
                .ForMember(dest => dest.Vifs, conf => conf.MapFrom(src => src.MultiPortVlans));

            CreateMap<InterfaceVlan, VifNetModel>()
                .ForMember(dest => dest.VlanID, conf => conf.MapFrom(src => src.VlanTag))
                .ForMember(dest => dest.ContractBandwidth, conf => conf.MapFrom(src => src.ContractBandwidthPool.ContractBandwidth.BandwidthMbps))
                .ForMember(dest => dest.TrustReceivedCosDscp, conf => conf.MapFrom(src => src.ContractBandwidthPool.TrustReceivedCosDscp))
                .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3))
                .ForMember(dest => dest.Layer3, conf => conf.MapFrom(src => src.IsLayer3 ? src : null));

            CreateMap<Interface, Layer3NetModel>()
                .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vrf.Name))
                .ForMember(dest => dest.EnableBgp, conf => conf.MapFrom(src => src.Vrf.BgpPeers.Count > 0))
                .ForMember(dest => dest.BgpPeers, conf => conf.MapFrom(src => src.Vrf.BgpPeers));

            CreateMap<InterfaceVlan, Layer3NetModel>()
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

            CreateMap<Attachment, UntaggedAttachmentInterfaceServiceNetModel>()
                .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3))
                .ForMember(dest => dest.InterfaceBandwidth, conf => conf.MapFrom(src => src.Bandwidth.BandwidthGbps))
                .ForMember(dest => dest.ContractBandwidth, conf => conf.MapFrom(src => src.ContractBandwidthPool.ContractBandwidth.BandwidthMbps))
                .ForMember(dest => dest.TrustReceivedCosDscp, conf => conf.MapFrom(src => src.ContractBandwidthPool.TrustReceivedCosDscp))
                .ForMember(dest => dest.InterfaceID, conf => conf.MapFrom(src => src.Port.Name))
                .ForMember(dest => dest.InterfaceType, conf => conf.MapFrom(src => src.Port.Type))
                .ForMember(dest => dest.Layer3, conf => conf.ResolveUsing(new AttachmentLayer3NetModelTypeResolver()));

            CreateMap<Attachment, TaggedAttachmentInterfaceServiceNetModel>()
                .ForMember(dest => dest.InterfaceBandwidth, conf => conf.MapFrom(src => src.Bandwidth.BandwidthGbps))
                .ForMember(dest => dest.InterfaceID, conf => conf.MapFrom(src => src.Port.Name))
                .ForMember(dest => dest.InterfaceType, conf => conf.MapFrom(src => src.Port.Type));

            CreateMap<Attachment, UntaggedAttachmentBundleInterfaceServiceNetModel>()
                .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3))
                .ForMember(dest => dest.InterfaceBandwidth, conf => conf.MapFrom(src => src.Bandwidth.BandwidthGbps))
                .ForMember(dest => dest.ContractBandwidth, conf => conf.MapFrom(src => src.ContractBandwidthPool.ContractBandwidth.BandwidthMbps))
                .ForMember(dest => dest.TrustReceivedCosDscp, conf => conf.MapFrom(src => src.ContractBandwidthPool.TrustReceivedCosDscp))
                .ForMember(dest => dest.BundleInterfaceMembers, conf => conf.MapFrom(src => src.BundleInterfacePorts))
                .ForMember(dest => dest.Layer3, conf => conf.ResolveUsing(new BundleAttachmentLayer3NetModelTypeResolver()));

            CreateMap<Attachment, TaggedAttachmentBundleInterfaceServiceNetModel>()
              .ForMember(dest => dest.InterfaceBandwidth, conf => conf.MapFrom(src => src.Bandwidth.BandwidthGbps))
              .ForMember(dest => dest.BundleInterfaceMembers, conf => conf.MapFrom(src => src.BundleInterfacePorts));

            CreateMap<Attachment, TaggedAttachmentMultiPortServiceNetModel>();

            CreateMap<Attachment, UntaggedAttachmentMultiPortServiceNetModel>()
               .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3))
               .ForMember(dest => dest.ContractBandwidth, conf => conf.MapFrom(src => src.ContractBandwidthPool.ContractBandwidth.BandwidthMbps))
               .ForMember(dest => dest.TrustReceivedCosDscp, conf => conf.MapFrom(src => src.ContractBandwidthPool.TrustReceivedCosDscp));

            CreateMap<Attachment, VrfNetModel>()
                .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vrf.Name))
                .ForMember(dest => dest.AdministratorSubField, conf => conf.MapFrom(src => src.Vrf.AdministratorSubField))
                .ForMember(dest => dest.AssignedNumberSubField, conf => conf.MapFrom(src => src.Vrf.AssignedNumberSubField));

            CreateMap<Attachment, VrfServiceNetModel>()
                .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vrf.Name))
                .ForMember(dest => dest.AdministratorSubField, conf => conf.MapFrom(src => src.Vrf.AdministratorSubField))
                .ForMember(dest => dest.AssignedNumberSubField, conf => conf.MapFrom(src => src.Vrf.AssignedNumberSubField));

            CreateMap<Vif, VifNetModel>()
               .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3))
               .ForMember(dest => dest.VlanID, conf => conf.MapFrom(src => src.VlanTag))
               .ForMember(dest => dest.ContractBandwidth, conf => conf.MapFrom(src => src.ContractBandwidthPool.ContractBandwidth.BandwidthMbps))
               .ForMember(dest => dest.Layer3, conf => conf.ResolveUsing(new VifLayer3NetModelTypeResolver()));

            CreateMap<Vif, VifServiceNetModel>()
               .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3))
               .ForMember(dest => dest.ContractBandwidth, conf => conf.MapFrom(src => src.ContractBandwidthPool.ContractBandwidth.BandwidthMbps))
               .ForMember(dest => dest.VlanID, conf => conf.MapFrom(src => src.VlanTag))
               .ForMember(dest => dest.Layer3, conf => conf.ResolveUsing(new VifLayer3NetModelTypeResolver()));

            CreateMap<Vif, VrfNetModel>()
              .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vrf.Name))
              .ForMember(dest => dest.AdministratorSubField, conf => conf.MapFrom(src => src.Vrf.AdministratorSubField))
              .ForMember(dest => dest.AssignedNumberSubField, conf => conf.MapFrom(src => src.Vrf.AssignedNumberSubField));

            CreateMap<Vif, VrfServiceNetModel>()
              .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vrf.Name))
              .ForMember(dest => dest.AdministratorSubField, conf => conf.MapFrom(src => src.Vrf.AdministratorSubField))
              .ForMember(dest => dest.AssignedNumberSubField, conf => conf.MapFrom(src => src.Vrf.AssignedNumberSubField));
        }

        public class AttachmentLayer3NetModelTypeResolver : IValueResolver<Attachment, UntaggedAttachmentInterfaceServiceNetModel, Layer3NetModel>
        {
            public Layer3NetModel Resolve(Attachment source, UntaggedAttachmentInterfaceServiceNetModel destination, Layer3NetModel destMember, ResolutionContext context)
            {
                var layer3NetModelResolver = new AttachmentLayer3NetModelResolver();
                return layer3NetModelResolver.Create(source, context);
            }
        }

        public class BundleAttachmentLayer3NetModelTypeResolver : IValueResolver<Attachment, UntaggedAttachmentBundleInterfaceServiceNetModel, Layer3NetModel>
        {
            public Layer3NetModel Resolve(Attachment source, UntaggedAttachmentBundleInterfaceServiceNetModel destination, Layer3NetModel destMember, ResolutionContext context)
            {
                var layer3NetModelResolver = new AttachmentLayer3NetModelResolver();
                return layer3NetModelResolver.Create(source, context);
            }
        }

        public class DeviceTypeConverter : ITypeConverter<Device, AttachmentServiceNetModel>
        {
            public AttachmentServiceNetModel Convert(Device source, AttachmentServiceNetModel destination, ResolutionContext context)
            {
                var result = new AttachmentServiceNetModel();
                var mapper = context.Mapper;
                var untaggedAttachmentInterfaces = new List<UntaggedAttachmentInterfaceNetModel>();
                var taggedAttachmentInterfaces = new List<TaggedAttachmentInterfaceNetModel>();
                var untaggedAttachmentBundleInterfaces = new List<UntaggedAttachmentBundleInterfaceNetModel>();
                var taggedAttachmentBundleInterfaces = new List<TaggedAttachmentBundleInterfaceNetModel>();
                var untaggedAttachmentMultiPorts = new List<UntaggedAttachmentMultiPortNetModel>();
                var taggedAttachmentMultiPorts = new List<TaggedAttachmentMultiPortNetModel>();

                result.PEName = source.Name;
                result.Vrfs = mapper.Map<List<VrfNetModel>>(source.Vrfs);

                if (source.Interfaces != null)
                {
                    foreach (Interface iface in source.Interfaces)
                    {
                        if (iface.IsBundle)
                        {
                            if (iface.IsTagged)
                            {
                                taggedAttachmentBundleInterfaces.Add(mapper.Map<TaggedAttachmentBundleInterfaceNetModel>(iface));
                            }
                            else
                            {
                                untaggedAttachmentBundleInterfaces.Add(mapper.Map<UntaggedAttachmentBundleInterfaceNetModel>(iface));
                            }
                        }
                        else
                        {
                            if (iface.IsTagged)
                            {
                                taggedAttachmentInterfaces.Add(mapper.Map<TaggedAttachmentInterfaceNetModel>(iface));
                            }
                            else
                            {
                                untaggedAttachmentInterfaces.Add(mapper.Map<UntaggedAttachmentInterfaceNetModel>(iface));
                            }
                        }
                    }
                }

                if (source.MultiPorts != null)
                {
                    foreach (MultiPort multiPort in source.MultiPorts)
                    {
                        if (multiPort.IsTagged)
                        {
                            taggedAttachmentMultiPorts.Add(mapper.Map<TaggedAttachmentMultiPortNetModel>(multiPort));
                        }
                        else
                        {
                            untaggedAttachmentMultiPorts.Add(mapper.Map<UntaggedAttachmentMultiPortNetModel>(multiPort));
                        }
                    }
                }

                result.TaggedAttachmentInterfaces = taggedAttachmentInterfaces;
                result.UntaggedAttachmentInterfaces = untaggedAttachmentInterfaces;
                result.TaggedAttachmentBundleInterfaces = taggedAttachmentBundleInterfaces;
                result.UntaggedAttachmentBundleInterfaces = untaggedAttachmentBundleInterfaces;
                result.TaggedAttachmentMultiPorts = taggedAttachmentMultiPorts;
                result.UntaggedAttachmentMultiPorts = untaggedAttachmentMultiPorts;

                return result;
            }
        }


        public class AttachmentLayer3NetModelResolver
        {
            public Layer3NetModel Create(Attachment source, ResolutionContext context)
            {
                var result = new Layer3NetModel();
                var mapper = context.Mapper;

                if (source.IsLayer3)
                {
                    result.EnableBgp = source.Vrf.BgpPeers.Count > 0;
                    result.BgpPeers = mapper.Map<List<BgpPeerNetModel>>(source.Vrf.BgpPeers);
                    result.IpAddress = source.IpAddress;
                    result.SubnetMask = source.SubnetMask;
                    result.VrfName = source.Vrf.Name;

                    return result;
                }
                else
                {
                    return null;
                }
            }
        }
        public class VifLayer3NetModelTypeResolver : IValueResolver<Vif, VifNetModel, Layer3NetModel>
        {
            public Layer3NetModel Resolve(Vif source, VifNetModel destination, Layer3NetModel destMember, ResolutionContext context)
            {
                var result = new Layer3NetModel();
                var mapper = context.Mapper;

                if (source.IsLayer3)
                {
                    result.EnableBgp = source.Vrf.BgpPeers.Count > 0;
                    result.BgpPeers = mapper.Map<List<BgpPeerNetModel>>(source.Vrf.BgpPeers);
                    result.IpAddress = source.IpAddress;
                    result.SubnetMask = source.SubnetMask;
                    result.VrfName = source.Vrf.Name;

                    return result;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
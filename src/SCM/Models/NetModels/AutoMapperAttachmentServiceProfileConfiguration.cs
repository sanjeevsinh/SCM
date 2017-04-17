using AutoMapper;
using SCM.Models.ServiceModels;
using System.Collections.Generic;
using System;
using System.Linq;

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
                .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vrf.Name))
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
                .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vrf.Name))
                .ForMember(dest => dest.Layer3, conf => conf.MapFrom(src => src.IsLayer3 ? src : null));

            CreateMap<Interface, TaggedAttachmentBundleInterfaceNetModel>()
                .ForMember(dest => dest.BundleInterfaceMembers, conf => conf.MapFrom(src => src.BundleInterfacePorts))
                .ForMember(dest => dest.InterfaceBandwidth, conf => conf.MapFrom(src => src.InterfaceBandwidth.BandwidthGbps))
                .ForMember(dest => dest.Vifs, conf => conf.MapFrom(src => src.InterfaceVlans));

            CreateMap<BundleInterfacePort, BundleInterfaceMemberNetModel>()
                .ForMember(dest => dest.InterfaceType, conf => conf.MapFrom(src => src.Port.Type))
                .ForMember(dest => dest.InterfaceID, conf => conf.MapFrom(src => src.Port.Name));

            CreateMap<Port, UntaggedMultiPortMemberNetModel>()
                .ForMember(dest => dest.InterfaceType, conf => conf.MapFrom(src => src.Type))
                .ForMember(dest => dest.InterfaceID, conf => conf.MapFrom(src => src.Name))
                .ForMember(dest => dest.InterfaceBandwidth, conf => conf.MapFrom(src => src.Interface.InterfaceBandwidth.BandwidthGbps))
                .ForMember(dest => dest.ContractBandwidth, conf => conf.MapFrom(src => src.Interface.ContractBandwidthPool.ContractBandwidth.BandwidthMbps))
                .ForMember(dest => dest.TrustReceivedCosDscp, conf => conf.MapFrom(src => src.Interface.ContractBandwidthPool.TrustReceivedCosDscp))
                .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Interface.Vrf.Name))
                .ForMember(dest => dest.Layer3, conf => conf.MapFrom(src => src.Interface.IsLayer3 ? src.Interface : null));

            CreateMap<Port, TaggedMultiPortMemberNetModel>()
                .ForMember(dest => dest.InterfaceType, conf => conf.MapFrom(src => src.Type))
                .ForMember(dest => dest.InterfaceID, conf => conf.MapFrom(src => src.Name))
                .ForMember(dest => dest.InterfaceBandwidth, conf => conf.MapFrom(src => src.Interface.InterfaceBandwidth.BandwidthGbps))
                .ForMember(dest => dest.Vifs, conf => conf.MapFrom(src => src.Interface.InterfaceVlans));

            CreateMap<MultiPort, UntaggedAttachmentMultiPortNetModel>()
                .ForMember(dest => dest.Name, conf => conf.MapFrom(src => $"MultiPort{src.Identifier}"))
                .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3))
                .ForMember(dest => dest.MultiPortMembers, conf => conf.MapFrom(src => src.Ports));

            CreateMap<MultiPort, TaggedAttachmentMultiPortNetModel>()
                .ForMember(dest => dest.Name, conf => conf.MapFrom(src => $"MultiPort{src.Identifier}"))
                .ForMember(dest => dest.MultiPortMembers, conf => conf.MapFrom(src => src.Ports));

            CreateMap<InterfaceVlan, VifNetModel>()
                .ForMember(dest => dest.VlanID, conf => conf.MapFrom(src => src.VlanTag))
                .ForMember(dest => dest.ContractBandwidth, conf => conf.MapFrom(src => src.ContractBandwidthPool.ContractBandwidth.BandwidthMbps))
                .ForMember(dest => dest.TrustReceivedCosDscp, conf => conf.MapFrom(src => src.ContractBandwidthPool.TrustReceivedCosDscp))
                .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3))
                .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vrf.Name))
                .ForMember(dest => dest.Layer3, conf => conf.MapFrom(src => src.IsLayer3 ? src : null))
                .Include<InterfaceVlan, VifServiceNetModel>();

            CreateMap<InterfaceVlan, VifServiceNetModel>();

            CreateMap<Interface, Layer3NetModel>()
                .ForMember(dest => dest.BgpPeers, conf => conf.MapFrom(src => src.Vrf.BgpPeers));

            CreateMap<AttachmentAndVifs, Layer3NetModel>()
                .ForMember(dest => dest.BgpPeers, conf => conf.MapFrom(src => src.Vrf.BgpPeers));

            CreateMap<Vif, Layer3NetModel>()
                .ForMember(dest => dest.BgpPeers, conf => conf.MapFrom(src => src.Vrf.BgpPeers));

            CreateMap<InterfaceVlan, Layer3NetModel>()
                .ForMember(dest => dest.BgpPeers, conf => conf.MapFrom(src => src.Vrf.BgpPeers));

            CreateMap<Vrf, VrfNetModel>()
                .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Name))
                .ForMember(dest => dest.AdministratorSubField, conf => conf.MapFrom(src => src.AdministratorSubField))
                .ForMember(dest => dest.AssignedNumberSubField, conf => conf.MapFrom(src => src.AssignedNumberSubField));

            CreateMap<BgpPeer, BgpPeerNetModel>()
                .ForMember(dest => dest.PeerIpv4Address, conf => conf.MapFrom(src => src.IpAddress))
                .ForMember(dest => dest.PeerAutonomousSystem, conf => conf.MapFrom(src => src.AutonomousSystem));

            CreateMap<AttachmentAndVifs, UntaggedAttachmentInterfaceNetModel>()
                .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3))
                .ForMember(dest => dest.InterfaceBandwidth, conf => conf.MapFrom(src => src.Bandwidth.BandwidthGbps))
                .ForMember(dest => dest.ContractBandwidth, conf => conf.MapFrom(src => src.ContractBandwidthPool.ContractBandwidth.BandwidthMbps))
                .ForMember(dest => dest.TrustReceivedCosDscp, conf => conf.MapFrom(src => src.ContractBandwidthPool.TrustReceivedCosDscp))
                .ForMember(dest => dest.InterfaceID, conf => conf.MapFrom(src => src.Port.Name))
                .ForMember(dest => dest.InterfaceType, conf => conf.MapFrom(src => src.Port.Type))
                .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vrf.Name))
                .ForMember(dest => dest.Layer3, conf => conf.MapFrom(src => src.IsLayer3 ? src : null))
                .Include<AttachmentAndVifs, UntaggedAttachmentInterfaceServiceNetModel>();

            CreateMap<AttachmentAndVifs, UntaggedAttachmentInterfaceServiceNetModel>();

            CreateMap<Attachment, TaggedAttachmentInterfaceNetModel>()
                .ForMember(dest => dest.InterfaceBandwidth, conf => conf.MapFrom(src => src.Bandwidth.BandwidthGbps))
                .ForMember(dest => dest.InterfaceID, conf => conf.MapFrom(src => src.Port.Name))
                .ForMember(dest => dest.InterfaceType, conf => conf.MapFrom(src => src.Port.Type))
                .Include<Attachment, TaggedAttachmentInterfaceServiceNetModel>()
                .Include<AttachmentAndVifs, TaggedAttachmentInterfaceNetModel>()
                .Include<AttachmentAndVifs, TaggedAttachmentInterfaceServiceNetModel>();

            CreateMap<Attachment, TaggedAttachmentInterfaceServiceNetModel>();
            CreateMap<AttachmentAndVifs, TaggedAttachmentInterfaceNetModel>();
            CreateMap<AttachmentAndVifs, TaggedAttachmentInterfaceServiceNetModel>();

            CreateMap<AttachmentAndVifs, UntaggedAttachmentBundleInterfaceNetModel>()
                .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3))
                .ForMember(dest => dest.InterfaceBandwidth, conf => conf.MapFrom(src => src.Bandwidth.BandwidthGbps))
                .ForMember(dest => dest.ContractBandwidth, conf => conf.MapFrom(src => src.ContractBandwidthPool.ContractBandwidth.BandwidthMbps))
                .ForMember(dest => dest.TrustReceivedCosDscp, conf => conf.MapFrom(src => src.ContractBandwidthPool.TrustReceivedCosDscp))
                .ForMember(dest => dest.BundleInterfaceMembers, conf => conf.MapFrom(src => src.BundleInterfacePorts))
                .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vrf.Name))
                .ForMember(dest => dest.Layer3, conf => conf.MapFrom(src => src.IsLayer3 ? src : null))
                .Include<AttachmentAndVifs, UntaggedAttachmentBundleInterfaceServiceNetModel>();

            CreateMap<AttachmentAndVifs, UntaggedAttachmentBundleInterfaceServiceNetModel>();

            CreateMap<Attachment, TaggedAttachmentBundleInterfaceNetModel>()
              .ForMember(dest => dest.InterfaceBandwidth, conf => conf.MapFrom(src => src.Bandwidth.BandwidthGbps))
              .ForMember(dest => dest.BundleInterfaceMembers, conf => conf.MapFrom(src => src.BundleInterfacePorts))
              .Include<Attachment, TaggedAttachmentBundleInterfaceServiceNetModel>()
              .Include<AttachmentAndVifs, TaggedAttachmentBundleInterfaceNetModel>()
              .Include<AttachmentAndVifs, TaggedAttachmentBundleInterfaceServiceNetModel>();

            CreateMap<Attachment, TaggedAttachmentBundleInterfaceServiceNetModel>();
            CreateMap<AttachmentAndVifs, TaggedAttachmentBundleInterfaceNetModel>();
            CreateMap<AttachmentAndVifs, TaggedAttachmentBundleInterfaceServiceNetModel>();

            CreateMap<AttachmentAndVifs, UntaggedAttachmentMultiPortNetModel>()
               .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3))
               .Include<AttachmentAndVifs, UntaggedAttachmentMultiPortServiceNetModel>();

            CreateMap<AttachmentAndVifs, UntaggedAttachmentMultiPortServiceNetModel>();

            CreateMap<Attachment, TaggedAttachmentMultiPortNetModel>();
            CreateMap<Attachment, TaggedAttachmentMultiPortServiceNetModel>();
            CreateMap<AttachmentAndVifs, TaggedAttachmentMultiPortNetModel>();
            CreateMap<AttachmentAndVifs, TaggedAttachmentMultiPortServiceNetModel>();

            CreateMap<AttachmentAndVifs, VrfNetModel>()
                .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vrf.Name))
                .ForMember(dest => dest.AdministratorSubField, conf => conf.MapFrom(src => src.Vrf.AdministratorSubField))
                .ForMember(dest => dest.AssignedNumberSubField, conf => conf.MapFrom(src => src.Vrf.AssignedNumberSubField))
                .Include<AttachmentAndVifs, VrfServiceNetModel>();

            CreateMap<AttachmentAndVifs, VrfServiceNetModel>();

            CreateMap<Vif, VifNetModel>()
               .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3))
               .ForMember(dest => dest.VlanID, conf => conf.MapFrom(src => src.VlanTag))
               .ForMember(dest => dest.ContractBandwidth, conf => conf.MapFrom(src => src.ContractBandwidthPool.ContractBandwidth.BandwidthMbps))
               .ForMember(dest => dest.TrustReceivedCosDscp, conf => conf.MapFrom(src => src.ContractBandwidthPool.TrustReceivedCosDscp))
               .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vrf.Name))
               .ForMember(dest => dest.Layer3, conf => conf.MapFrom(src => src.IsLayer3 ? src : null))
               .Include<Vif, VifServiceNetModel>();

            CreateMap<Vif, VifServiceNetModel>();

            CreateMap<Vif, VrfNetModel>()
              .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vrf.Name))
              .ForMember(dest => dest.AdministratorSubField, conf => conf.MapFrom(src => src.Vrf.AdministratorSubField))
              .ForMember(dest => dest.AssignedNumberSubField, conf => conf.MapFrom(src => src.Vrf.AssignedNumberSubField))
              .Include<Vif, VrfServiceNetModel>();

            CreateMap<Vif, VrfServiceNetModel>();

            CreateMap<Device, AttachmentServiceNetModel>().ConvertUsing(new DeviceTypeConverter());

            CreateMap<AttachmentAndVifs, AttachmentServiceNetModel>().ConvertUsing(new AttachmentTypeConverter());

            CreateMap<Vif, AttachmentServiceNetModel>().ConvertUsing(new VifTypeConverter());
        }

        public class DeviceTypeConverter : ITypeConverter<Device, AttachmentServiceNetModel>
        {
            public AttachmentServiceNetModel Convert(Device source, AttachmentServiceNetModel destination, ResolutionContext context)
            {
                var result = new AttachmentServiceNetModel();
                var mapper = context.Mapper;

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

                                result.TaggedAttachmentBundleInterfaces.Add(mapper.Map<TaggedAttachmentBundleInterfaceNetModel>(iface));
                            }
                            else
                            {
                                result.UntaggedAttachmentBundleInterfaces.Add(mapper.Map<UntaggedAttachmentBundleInterfaceNetModel>(iface));
                            }
                        }
                        else if (!iface.IsMultiPort)
                        {
                            if (iface.IsTagged)
                            {
                                result.TaggedAttachmentInterfaces.Add(mapper.Map<TaggedAttachmentInterfaceNetModel>(iface));
                            }
                            else
                            {
                                result.UntaggedAttachmentInterfaces.Add(mapper.Map<UntaggedAttachmentInterfaceNetModel>(iface));
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
                            result.TaggedAttachmentMultiPorts.Add(mapper.Map<TaggedAttachmentMultiPortNetModel>(multiPort));
                        }
                        else
                        {
                            result.UntaggedAttachmentMultiPorts.Add(mapper.Map<UntaggedAttachmentMultiPortNetModel>(multiPort));
                        }
                    }
                }

                return result;
            }
        }
        public class AttachmentTypeConverter : ITypeConverter<AttachmentAndVifs, AttachmentServiceNetModel>
        {
            public AttachmentServiceNetModel Convert(AttachmentAndVifs source, AttachmentServiceNetModel destination, ResolutionContext context)
            {
                var result = new AttachmentServiceNetModel();
                var mapper = context.Mapper;

                result.PEName = source.Device.Name;
                var vrfs = new List<Vrf>();

                if (source.IsBundle)
                {
                    if (source.IsTagged)
                    {

                        result.TaggedAttachmentBundleInterfaces.Add(mapper.Map<TaggedAttachmentBundleInterfaceNetModel>(source));
                        result.Vrfs.AddRange(mapper.Map<List<VrfNetModel>>(source.Vifs.Select(q => q.Vrf)));
                    }
                    else
                    {
                        result.UntaggedAttachmentBundleInterfaces.Add(mapper.Map<UntaggedAttachmentBundleInterfaceNetModel>(source));
                        result.Vrfs.Add(mapper.Map<VrfNetModel>(source.Vrf));
                    }
                }
                else if (source.IsMultiPort)
                {
                    if (source.IsTagged)
                    {
                        result.TaggedAttachmentMultiPorts.Add(mapper.Map<TaggedAttachmentMultiPortNetModel>(source));
                        result.Vrfs.AddRange(mapper.Map<List<VrfNetModel>>(source.Vifs.Select(q => q.Vrf)));
                    }
                    else
                    {
                        result.UntaggedAttachmentMultiPorts.Add(mapper.Map<UntaggedAttachmentMultiPortNetModel>(source));
                        result.Vrfs.Add(mapper.Map<VrfNetModel>(source.Vrf));
                    }
                }
                else
                {
                    if (source.IsTagged)
                    {
                        result.TaggedAttachmentInterfaces.Add(mapper.Map<TaggedAttachmentInterfaceNetModel>(source));
                        result.Vrfs.AddRange(mapper.Map<List<VrfNetModel>>(source.Vifs.Select(q => q.Vrf)));
                    }
                    else
                    {
                        result.UntaggedAttachmentInterfaces.Add(mapper.Map<UntaggedAttachmentInterfaceNetModel>(source));
                        result.Vrfs.Add(mapper.Map<VrfNetModel>(source.Vrf));
                    }
                }               
                return result;
            }
        }

        public class VifTypeConverter : ITypeConverter<Vif, AttachmentServiceNetModel>
        {
            public AttachmentServiceNetModel Convert(Vif source, AttachmentServiceNetModel destination, ResolutionContext context)
            {
                var result = new AttachmentServiceNetModel();
                var mapper = context.Mapper;

                result.PEName = source.Vrf.Device.Name;

                if (source.Attachment.IsBundle)
                {
                    if (source.Attachment.IsTagged)
                    {
                        var data = mapper.Map<TaggedAttachmentBundleInterfaceNetModel>(source.Attachment);
                        var vif = mapper.Map<VifNetModel>(source);
                        data.Vifs.Add(vif);
                        result.TaggedAttachmentBundleInterfaces.Add(data);
                        result.Vrfs.Add(mapper.Map<VrfNetModel>(source.Vrf));
                    }
                }
                else if (source.Attachment.IsMultiPort)
                {
                    if (source.Attachment.IsTagged)
                    {
                        var data = mapper.Map<TaggedAttachmentMultiPortNetModel>(source.Attachment);
                        foreach (var member in data.MultiPortMembers)
                        {
                            member.Vifs = member.Vifs.Where(q => q.VlanID == source.VlanTag).ToList();
                        }
                        result.TaggedAttachmentMultiPorts.Add(data);
                        result.Vrfs.Add(mapper.Map<VrfNetModel>(source.Vrf));
                    }
                }
                else
                {
                    if (source.Attachment.IsTagged)
                    {
                        var data = mapper.Map<TaggedAttachmentInterfaceNetModel>(source.Attachment);
                        var vif = mapper.Map<VifNetModel>(source);
                        data.Vifs.Add(vif);
                        result.TaggedAttachmentInterfaces.Add(data);
                        result.Vrfs.Add(mapper.Map<VrfNetModel>(source.Vrf));
                    }
                }

                return result;
            }
        }
    }
}
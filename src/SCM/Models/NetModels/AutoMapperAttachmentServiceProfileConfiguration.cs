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
                .ForMember(dest => dest.ContractBandwdithPool, conf => conf.MapFrom(src => src.ContractBandwidthPool))
                .ForMember(dest => dest.PolicyBandwidth, conf => conf.ResolveUsing(new UntaggedAttachmentInterfacePolicyBandwidthResolver()))
                .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vrf.Name))
                .ForMember(dest => dest.Layer3, conf => conf.MapFrom(src => src.IsLayer3 ? src : null));

            CreateMap<Interface, TaggedAttachmentInterfaceNetModel>()
                .ForMember(dest => dest.InterfaceType, conf => conf.MapFrom(src => src.Port.Type))
                .ForMember(dest => dest.InterfaceID, conf => conf.MapFrom(src => src.Port.Name))
                .ForMember(dest => dest.InterfaceBandwidth, conf => conf.MapFrom(src => src.InterfaceBandwidth.BandwidthGbps))
                .ForMember(dest => dest.Vifs, conf => conf.MapFrom(src => src.InterfaceVlans))
                .ForMember(dest => dest.ContractBandwidthPools, conf => conf.MapFrom(src => src.InterfaceVlans.Select(q => q.ContractBandwidthPool).Distinct()));

            CreateMap<Interface, UntaggedAttachmentBundleInterfaceNetModel>()
                .ForMember(dest => dest.BundleInterfaceMembers, conf => conf.MapFrom(src => src.BundleInterfacePorts))
                .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3))
                .ForMember(dest => dest.InterfaceBandwidth, conf => conf.MapFrom(src => src.InterfaceBandwidth.BandwidthGbps))
                .ForMember(dest => dest.ContractBandwdithPool, conf => conf.MapFrom(src => src.ContractBandwidthPool))
                .ForMember(dest => dest.PolicyBandwidth, conf => conf.ResolveUsing(new UntaggedAttachmentBundleInterfacePolicyBandwidthResolver()))
                .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vrf.Name))
                .ForMember(dest => dest.Layer3, conf => conf.MapFrom(src => src.IsLayer3 ? src : null));

            CreateMap<Interface, TaggedAttachmentBundleInterfaceNetModel>()
                .ForMember(dest => dest.BundleInterfaceMembers, conf => conf.MapFrom(src => src.BundleInterfacePorts))
                .ForMember(dest => dest.InterfaceBandwidth, conf => conf.MapFrom(src => src.InterfaceBandwidth.BandwidthGbps))
                .ForMember(dest => dest.Vifs, conf => conf.MapFrom(src => src.InterfaceVlans))
                .ForMember(dest => dest.ContractBandwidthPools, conf => conf.MapFrom(src => src.InterfaceVlans.Select(q => q.ContractBandwidthPool).Distinct()));

            CreateMap<BundleInterfacePort, BundleInterfaceMemberNetModel>()
                .ForMember(dest => dest.InterfaceType, conf => conf.MapFrom(src => src.Port.Type))
                .ForMember(dest => dest.InterfaceID, conf => conf.MapFrom(src => src.Port.Name));

            CreateMap<Port, UntaggedMultiPortMemberNetModel>()
                .ForMember(dest => dest.InterfaceType, conf => conf.MapFrom(src => src.Type))
                .ForMember(dest => dest.InterfaceID, conf => conf.MapFrom(src => src.Name))
                .ForMember(dest => dest.InterfaceBandwidth, conf => conf.MapFrom(src => src.Interface.InterfaceBandwidth.BandwidthGbps))
                .ForMember(dest => dest.PolicyBandwidth, conf => conf.ResolveUsing(new UntaggedMultiPortMemberPolicyBandwidthResolver()))
                .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Interface.Vrf.Name))
                .ForMember(dest => dest.Layer3, conf => conf.MapFrom(src => src.Interface.IsLayer3 ? src.Interface : null));

            CreateMap<Port, TaggedMultiPortMemberNetModel>()
                .ForMember(dest => dest.InterfaceType, conf => conf.MapFrom(src => src.Type))
                .ForMember(dest => dest.InterfaceID, conf => conf.MapFrom(src => src.Name))
                .ForMember(dest => dest.InterfaceBandwidth, conf => conf.MapFrom(src => src.Interface.InterfaceBandwidth.BandwidthGbps))
                .ForMember(dest => dest.PolicyBandwidths, conf => conf.ResolveUsing(new TaggedMultiPortMemberPolicyBandwidthResolver()))
                .ForMember(dest => dest.Vifs, conf => conf.MapFrom(src => src.Interface.InterfaceVlans));

            CreateMap<MultiPort, UntaggedAttachmentMultiPortNetModel>()
                .ForMember(dest => dest.Name, conf => conf.MapFrom(src => $"MultiPort{src.Identifier}"))
                .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3))
                .ForMember(dest => dest.MultiPortMembers, conf => conf.MapFrom(src => src.Ports));

            CreateMap<MultiPort, TaggedAttachmentMultiPortNetModel>()
                .ForMember(dest => dest.Name, conf => conf.MapFrom(src => $"MultiPort{src.Identifier}"))
                .ForMember(dest => dest.MultiPortMembers, conf => conf.MapFrom(src => src.Ports))
                .ForMember(dest => dest.ContractBandwidthPools, conf => conf.MapFrom(src => src.MultiPortVlans.Select(q => q.ContractBandwidthPool).Distinct()));

            CreateMap<InterfaceVlan, AttachmentVifNetModel>()
                .ForMember(dest => dest.VlanID, conf => conf.MapFrom(src => src.VlanTag))
                .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3))
                .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vrf.Name))
                .ForMember(dest => dest.Layer3, conf => conf.MapFrom(src => src.IsLayer3 ? src : null))
                .Include<InterfaceVlan, AttachmentVifServiceNetModel>();

            CreateMap<InterfaceVlan, AttachmentVifServiceNetModel>();

            CreateMap<InterfaceVlan, MultiPortVifNetModel>()
                .ForMember(dest => dest.VlanID, conf => conf.MapFrom(src => src.VlanTag))
                .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3))
                .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vrf.Name))
                .ForMember(dest => dest.Layer3, conf => conf.MapFrom(src => src.IsLayer3 ? src : null))
                .ForMember(dest => dest.PolicyBandwidthName, conf => conf.MapFrom(src => $"{src.MultiPortVlan.ContractBandwidthPool.Name}-{src.Interface.InterfaceID}"))
                .Include<InterfaceVlan, MultiPortVifServiceNetModel>();

            CreateMap<InterfaceVlan, MultiPortVifServiceNetModel>();

            CreateMap<MultiPortVif, MultiPortVifNetModel>()
               .ForMember(dest => dest.VlanID, conf => conf.MapFrom(src => src.VlanTag))
               .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3))
               .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vrf.Name))
               .ForMember(dest => dest.Layer3, conf => conf.MapFrom(src => src.IsLayer3 ? src : null))
               .ForMember(dest => dest.PolicyBandwidthName, conf => conf.MapFrom(src => $"{src.ContractBandwidthPool.Name}-{src.MemberAttachment.ID}"))
               .Include<MultiPortVif, MultiPortVifServiceNetModel>();

            CreateMap<MultiPortVif, MultiPortVifServiceNetModel>();

            CreateMap<MultiPortVif, Layer3NetModel>()
                .ForMember(dest => dest.BgpPeers, conf => conf.MapFrom(src => src.Vrf.BgpPeers));

            CreateMap<Interface, Layer3NetModel>()
                .ForMember(dest => dest.BgpPeers, conf => conf.MapFrom(src => src.Vrf.BgpPeers));

            CreateMap<InterfaceVlan, Layer3NetModel>()
                .ForMember(dest => dest.BgpPeers, conf => conf.MapFrom(src => src.Vrf.BgpPeers));

            CreateMap<Vrf, VrfNetModel>()
                .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Name))
                .ForMember(dest => dest.AdministratorSubField, conf => conf.MapFrom(src => src.AdministratorSubField))
                .ForMember(dest => dest.AssignedNumberSubField, conf => conf.MapFrom(src => src.AssignedNumberSubField))
                .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3));

            CreateMap<BgpPeer, BgpPeerNetModel>()
                .ForMember(dest => dest.PeerIpv4Address, conf => conf.MapFrom(src => src.IpAddress))
                .ForMember(dest => dest.PeerAutonomousSystem, conf => conf.MapFrom(src => src.AutonomousSystem));

            CreateMap<AttachmentAndVifs, Layer3NetModel>()
                .ForMember(dest => dest.BgpPeers, conf => conf.MapFrom(src => src.Vrf.BgpPeers));

            CreateMap<Vif, Layer3NetModel>()
                .ForMember(dest => dest.BgpPeers, conf => conf.MapFrom(src => src.Vrf.BgpPeers));

            CreateMap<AttachmentAndVifs, UntaggedAttachmentInterfaceNetModel>()
                .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3))
                .ForMember(dest => dest.InterfaceBandwidth, conf => conf.MapFrom(src => src.Bandwidth.BandwidthGbps))
                .ForMember(dest => dest.ContractBandwdithPool, conf => conf.MapFrom(src => src.ContractBandwidthPool))
                .ForMember(dest => dest.PolicyBandwidth, conf => conf.ResolveUsing(new UntaggedAttachmentInterfacePolicyBandwidthResolver()))
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

            CreateMap<AttachmentAndVifs, TaggedAttachmentInterfaceNetModel>()
                .ForMember(dest => dest.ContractBandwidthPools, conf => conf.MapFrom(src => src.Vifs.Select(q => q.ContractBandwidthPool).Distinct()))
                .Include<AttachmentAndVifs, TaggedAttachmentInterfaceServiceNetModel>();

            CreateMap<AttachmentAndVifs, TaggedAttachmentInterfaceServiceNetModel>();

            CreateMap<AttachmentAndVifs, UntaggedAttachmentBundleInterfaceNetModel>()
                .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3))
                .ForMember(dest => dest.InterfaceBandwidth, conf => conf.MapFrom(src => src.Bandwidth.BandwidthGbps))
                .ForMember(dest => dest.ContractBandwdithPool, conf => conf.MapFrom(src => src.ContractBandwidthPool))
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

            CreateMap<AttachmentAndVifs, TaggedAttachmentBundleInterfaceNetModel>()
                .ForMember(dest => dest.ContractBandwidthPools, conf => conf.MapFrom(src => src.Vifs.Select(q => q.ContractBandwidthPool).Distinct()))
                .Include<AttachmentAndVifs, TaggedAttachmentBundleInterfaceServiceNetModel>();

            CreateMap<AttachmentAndVifs, TaggedAttachmentBundleInterfaceServiceNetModel>();

            CreateMap<AttachmentAndVifs, UntaggedAttachmentMultiPortNetModel>()
               .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3))
               .Include<AttachmentAndVifs, UntaggedAttachmentMultiPortServiceNetModel>();

            CreateMap<AttachmentAndVifs, UntaggedAttachmentMultiPortServiceNetModel>();

            CreateMap<Attachment, TaggedAttachmentMultiPortNetModel>();

            CreateMap<Attachment, TaggedAttachmentMultiPortServiceNetModel>();

            CreateMap<AttachmentAndVifs, TaggedAttachmentMultiPortNetModel>()
               .ForMember(dest => dest.ContractBandwidthPools, conf => conf.MapFrom(src => src.Vifs.Select(q => q.ContractBandwidthPool).Distinct()))
               .Include<AttachmentAndVifs, TaggedAttachmentMultiPortServiceNetModel>();

            CreateMap<AttachmentAndVifs, TaggedAttachmentMultiPortServiceNetModel>();

            CreateMap<AttachmentAndVifs, VrfNetModel>()
                .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vrf.Name))
                .ForMember(dest => dest.AdministratorSubField, conf => conf.MapFrom(src => src.Vrf.AdministratorSubField))
                .ForMember(dest => dest.AssignedNumberSubField, conf => conf.MapFrom(src => src.Vrf.AssignedNumberSubField))
                .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.Vrf.IsLayer3))
                .Include<AttachmentAndVifs, VrfServiceNetModel>();

            CreateMap<AttachmentAndVifs, VrfServiceNetModel>();

            CreateMap<Vif, AttachmentVifNetModel>()
               .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3))
               .ForMember(dest => dest.VlanID, conf => conf.MapFrom(src => src.VlanTag))
               .ForMember(dest => dest.ContractBandwidthPoolName, conf => conf.MapFrom(src => src.ContractBandwidthPool.Name))
               .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vrf.Name))
               .ForMember(dest => dest.Layer3, conf => conf.MapFrom(src => src.IsLayer3 ? src : null))
               .Include<Vif, AttachmentVifServiceNetModel>();

            CreateMap<Vif, AttachmentVifServiceNetModel>();

            CreateMap<Vif, MultiPortVifNetModel>()
               .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3))
               .ForMember(dest => dest.VlanID, conf => conf.MapFrom(src => src.VlanTag))
               .ForMember(dest => dest.PolicyBandwidthName, conf => conf.MapFrom(src => $"{src.ContractBandwidthPool.Name}-{src.Attachment.ID}"))
               .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vrf.Name))
               .ForMember(dest => dest.Layer3, conf => conf.MapFrom(src => src.IsLayer3 ? src : null))
               .Include<Vif, MultiPortVifServiceNetModel>();

            CreateMap<Vif, MultiPortVifServiceNetModel>();

            CreateMap<Vif, VrfNetModel>()
              .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vrf.Name))
              .ForMember(dest => dest.AdministratorSubField, conf => conf.MapFrom(src => src.Vrf.AdministratorSubField))
              .ForMember(dest => dest.AssignedNumberSubField, conf => conf.MapFrom(src => src.Vrf.AssignedNumberSubField))
              .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.Vrf.IsLayer3))
              .Include<Vif, VrfServiceNetModel>();

            CreateMap<Vif, VrfServiceNetModel>();

            CreateMap<ContractBandwidthPool, ContractBandwidthPoolNetModel>()
                .ForMember(dest => dest.ContractBandwidth, conf => conf.MapFrom(src => src.ContractBandwidth.BandwidthMbps));

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
                        var vif = mapper.Map<AttachmentVifNetModel>(source);
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

                        data.ContractBandwidthPools.Add(mapper.Map<ContractBandwidthPoolNetModel>(source.ContractBandwidthPool));
                        result.TaggedAttachmentMultiPorts.Add(data);
                        result.Vrfs.Add(mapper.Map<VrfNetModel>(source.Vrf));
                    }
                }
                else
                {
                    if (source.Attachment.IsTagged)
                    {
                        var data = mapper.Map<TaggedAttachmentInterfaceNetModel>(source.Attachment);
                        var vif = mapper.Map<AttachmentVifNetModel>(source);
                        data.Vifs.Add(vif);
                        result.TaggedAttachmentInterfaces.Add(data);
                        result.Vrfs.Add(mapper.Map<VrfNetModel>(source.Vrf));
                    }
                }

                return result;
            }
        }

        public class UntaggedAttachmentInterfacePolicyBandwidthResolver : IValueResolver<object, UntaggedAttachmentInterfaceNetModel, PolicyBandwidthNetModel>
        {
            public PolicyBandwidthNetModel Resolve(object source, UntaggedAttachmentInterfaceNetModel destination, 
                PolicyBandwidthNetModel destMember, ResolutionContext context)
            {
                int interfaceBandwidthGbps = 0;
                int contractBandwidthMbps = 0;
                string name = string.Empty;

                if (source.GetType() == typeof(Interface))
                {
                    var item = (Interface)source;
                    interfaceBandwidthGbps = item.InterfaceBandwidth.BandwidthGbps;
                    contractBandwidthMbps = item.ContractBandwidthPool.ContractBandwidth.BandwidthMbps;
                    name = item.ContractBandwidthPool.Name;
                }
                else if (source.GetType() == typeof(AttachmentAndVifs))
                {
                    var item = (AttachmentAndVifs)source;
                    interfaceBandwidthGbps = item.Bandwidth.BandwidthGbps;
                    contractBandwidthMbps = item.ContractBandwidthPool.ContractBandwidth.BandwidthMbps;
                    name = item.ContractBandwidthPool.Name;
                }

                if (interfaceBandwidthGbps * 1000 > contractBandwidthMbps)
                {
                    var result = new PolicyBandwidthNetModel();
                    result.Name = name;
                    result.Bandwidth = contractBandwidthMbps;

                    return result;
                }
                else
                {
                    return null;
                }
            }
        }

        public class UntaggedAttachmentBundleInterfacePolicyBandwidthResolver : IValueResolver<Interface, UntaggedAttachmentBundleInterfaceNetModel, PolicyBandwidthNetModel>
        {
            public PolicyBandwidthNetModel Resolve(Interface source, UntaggedAttachmentBundleInterfaceNetModel destination,
                PolicyBandwidthNetModel destMember, ResolutionContext context)
            {
                if (source.InterfaceBandwidth.BandwidthGbps * 1000 > source.ContractBandwidthPool.ContractBandwidth.BandwidthMbps)
                {
                    var result = new PolicyBandwidthNetModel();
                    result.Name = source.ContractBandwidthPool.Name;
                    result.Bandwidth = source.ContractBandwidthPool.ContractBandwidth.BandwidthMbps;

                    return result;
                }
                else
                {
                    return null;
                }
            }
        }

        public class UntaggedMultiPortMemberPolicyBandwidthResolver : IValueResolver<Port, UntaggedMultiPortMemberNetModel, PolicyBandwidthNetModel>
        {
            public PolicyBandwidthNetModel Resolve(Port source, UntaggedMultiPortMemberNetModel destination, PolicyBandwidthNetModel destMember, ResolutionContext context)
            {
                var memberPortCount = source.MultiPort.Ports.Count();
                var interfaceBandwidthGbps = source.Interface.InterfaceBandwidth.BandwidthGbps;
                var contractBandwidthMbps = source.MultiPort.ContractBandwidthPool.ContractBandwidth.BandwidthMbps;

                if (interfaceBandwidthGbps * 1000 > (contractBandwidthMbps / memberPortCount))
                {
                    var result = new PolicyBandwidthNetModel();
                    result.Name = $"{source.MultiPort.ContractBandwidthPool.Name}-{source.ID}";
                    result.Bandwidth = contractBandwidthMbps / memberPortCount;

                    return result;
                }
                else
                {
                    return null;
                }
            }
        }

        public class TaggedMultiPortMemberPolicyBandwidthResolver : IValueResolver<Port, TaggedMultiPortMemberNetModel, List<TaggedMultiPortPolicyBandwidthNetModel>>
        {
            public List<TaggedMultiPortPolicyBandwidthNetModel> Resolve(Port source, TaggedMultiPortMemberNetModel destination, List<TaggedMultiPortPolicyBandwidthNetModel> destMember, ResolutionContext context)
            {
                var memberPortCount = source.MultiPort.Ports.Count();

                var result = new List<TaggedMultiPortPolicyBandwidthNetModel>();
                foreach (var multiPortVlan in source.MultiPort.MultiPortVlans)
                {
                    var bandwidth = multiPortVlan.ContractBandwidthPool.ContractBandwidth.BandwidthMbps / memberPortCount;

                    var policyBandwidth = new TaggedMultiPortPolicyBandwidthNetModel()
                    {
                        Name = $"{multiPortVlan.ContractBandwidthPool.Name}-{source.Interface.InterfaceID}",
                        Bandwidth = (int)Math.Round(bandwidth * 1.1, MidpointRounding.AwayFromZero)
                    };

                    policyBandwidth.ContractBandwidthPoolName = multiPortVlan.ContractBandwidthPool.Name;
                    result.Add(policyBandwidth);

                }

                return result;
            }
        }
    }
}
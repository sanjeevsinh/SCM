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
            CreateMap<Attachment, UntaggedAttachmentInterfaceNetModel>()
               .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3))
               .ForMember(dest => dest.AttachmentBandwidth, conf => conf.MapFrom(src => src.AttachmentBandwidth.BandwidthGbps))
               .ForMember(dest => dest.ContractBandwidthPool, conf => conf.MapFrom(src => src.ContractBandwidthPool))
               .ForMember(dest => dest.PolicyBandwidth, conf => conf.ResolveUsing(new UntaggedAttachmentInterfacePolicyBandwidthResolver()))
               .ForMember(dest => dest.InterfaceName, conf => conf.MapFrom(src => src.Interfaces.Single().Ports.Single().Name))
               .ForMember(dest => dest.InterfaceType, conf => conf.MapFrom(src => src.Interfaces.Single().Ports.Single().Type))
               .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vrf.Name))
               .ForMember(dest => dest.Layer3, conf => conf.MapFrom(src => src.IsLayer3 ? src.Interfaces.Single() : null))
               .Include<Attachment, UntaggedAttachmentInterfaceServiceNetModel>();

            CreateMap<Attachment, UntaggedAttachmentInterfaceServiceNetModel>();

            CreateMap<Attachment, TaggedAttachmentInterfaceNetModel>()
                .ForMember(dest => dest.AttachmentBandwidth, conf => conf.MapFrom(src => src.AttachmentBandwidth.BandwidthGbps))
                .ForMember(dest => dest.InterfaceName, conf => conf.MapFrom(src => src.Interfaces.Single().Ports.Single().Name))
                .ForMember(dest => dest.InterfaceType, conf => conf.MapFrom(src => src.Interfaces.Single().Ports.Single().Type))
                .ForMember(dest => dest.ContractBandwidthPools, conf => conf.MapFrom(src => src.Vifs.Select(q => q.ContractBandwidthPool)
                    .GroupBy(q => q.Name)
                    .Select(group => group.First())))
                .ForMember(dest => dest.Vifs, conf => conf.MapFrom(src => src.Vifs.SelectMany(q => q.Vlans)))
                .Include<Attachment, TaggedAttachmentInterfaceServiceNetModel>();

            CreateMap<Attachment, TaggedAttachmentInterfaceServiceNetModel>();

            CreateMap<Attachment, UntaggedAttachmentBundleInterfaceNetModel>()
                .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3))
                .ForMember(dest => dest.AttachmentBandwidth, conf => conf.MapFrom(src => src.AttachmentBandwidth.BandwidthGbps))
                .ForMember(dest => dest.ContractBandwidthPool, conf => conf.MapFrom(src => src.ContractBandwidthPool))
                .ForMember(dest => dest.PolicyBandwidth, conf => conf.ResolveUsing(new UntaggedAttachmentBundleInterfacePolicyBandwidthResolver()))
                .ForMember(dest => dest.BundleInterfaceMembers, conf => conf.MapFrom(src => src.Interfaces.SelectMany(q => q.Ports)))
                .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vrf.Name))
                .ForMember(dest => dest.Layer3, conf => conf.MapFrom(src => src.IsLayer3 ? src.Interfaces.Single() : null))
                .Include<Attachment, UntaggedAttachmentBundleInterfaceServiceNetModel>();

            CreateMap<Attachment, UntaggedAttachmentBundleInterfaceServiceNetModel>();

            CreateMap<Attachment, TaggedAttachmentBundleInterfaceNetModel>()
              .ForMember(dest => dest.AttachmentBandwidth, conf => conf.MapFrom(src => src.AttachmentBandwidth.BandwidthGbps))
              .ForMember(dest => dest.BundleID, conf => conf.MapFrom(src => src.ID))
              .ForMember(dest => dest.BundleInterfaceMembers, conf => conf.MapFrom(src => src.Interfaces.SelectMany(q => q.Ports)))
              .ForMember(dest => dest.ContractBandwidthPools, conf => conf.MapFrom(src => src.Vifs.Select(q => q.ContractBandwidthPool)
                    .GroupBy(q => q.Name)
                    .Select(group => group.First())))
              .ForMember(dest => dest.Vifs, conf => conf.MapFrom(src => src.Vifs.SelectMany(q => q.Vlans)))
              .Include<Attachment, TaggedAttachmentBundleInterfaceServiceNetModel>();

            CreateMap<Attachment, TaggedAttachmentBundleInterfaceServiceNetModel>();

            CreateMap<Attachment, UntaggedAttachmentMultiPortNetModel>()
               .ForMember(dest => dest.AttachmentBandwidth, conf => conf.MapFrom(src => src.AttachmentBandwidth.BandwidthGbps))
               .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3))
               .ForMember(dest => dest.MultiPortMembers, conf => conf.MapFrom(src => src.Interfaces.SelectMany(q => q.Ports)))
               .ForMember(dest => dest.Name, conf => conf.MapFrom(src => src.ID))
               .Include<Attachment, UntaggedAttachmentMultiPortServiceNetModel>();

            CreateMap<Attachment, UntaggedAttachmentMultiPortServiceNetModel>();

            CreateMap<Attachment, TaggedAttachmentMultiPortNetModel>()
                .ForMember(dest => dest.AttachmentBandwidth, conf => conf.MapFrom(src => src.AttachmentBandwidth.BandwidthGbps))
                .ForMember(dest => dest.ContractBandwidthPools, conf => conf.MapFrom(src => src.Vifs.Select(q => q.ContractBandwidthPool)
                     .GroupBy(q => q.Name)
                     .Select(group => group.First())))
                .ForMember(dest => dest.MultiPortMembers, conf => conf.MapFrom(src => src.Interfaces.SelectMany(q => q.Ports)))
                .ForMember(dest => dest.Name, conf => conf.MapFrom(src => src.ID))
                .Include<Attachment, TaggedAttachmentMultiPortServiceNetModel>();

            CreateMap<Attachment, TaggedAttachmentMultiPortServiceNetModel>();

            CreateMap<Attachment, VrfNetModel>()
                .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vrf.Name))
                .ForMember(dest => dest.AdministratorSubField, conf => conf.MapFrom(src => src.Vrf.AdministratorSubField))
                .ForMember(dest => dest.AssignedNumberSubField, conf => conf.MapFrom(src => src.Vrf.AssignedNumberSubField))
                .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.Vrf.IsLayer3))
                .Include<Attachment, VrfServiceNetModel>();

            CreateMap<Attachment, VrfServiceNetModel>();

            CreateMap<Interface, Layer3NetModel>()
                .ForMember(dest => dest.BgpPeers, conf => conf.MapFrom(src => src.Attachment.Vrf.BgpPeers));

            CreateMap<Vlan, AttachmentVifNetModel>()
               .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.Vif.IsLayer3))
               .ForMember(dest => dest.VlanID, conf => conf.MapFrom(src => src.Vif.VlanTag))
               .ForMember(dest => dest.ContractBandwidthPoolName, conf => conf.MapFrom(src => src.Vif.ContractBandwidthPool.Name))
               .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vif.Vrf.Name))
               .ForMember(dest => dest.Layer3, conf => conf.MapFrom(src => src.Vif.IsLayer3 ? src : null))
               .Include<Vlan, AttachmentVifServiceNetModel>();

            CreateMap<Vlan, AttachmentVifServiceNetModel>();

            CreateMap<Vlan, MultiPortVifNetModel>()
               .ForMember(dest => dest.VlanID, conf => conf.MapFrom(src => src.Vif.VlanTag))
               .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.Vif.IsLayer3))
               .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vif.Vrf.Name))
               .ForMember(dest => dest.Layer3, conf => conf.MapFrom(src => src.Vif.IsLayer3 ? src : null))
               .ForMember(dest => dest.PolicyBandwidthName, conf => conf.MapFrom(src => $"{src.Vif.ContractBandwidthPool.Name}-{src.Interface.InterfaceID}"))
               .Include<Vlan, MultiPortVifServiceNetModel>();

            CreateMap<Vlan, MultiPortVifServiceNetModel>();

            CreateMap<Vif, VrfNetModel>()
              .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vrf.Name))
              .ForMember(dest => dest.AdministratorSubField, conf => conf.MapFrom(src => src.Vrf.AdministratorSubField))
              .ForMember(dest => dest.AssignedNumberSubField, conf => conf.MapFrom(src => src.Vrf.AssignedNumberSubField))
              .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.Vrf.IsLayer3))
              .Include<Vif, VrfServiceNetModel>();

            CreateMap<Vif, VrfServiceNetModel>();

            CreateMap<Vlan, Layer3NetModel>()
                .ForMember(dest => dest.BgpPeers, conf => conf.MapFrom(src => src.Vif.Vrf.BgpPeers));

            CreateMap<Attachment, AttachmentServiceNetModel>().ConvertUsing(new AttachmentTypeConverter());

            CreateMap<Vif, AttachmentServiceNetModel>().ConvertUsing(new VifTypeConverter());

            CreateMap<Port, BundleInterfaceMemberNetModel>()
                .ForMember(dest => dest.InterfaceType, conf => conf.MapFrom(src => src.Type))
                .ForMember(dest => dest.InterfaceName, conf => conf.MapFrom(src => src.Name));

            CreateMap<Port, UntaggedMultiPortMemberNetModel>()
                .ForMember(dest => dest.InterfaceType, conf => conf.MapFrom(src => src.Type))
                .ForMember(dest => dest.InterfaceName, conf => conf.MapFrom(src => src.Name))
                .ForMember(dest => dest.PolicyBandwidth, conf => conf.ResolveUsing(new UntaggedMultiPortMemberPolicyBandwidthResolver()))
                .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Interface.Attachment.Vrf.Name))
                .ForMember(dest => dest.Layer3, conf => conf.MapFrom(src => src.Interface.Attachment.IsLayer3 ? src.Interface : null));

            CreateMap<Port, TaggedMultiPortMemberNetModel>()
                .ForMember(dest => dest.InterfaceType, conf => conf.MapFrom(src => src.Type))
                .ForMember(dest => dest.InterfaceName, conf => conf.MapFrom(src => src.Name))
                .ForMember(dest => dest.PolicyBandwidths, conf => conf.ResolveUsing(new TaggedMultiPortMemberPolicyBandwidthResolver()))
                .ForMember(dest => dest.Vifs, conf => conf.MapFrom(src => src.Interface.Vlans));

            CreateMap<Interface, Layer3NetModel>()
                .ForMember(dest => dest.BgpPeers, conf => conf.MapFrom(src => src.Attachment.Vrf.BgpPeers));

            CreateMap<Vlan, Layer3NetModel>()
                .ForMember(dest => dest.BgpPeers, conf => conf.MapFrom(src => src.Vif.Vrf.BgpPeers));

            CreateMap<Vrf, VrfNetModel>()
                .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Name))
                .ForMember(dest => dest.AdministratorSubField, conf => conf.MapFrom(src => src.AdministratorSubField))
                .ForMember(dest => dest.AssignedNumberSubField, conf => conf.MapFrom(src => src.AssignedNumberSubField))
                .ForMember(dest => dest.EnableLayer3, conf => conf.MapFrom(src => src.IsLayer3));

            CreateMap<BgpPeer, BgpPeerNetModel>()
                .ForMember(dest => dest.PeerIpv4Address, conf => conf.MapFrom(src => src.IpAddress))
                .ForMember(dest => dest.PeerAutonomousSystem, conf => conf.MapFrom(src => src.AutonomousSystem));

            CreateMap<ContractBandwidthPool, ContractBandwidthPoolNetModel>()
                .ForMember(dest => dest.ContractBandwidth, conf => conf.MapFrom(src => src.ContractBandwidth.BandwidthMbps));

            CreateMap<Device, AttachmentServiceNetModel>().ConvertUsing(new DeviceTypeConverter());
        }

        public class DeviceTypeConverter : ITypeConverter<Device, AttachmentServiceNetModel>
        {
            public AttachmentServiceNetModel Convert(Device source, AttachmentServiceNetModel destination, ResolutionContext context)
            {
                var result = new AttachmentServiceNetModel();
                var mapper = context.Mapper;

                result.PEName = source.Name;
                result.Vrfs = mapper.Map<List<VrfNetModel>>(source.Vrfs);

                if (source.Attachments != null)
                {
                    foreach (var attachment in source.Attachments)
                    {
                        if (attachment.IsBundle)
                        {
                            if (attachment.IsTagged)
                            {
                                result.TaggedAttachmentBundleInterfaces.Add(mapper.Map<TaggedAttachmentBundleInterfaceNetModel>(attachment));
                            }
                            else
                            {
                                result.UntaggedAttachmentBundleInterfaces.Add(mapper.Map<UntaggedAttachmentBundleInterfaceNetModel>(attachment));
                            }
                        }
                        else if (attachment.IsMultiPort)
                        {
                            if (attachment.IsTagged)
                            {
                                result.TaggedAttachmentMultiPorts.Add(mapper.Map<TaggedAttachmentMultiPortNetModel>(attachment));
                            }
                            else
                            {
                                result.UntaggedAttachmentMultiPorts.Add(mapper.Map<UntaggedAttachmentMultiPortNetModel>(attachment));
                            }
                        }
                        else
                        {
                            if (attachment.IsTagged)
                            {
                                result.TaggedAttachmentInterfaces.Add(mapper.Map<TaggedAttachmentInterfaceNetModel>(attachment));
                            }
                            else
                            {
                                result.UntaggedAttachmentInterfaces.Add(mapper.Map<UntaggedAttachmentInterfaceNetModel>(attachment));
                            }
                        }
                    }
                }

                return result;
            }
        }

        public class AttachmentTypeConverter : ITypeConverter<Attachment, AttachmentServiceNetModel>
        {
            public AttachmentServiceNetModel Convert(Attachment source, AttachmentServiceNetModel destination, ResolutionContext context)
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

                result.PEName = source.Attachment.Device.Name;

                if (source.Attachment.IsBundle)
                {
                    if (source.Attachment.IsTagged)
                    {
                        var data = mapper.Map<TaggedAttachmentBundleInterfaceNetModel>(source.Attachment);
                        var vif = mapper.Map<AttachmentVifNetModel>(source.Vlans.Single());
                        data.Vifs.Add(vif);
                        data.ContractBandwidthPools.Add(mapper.Map<ContractBandwidthPoolNetModel>(source.ContractBandwidthPool));
                        result.TaggedAttachmentBundleInterfaces.Add(data);
                        result.Vrfs.Add(mapper.Map<VrfNetModel>(source.Vrf));
                    }
                }
                else if (source.Attachment.IsMultiPort)
                {
                    if (source.Attachment.IsTagged)
                    {
                        var data = mapper.Map<TaggedAttachmentMultiPortNetModel>(source.Attachment);

                        // Filter data to remove all VIFs and policy-bandwidths other than the VIF we're interested in
                        // and the policy-bandwidth for that VIF

                        foreach (var member in data.MultiPortMembers)
                        {
                            member.Vifs = member.Vifs.Where(q => q.VlanID == source.VlanTag).ToList();
                            var vif = member.Vifs.Single();
                            member.PolicyBandwidths = member.PolicyBandwidths.Where(q => q.Name == vif.PolicyBandwidthName).ToList();
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
                        var vif = mapper.Map<AttachmentVifNetModel>(source.Vlans.Single());
                        data.Vifs.Add(vif);
                        data.ContractBandwidthPools.Add(mapper.Map<ContractBandwidthPoolNetModel>(source.ContractBandwidthPool));
                        result.TaggedAttachmentInterfaces.Add(data);
                        result.Vrfs.Add(mapper.Map<VrfNetModel>(source.Vrf));
                    }
                }

                return result;
            }
        }

        public class UntaggedAttachmentInterfacePolicyBandwidthResolver : IValueResolver<Attachment, UntaggedAttachmentInterfaceNetModel, PolicyBandwidthNetModel>
        {
            public PolicyBandwidthNetModel Resolve(Attachment source, UntaggedAttachmentInterfaceNetModel destination,
                PolicyBandwidthNetModel destMember, ResolutionContext context)
            {

                var interfaceBandwidthGbps = source.AttachmentBandwidth.BandwidthGbps;
                var contractBandwidthMbps = source.ContractBandwidthPool.ContractBandwidth.BandwidthMbps;

                if (interfaceBandwidthGbps * 1000 > contractBandwidthMbps)
                {
                    var result = new PolicyBandwidthNetModel();
                    result.Name = source.ContractBandwidthPool.Name;
                    result.Bandwidth = contractBandwidthMbps;

                    return result;
                }
                else
                {
                    return null;
                }
            }
        }
    }

    public class UntaggedAttachmentInterfacePolicyBandwidthResolver : IValueResolver<Attachment, UntaggedAttachmentInterfaceNetModel, PolicyBandwidthNetModel>
    {
        public PolicyBandwidthNetModel Resolve(Attachment source, UntaggedAttachmentInterfaceNetModel destination,
            PolicyBandwidthNetModel destMember, ResolutionContext context)
        {

            var attachmentBandwidthGbps = source.AttachmentBandwidth.BandwidthGbps;
            var contractBandwidthMbps = source.ContractBandwidthPool.ContractBandwidth.BandwidthMbps;

            if (attachmentBandwidthGbps * 1000 > contractBandwidthMbps)
            {
                var result = new PolicyBandwidthNetModel();
                result.Name = source.ContractBandwidthPool.Name;
                result.Bandwidth = contractBandwidthMbps;

                return result;
            }
            else
            {
                return null;
            }
        }
    }

    public class UntaggedAttachmentBundleInterfacePolicyBandwidthResolver : IValueResolver<Attachment, UntaggedAttachmentBundleInterfaceNetModel, PolicyBandwidthNetModel>
    {
        public PolicyBandwidthNetModel Resolve(Attachment source, UntaggedAttachmentBundleInterfaceNetModel destination,
            PolicyBandwidthNetModel destMember, ResolutionContext context)
        {
            if (source.AttachmentBandwidth.BandwidthGbps * 1000 > source.ContractBandwidthPool.ContractBandwidth.BandwidthMbps)
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
            var memberPortCount = source.Interface.Ports.Count();
            var portBandwidthGbps = source.PortBandwidth.BandwidthGbps;
            var contractBandwidthMbps = source.Interface.Attachment.ContractBandwidthPool.ContractBandwidth.BandwidthMbps;

            if (portBandwidthGbps * 1000 > (contractBandwidthMbps / memberPortCount))
            {
                var result = new PolicyBandwidthNetModel();
                result.Name = $"{source.Interface.Attachment.ContractBandwidthPool.Name}-{source.InterfaceID}";
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
            var attachment = source.Interface.Attachment;
            var membersCount = attachment.Interfaces.Count();

            var result = new List<TaggedMultiPortPolicyBandwidthNetModel>();
            foreach (var vif in attachment.Vifs)
            {
                var bandwidth = vif.ContractBandwidthPool.ContractBandwidth.BandwidthMbps / membersCount;

                var policyBandwidth = new TaggedMultiPortPolicyBandwidthNetModel()
                {
                    Name = $"{vif.ContractBandwidthPool.Name}-{source.InterfaceID}",
                    Bandwidth = (int)Math.Round(bandwidth * 1.1, MidpointRounding.AwayFromZero)
                };

                policyBandwidth.ContractBandwidthPoolName = vif.ContractBandwidthPool.Name;
                result.Add(policyBandwidth);

            }

            // Return only unique (according to the name property) Policy Bandwidths

            return result
                .GroupBy(q => q.Name)
                .Select(group => group.First())
                .ToList();
        }
    }
}
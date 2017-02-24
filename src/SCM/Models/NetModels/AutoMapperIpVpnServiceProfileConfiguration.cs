using AutoMapper;
using SCM.Models;
using System.Collections.Generic;
using System;
using System.Linq;

namespace SCM.Models.NetModels.IpVpn
{
    public class AutoMapperIpVpnServiceProfileConfiguration : Profile
    {
        public AutoMapperIpVpnServiceProfileConfiguration()
        {
            CreateMap<RouteTarget, RouteTargetNetModel>();

            CreateMap<AttachmentSetVrf, VrfNetModel>()
                .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Vrf.Name))
                .ForMember(dest => dest.Preference, conf => conf.MapFrom(src => src.Preference));

            CreateMap<VpnAttachmentSet, VpnAttachmentSetNetModel>().ConvertUsing(new VpnAttachmentSetTypeConverter());

            CreateMap<Device, PENetModel>()
                .ForMember(dest => dest.PEName, conf => conf.MapFrom(src => src.Name))
                .ForMember(dest => dest.Vrfs, conf => conf.Ignore());

            CreateMap<VpnTenantNetwork, TenantPrefixNetModel>()
                .ForMember(dest => dest.Prefix, conf => conf.MapFrom(src => src.TenantNetwork.IpPrefix + "/" + src.TenantNetwork.Length));

            CreateMap<VpnTenantCommunity, TenantCommunityNetModel>()
                .ForMember(dest => dest.AutonomousSystemNumber, conf => conf.MapFrom(src => src.TenantCommunity.AutonomousSystemNumber))
                .ForMember(dest => dest.Number, conf => conf.MapFrom(src => src.TenantCommunity.Number));

            CreateMap<Vpn, IpVpnServiceNetModel>().ConvertUsing(new VpnTypeConverter());
        }

        public class VpnTypeConverter : ITypeConverter<Vpn, IpVpnServiceNetModel>
        {
            public IpVpnServiceNetModel Convert(Vpn source, IpVpnServiceNetModel destination, ResolutionContext context)
            {
                var result = new IpVpnServiceNetModel();
                var Mapper = context.Mapper;

                result.Name = source.Name;

                result.TopologyType = source.VpnTopologyType.TopologyType;

                if (source.VpnTopologyType.TopologyType == "Any-to-Any")
                {
                    result.RouteTargetA = Mapper.Map<RouteTargetNetModel>(source.RouteTargets.Single());
                }
                else
                {
                    result.IsExtranet = source.IsExtranet;
                    result.RouteTargetA = Mapper.Map<RouteTargetNetModel>(source.RouteTargets.Single(r => !r.IsHubExport));
                    result.RouteTargetB = Mapper.Map<RouteTargetNetModel>(source.RouteTargets.Single(r => r.IsHubExport));
                }

                result.VpnAttachmentSets = Mapper.Map<List<VpnAttachmentSetNetModel>>(source.VpnAttachmentSets);

                return result;
            }
        }

        public class VpnAttachmentSetTypeConverter : ITypeConverter<VpnAttachmentSet, VpnAttachmentSetNetModel>
        {
            public VpnAttachmentSetNetModel Convert(VpnAttachmentSet source, VpnAttachmentSetNetModel destination, ResolutionContext context)
            {
                var result = new VpnAttachmentSetNetModel();
                var Mapper = context.Mapper;

                var devices = source.AttachmentSet.AttachmentSetVrfs.Select(s => s.Vrf.Device).ToList();

                var PEs = Mapper.Map<List<PENetModel>>(devices);
                foreach (PENetModel PE in PEs)
                {
                    PE.Vrfs = Mapper.Map<List<VrfNetModel>>(source.AttachmentSet.AttachmentSetVrfs.Where(v => v.Vrf.Device.Name == PE.PEName));
                }

                result.PEs = PEs;
                result.TenantPrefixes = Mapper.Map<List<TenantPrefixNetModel>>(source.VpnTenantNetworks);
                result.TenantCommunities = Mapper.Map<List<TenantCommunityNetModel>>(source.VpnTenantCommunities);
                result.Name = source.AttachmentSet.Name;

                return result;
            }
        }
    }
}
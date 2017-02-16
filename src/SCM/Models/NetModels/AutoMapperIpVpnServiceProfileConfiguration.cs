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

            CreateMap<Vrf, VrfNetModel>()
                .ForMember(dest => dest.VrfName, conf => conf.MapFrom(src => src.Name));

            CreateMap<VpnAttachmentSet, VpnAttachmentSetNetModel>().ConvertUsing(new VpnAttachmentSetTypeConverter());

            CreateMap<Device, PENetModel>()
                .ForMember(dest => dest.PEName, conf => conf.MapFrom(src => src.Name));

            CreateMap<VpnTenantNetwork, TenantPrefixNetModel>()
                .ForMember(dest => dest.Prefix, conf => conf.MapFrom(src => src.TenantNetwork.IpPrefix + "/" + src.TenantNetwork.Length));

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

                var devices = source.AttachmentSet.AttachmentSetVrfs.Select(s => s.Vrf.Device);
                result.PEs = Mapper.Map<List<PENetModel>>(devices);

                //var tenantNetworks = source.VpnTenantNetworks.Select(s => s.TenantNetwork);
                result.TenantPrefixes = Mapper.Map<List<TenantPrefixNetModel>>(source.VpnTenantNetworks);

                result.Name = source.AttachmentSet.Name;

                return result;
            }
        }
    }
}
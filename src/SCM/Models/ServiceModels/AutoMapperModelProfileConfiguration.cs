﻿using AutoMapper;
using System.Linq;

namespace SCM.Models.ServiceModels
{
    public class AutoMapperServiceModelProfileConfiguration : Profile
    {
        public AutoMapperServiceModelProfileConfiguration()
        {
            CreateMap<Interface, Attachment>()
                .ForMember(dest => dest.Bandwidth, conf => conf.MapFrom(src => src.InterfaceBandwidth))
                .ForMember(dest => dest.BandwidthID, conf => conf.MapFrom(src => src.InterfaceBandwidthID))
                .ForMember(dest => dest.ID, conf => conf.MapFrom(src => src.InterfaceID))
                .ForMember(dest => dest.InterfaceType, conf => conf.MapFrom(src => src.Port != null ? src.Port.Type : null))
                .ForMember(dest => dest.InterfaceName, conf => conf.MapFrom(src => src.Port != null ? src.Port.Name : null))
                .ForMember(dest => dest.Location, conf => conf.MapFrom(src => src.Device.Location))
                .ForMember(dest => dest.LocationID, conf => conf.MapFrom(src => src.Device.LocationID))
                .ForMember(dest => dest.Name, conf => conf.ResolveUsing(new AttachmentNameResolver()))
                .ForMember(dest => dest.Plane, conf => conf.MapFrom(src => src.Device.Plane))
                .ForMember(dest => dest.PlaneID, conf => conf.MapFrom(src => src.Device.PlaneID))
                .ForMember(dest => dest.Region, conf => conf.MapFrom(src => src.Device.Location.SubRegion.Region))
                .ForMember(dest => dest.RegionID, conf => conf.MapFrom(src => src.Device.Location.SubRegion.RegionID))
                .ForMember(dest => dest.SubRegion, conf => conf.MapFrom(src => src.Device.Location.SubRegion))
                .ForMember(dest => dest.SubRegionID, conf => conf.MapFrom(src => src.Device.Location.SubRegionID))
                .ForMember(dest => dest.Tenant, conf => conf.MapFrom(src => src.Tenant))
                .ForMember(dest => dest.TenantID, conf => conf.MapFrom(src => src.TenantID))
                .ForMember(dest => dest.Vifs, conf => conf.MapFrom(src => src.InterfaceVlans));

            CreateMap<AttachmentRequest, Interface>()
                .ForMember(dest => dest.InterfaceBandwidthID, conf => conf.MapFrom(src => src.BandwidthID))
                .ForMember(dest => dest.IsBundle, conf => conf.MapFrom(src => src.BundleRequired))
                .ForMember(dest => dest.IsMultiPort, conf => conf.MapFrom(src => src.MultiPortRequired));

            CreateMap<AttachmentRequest, MultiPort>();

            CreateMap<AttachmentRequest, Vrf>();

            CreateMap<VifRequest, InterfaceVlan>()
                .ForMember(dest => dest.InterfaceID, conf => conf.MapFrom(src => src.AttachmentID))
                .ForMember(dest => dest.VlanTag, conf => conf.MapFrom(src => src.AllocatedVlanTag));

            CreateMap<InterfaceVlan, Vif>()
                .ForMember(dest => dest.ID, conf => conf.MapFrom(src => src.InterfaceVlanID))
                .ForMember(dest => dest.Name, conf => conf.ResolveUsing(new VifNameResolver()))
                .ForMember(dest => dest.Tenant, conf => conf.MapFrom(src => src.Tenant))
                .ForMember(dest => dest.TenantID, conf => conf.MapFrom(src => src.TenantID))
                .ForMember(dest => dest.AttachmentID, conf => conf.MapFrom(src => src.InterfaceID));

            CreateMap<VifRequest, Vrf>();

            CreateMap<RouteTargetRequest, RouteTarget>()
                .ForMember(dest => dest.AssignedNumberSubField, conf => conf.MapFrom(src => src.AllocatedAssignedNumberSubField));

            CreateMap<Interface, AttachmentOrVif>().ConvertUsing(new InterfaceAttachmentSetMemberTypeConverter());
            CreateMap<InterfaceVlan, AttachmentOrVif>().ConvertUsing(new InterfaceVlanAttachmentSetMemberTypeConverter());
        }

        public class AttachmentNameResolver : IValueResolver<Interface, Attachment, string>
        {
            public string Resolve(Interface source, Attachment destination, string destMember, ResolutionContext context)
            {
                if (source.IsBundle)
                {
                    return $"Bundle {source.BundleID.ToString()}";
                }
                else
                {
                    return $"{source.Port.Type} {source.Port.Name}";
                }
            }
        }
        public class VifNameResolver : IValueResolver<InterfaceVlan, Vif, string>
        {
            public string Resolve(InterfaceVlan source, Vif destination, string destMember, ResolutionContext context)
            {
                if (source.Interface.IsBundle)
                {
                    return $"Bundle {source.Interface.BundleID.ToString()}.{source.VlanTag}";
                }
                else
                {
                    return $"{source.Interface.Port.Type} {source.Interface.Port.Name}.{source.VlanTag}";
                }
            }
        }
        public class InterfaceAttachmentSetMemberTypeConverter : ITypeConverter<Interface, AttachmentOrVif>
        {
            public AttachmentOrVif Convert(Interface source, AttachmentOrVif destination, ResolutionContext context)
            {
                var mapper = context.Mapper;
                var result = new AttachmentOrVif();

                result.ID = source.InterfaceID;
                if (source.IsBundle)
                {
                    result.IsBundle = true;
                    result.AttachmentName = $"Bundle {source.BundleID}";
                }
                else
                {
                    result.AttachmentName = source.Port.Type + source.Port.Name;
                }

                if (source.Vrf.AttachmentSetVrfs.Count > 0)
                {
                    result.AttachmentSetName = source.Vrf.AttachmentSetVrfs.First().AttachmentSet.Name;
                }

                result.ContractBandwidth = source.ContractBandwidthPool.ContractBandwidth.BandwidthMbps;
                result.ContractBandwidthPool = source.ContractBandwidthPool.Name;
                result.DeviceName = source.Device.Name;
                result.InterfaceBandwidth = source.InterfaceBandwidth.BandwidthGbps;
                result.IpAddress = source.IpAddress;
                result.Location = source.Device.Location.SiteName;
                result.PlaneName = source.Device.Plane.Name;
                result.Region = source.Device.Location.SubRegion.Region.Name;
                result.SubRegion = source.Device.Location.SubRegion.Name;
                result.TenantName = source.Tenant.Name;
                result.SubnetMask = source.SubnetMask;
                result.TenantName = source.Tenant.Name;
                result.VrfName = source.Vrf.Name;
                result.RequiresSync = source.RequiresSync;

                return result;
            }
        }
        public class InterfaceVlanAttachmentSetMemberTypeConverter : ITypeConverter<InterfaceVlan, AttachmentOrVif>
        {
            public AttachmentOrVif Convert(InterfaceVlan source, AttachmentOrVif destination, ResolutionContext context)
            {
                var mapper = context.Mapper;
                var result = new AttachmentOrVif();

                result.ID = source.InterfaceVlanID;
                if (source.Interface.IsBundle)
                {
                    result.IsBundle = true;
                    result.AttachmentName = $"Bundle {source.Interface.BundleID}";
                }
                else
                {
                    result.AttachmentName = source.Interface.Port.Type + source.Interface.Port.Name;
                }
                if (source.Vrf.AttachmentSetVrfs.Count > 0)
                {
                    result.AttachmentSetName = source.Vrf.AttachmentSetVrfs.First().AttachmentSet.Name;
                }

                result.ContractBandwidth = source.ContractBandwidthPool.ContractBandwidth.BandwidthMbps;
                result.ContractBandwidthPool = source.ContractBandwidthPool.Name;
                result.DeviceName = source.Interface.Device.Name;
                result.InterfaceBandwidth = source.Interface.InterfaceBandwidth.BandwidthGbps;
                result.IpAddress = source.IpAddress;
                result.SubnetMask = source.SubnetMask;
                result.Location = source.Interface.Device.Location.SiteName;
                result.PlaneName = source.Interface.Device.Plane.Name;
                result.Region = source.Interface.Device.Location.SubRegion.Region.Name;
                result.SubRegion = source.Interface.Device.Location.SubRegion.Name;
                result.TenantName = source.Tenant.Name;
                result.IsVif = true;
                result.VifName = $"{result.AttachmentName}.{source.VlanTag}";
                result.VrfName = source.Vrf.Name;
                result.RequiresSync = source.RequiresSync;

                return result;
            }
        }
    }
}
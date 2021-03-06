﻿using System;
using SCM.Models;
using SCM.Data;
using System.Threading.Tasks;

namespace SCM.Data
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private SigmaContext context;
        private GenericRepository<Attachment> attachmentRepository;
        private GenericRepository<AttachmentSet> attachmentSetRepository;
        private GenericRepository<AttachmentSetVrf> attachmentSetVrfRepository;
        private GenericRepository<VpnAttachmentSet> vpnAttachmentSetRepository;
        private GenericRepository<AttachmentRedundancy> attachmentRedundancyRepository;
        private GenericRepository<BgpPeer> bgpPeerRepository;
        private GenericRepository<ContractBandwidth> contractBandwidthRepository;
        private GenericRepository<ContractBandwidthPool> contractBandwidthPoolRepository;
        private GenericRepository<Device> deviceRepository;
        private GenericRepository<Interface> interfaceRepository;
        private GenericRepository<Vlan> vlanRepository;
        private GenericRepository<Vif> vifRepository;
        private GenericRepository<Location> locationRepository;
        private GenericRepository<AttachmentBandwidth> attachmentBandwidthRepository;
        private GenericRepository<PortBandwidth> portBandwidthRepository;
        private GenericRepository<Port> portRepository;
        private GenericRepository<Region> regionRepository;
        private GenericRepository<RouteTarget> routeTargetRepository;
        private GenericRepository<SubRegion> subRegionRepository;
        private GenericRepository<Tenant> tenantRepository;
        private GenericRepository<TenantNetwork> tenantNetworkRepository;
        private GenericRepository<TenantCommunity> tenantCommunityRepository;
        private GenericRepository<Vpn> vpnRepository;
        private GenericRepository<VpnTenantNetwork> vpnTenantNetworkRepository;
        private GenericRepository<VpnTenantCommunity> vpnTenantCommunityRepository;
        private GenericRepository<VpnProtocolType> vpnProtocolTypeRepository;
        private GenericRepository<VpnTenancyType> vpnTenancyTypeRepository;
        private GenericRepository<VpnTopologyType> vpnTopologyTypeRepository;
        private GenericRepository<Plane> planeRepository;
        private GenericRepository<Vrf> vrfRepository;
        private GenericRepository<RouteTargetRange> routeTargetRangeRepository;
        private GenericRepository<RouteDistinguisherRange> routeDistinguisherRangeRepository;
        private GenericRepository<VlanTagRange> vlanTagRangeRepository;

        public UnitOfWork(SigmaContext sigmaContext)
        {
            context = sigmaContext;
        }

        public GenericRepository<Attachment> AttachmentRepository
        {
            get
            {
                if (this.attachmentRepository == null)
                {
                    this.attachmentRepository = new GenericRepository<Attachment>(context);
                }
                return attachmentRepository;
            }
        }

        public GenericRepository<AttachmentBandwidth> AttachmentBandwidthRepository
        {
            get
            {
                if (this.attachmentBandwidthRepository == null)
                {
                    this.attachmentBandwidthRepository = new GenericRepository<AttachmentBandwidth>(context);
                }
                return attachmentBandwidthRepository;
            }
        }

        public GenericRepository<AttachmentSet> AttachmentSetRepository
        {
            get
            {
                if (this.attachmentSetRepository == null)
                {
                    this.attachmentSetRepository = new GenericRepository<AttachmentSet>(context);
                }
                return attachmentSetRepository;
            }
        }

        public GenericRepository<AttachmentSetVrf> AttachmentSetVrfRepository
        {
            get
            {
                if (this.attachmentSetVrfRepository == null)
                {
                    this.attachmentSetVrfRepository = new GenericRepository<AttachmentSetVrf>(context);
                }
                return attachmentSetVrfRepository;
            }
        }

        public GenericRepository<VpnAttachmentSet> VpnAttachmentSetRepository
        {
            get
            {
                if (this.vpnAttachmentSetRepository == null)
                {
                    this.vpnAttachmentSetRepository = new GenericRepository<VpnAttachmentSet>(context);
                }
                return vpnAttachmentSetRepository;
            }
        }

        public GenericRepository<AttachmentRedundancy> AttachmentRedundancyRepository
        {
            get
            {
                if (this.attachmentRedundancyRepository == null)
                {
                    this.attachmentRedundancyRepository = new GenericRepository<AttachmentRedundancy>(context);
                }
                return attachmentRedundancyRepository;
            }
        }

        public GenericRepository<BgpPeer> BgpPeerRepository
        {
            get
            {
                if (this.bgpPeerRepository == null)
                {
                    this.bgpPeerRepository = new GenericRepository<BgpPeer>(context);
                }
                return bgpPeerRepository;
            }
        }

        public GenericRepository<ContractBandwidth>  ContractBandwidthRepository
        {
            get
            {
                if (this.contractBandwidthRepository == null)
                {
                    this.contractBandwidthRepository = new GenericRepository<ContractBandwidth>(context);
                }
                return contractBandwidthRepository;
            }
        }
        public GenericRepository<ContractBandwidthPool> ContractBandwidthPoolRepository
        {
            get
            {
                if (this.contractBandwidthPoolRepository == null)
                {
                    this.contractBandwidthPoolRepository = new GenericRepository<ContractBandwidthPool>(context);
                }
                return contractBandwidthPoolRepository;
            }
        }

        public GenericRepository<Device> DeviceRepository
        {
            get
            {
                if (this.deviceRepository == null)
                {
                    this.deviceRepository = new GenericRepository<Device>(context);
                }
                return deviceRepository;
            }
        }

        public GenericRepository<Interface> InterfaceRepository
        {
            get
            {
                if (this.interfaceRepository == null)
                {
                    this.interfaceRepository = new GenericRepository<Interface>(context);
                }
                return interfaceRepository;
            }
        }

        public GenericRepository<Vlan> VlanRepository
        {
            get
            {
                if (this.vlanRepository == null)
                {
                    this.vlanRepository = new GenericRepository<Vlan>(context);
                }
                return vlanRepository;
            }
        }

        public GenericRepository<Vif> VifRepository
        {
            get
            {
                if (this.vifRepository == null)
                {
                    this.vifRepository = new GenericRepository<Vif>(context);
                }
                return vifRepository;
            }
        }

        public GenericRepository<Location> LocationRepository
        {
            get
            {
                if (this.locationRepository == null)
                {
                    this.locationRepository = new GenericRepository<Location>(context);
                }
                return locationRepository;
            }
        }


        public GenericRepository<PortBandwidth> PortBandwidthRepository
        {
            get
            {
                if (this.portBandwidthRepository == null)
                {
                    this.portBandwidthRepository = new GenericRepository<PortBandwidth>(context);
                }
                return portBandwidthRepository;
            }
        }

        public GenericRepository<Port> PortRepository
        {
            get
            {
                if (this.portRepository == null)
                {
                    this.portRepository = new GenericRepository<Port>(context);
                }
                return portRepository;
            }
        }

        public GenericRepository<Region> RegionRepository
        {
            get
            {
                if (this.regionRepository == null)
                {
                    this.regionRepository = new GenericRepository<Region>(context);
                }
                return regionRepository;
            }
        }

        public GenericRepository<RouteTarget> RouteTargetRepository
        {
            get
            {
                if (this.routeTargetRepository == null)
                {
                    this.routeTargetRepository = new GenericRepository<RouteTarget>(context);
                }
                return routeTargetRepository;
            }
        }

        public GenericRepository<SubRegion> SubRegionRepository
        {
            get
            {
                if (this.subRegionRepository == null)
                {
                    this.subRegionRepository = new GenericRepository<SubRegion>(context);
                }
                return subRegionRepository;
            }
        }
        public GenericRepository<Tenant> TenantRepository
        {
            get
            {
                if (this.tenantRepository == null)
                {
                    this.tenantRepository = new GenericRepository<Tenant>(context);
                }
                return tenantRepository;
            }
        }

        public GenericRepository<TenantNetwork> TenantNetworkRepository
        {
            get
            {
                if (this.tenantNetworkRepository == null)
                {
                    this.tenantNetworkRepository = new GenericRepository<TenantNetwork>(context);
                }
                return tenantNetworkRepository;
            }
        }

        public GenericRepository<TenantCommunity> TenantCommunityRepository
        {
            get
            {
                if (this.tenantCommunityRepository == null)
                {
                    this.tenantCommunityRepository = new GenericRepository<TenantCommunity>(context);
                }
                return tenantCommunityRepository;
            }
        }

        public GenericRepository<Vpn> VpnRepository
        {
            get
            {
                if (this.vpnRepository == null)
                {
                    this.vpnRepository = new GenericRepository<Vpn>(context);
                }
                return vpnRepository;
            }
        }

        public GenericRepository<VpnTenantNetwork> VpnTenantNetworkRepository
        {
            get
            {
                if (this.vpnTenantNetworkRepository == null)
                {
                    this.vpnTenantNetworkRepository = new GenericRepository<VpnTenantNetwork>(context);
                }
                return vpnTenantNetworkRepository;
            }
        }

        public GenericRepository<VpnTenantCommunity> VpnTenantCommunityRepository
        {
            get
            {
                if (this.vpnTenantCommunityRepository == null)
                {
                    this.vpnTenantCommunityRepository = new GenericRepository<VpnTenantCommunity>(context);
                }
                return vpnTenantCommunityRepository;
            }
        }


        public GenericRepository<VpnProtocolType> VpnProtocolTypeRepository
        {
            get
            {
                if (this.vpnProtocolTypeRepository == null)
                {
                    this.vpnProtocolTypeRepository = new GenericRepository<VpnProtocolType>(context);
                }
                return vpnProtocolTypeRepository;
            }
        }

        public GenericRepository<VpnTenancyType> VpnTenancyTypeRepository
        {
            get
            {
                if (this.vpnTenancyTypeRepository == null)
                {
                    this.vpnTenancyTypeRepository = new GenericRepository<VpnTenancyType>(context);
                }
                return vpnTenancyTypeRepository;
            }
        }
   
        public GenericRepository<VpnTopologyType> VpnTopologyTypeRepository
        {
            get
            {
                if (this.vpnTopologyTypeRepository == null)
                {
                    this.vpnTopologyTypeRepository = new GenericRepository<VpnTopologyType>(context);
                }
                return vpnTopologyTypeRepository;
            }
        }

        public GenericRepository<Plane> PlaneRepository
        {
            get
            {
                if (this.planeRepository == null)
                {
                    this.planeRepository = new GenericRepository<Plane>(context);
                }
                return planeRepository;
            }
        }
        public GenericRepository<Vrf> VrfRepository
        {
            get
            {
                if (this.vrfRepository == null)
                {
                    this.vrfRepository = new GenericRepository<Vrf>(context);
                }
                return vrfRepository;
            }
        }

        public GenericRepository<RouteTargetRange> RouteTargetRangeRepository
        {
            get
            {
                if (this.routeTargetRangeRepository == null)
                {
                    this.routeTargetRangeRepository = new GenericRepository<RouteTargetRange>(context);
                }
                return routeTargetRangeRepository;
            }
        }

        public GenericRepository<RouteDistinguisherRange> RouteDistinguisherRangeRepository
        {
            get
            {
                if (this.routeDistinguisherRangeRepository == null)
                {
                    this.routeDistinguisherRangeRepository = new GenericRepository<RouteDistinguisherRange>(context);
                }
                return routeDistinguisherRangeRepository;
            }
        }

        public GenericRepository<VlanTagRange> VlanTagRangeRepository
        {
            get
            {
                if (this.vlanTagRangeRepository == null)
                {
                    this.vlanTagRangeRepository = new GenericRepository<VlanTagRange>(context);
                }
                return vlanTagRangeRepository;
            }
        }

        public async Task<int> SaveAsync()
        {
            return await context.SaveChangesAsync();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
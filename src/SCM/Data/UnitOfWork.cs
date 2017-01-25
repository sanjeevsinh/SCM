using System;
using SCM.Models;
using SCM.Data;
using System.Threading.Tasks;

namespace SCM.Data
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private SigmaContext context;
        private GenericRepository<BgpPeer> bgpPeerRepository;
        private GenericRepository<BundleInterface> bundleInterfaceRepository;
        private GenericRepository<BundleInterfacePort> bundleInterfacePortRepository;
        private GenericRepository<BundleInterfaceVlan> bundleInterfaceVlanRepository;
        private GenericRepository<Device> deviceRepository;
        private GenericRepository<Interface> interfaceRepository;
        private GenericRepository<InterfaceVlan> interfaceVlanRepository;
        private GenericRepository<Location> locationRepository;
        private GenericRepository<InterfaceBandwidth> interfaceBandwidthRepository;
        private GenericRepository<PortBandwidth> portBandwidthRepository;
        private GenericRepository<Port> portRepository;
        private GenericRepository<Region> regionRepository;
        private GenericRepository<RouteTarget> routeTargetRepository;
        private GenericRepository<SubRegion> subRegionRepository;
        private GenericRepository<Tenant> tenantRepository;
        private GenericRepository<TenantNetwork> tenantNetworkRepository;
        private GenericRepository<Vpn> vpnRepository;
        private GenericRepository<VpnProtocolType> vpnProtocolTypeRepository;
        private GenericRepository<VpnTenancyType> vpnTenancyTypeRepository;
        private GenericRepository<VpnTopologyType> vpnTopologyTypeRepository;
        private GenericRepository<Plane> planeRepository;
        private GenericRepository<Vrf> vrfRepository;

        public UnitOfWork(SigmaContext sigmaContext)
        {
            context = sigmaContext;
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

        public GenericRepository<BundleInterface> BundleInterfaceRepository
        {
            get
            {
                if (this.bundleInterfaceRepository == null)
                {
                    this.bundleInterfaceRepository = new GenericRepository<BundleInterface>(context);
                }
                return bundleInterfaceRepository;
            }
        }
        public GenericRepository<BundleInterfacePort> BundleInterfacePortRepository
        {
            get
            {
                if (this.bundleInterfacePortRepository == null)
                {
                    this.bundleInterfacePortRepository = new GenericRepository<BundleInterfacePort>(context);
                }
                return bundleInterfacePortRepository;
            }
        }

        public GenericRepository<BundleInterfaceVlan> BundleInterfaceVlanRepository
        {
            get
            {
                if (this.bundleInterfaceVlanRepository == null)
                {
                    this.bundleInterfaceVlanRepository = new GenericRepository<BundleInterfaceVlan>(context);
                }
                return bundleInterfaceVlanRepository;
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

        public GenericRepository<InterfaceVlan> InterfaceVlanRepository
        {
            get
            {
                if (this.interfaceVlanRepository == null)
                {
                    this.interfaceVlanRepository = new GenericRepository<InterfaceVlan>(context);
                }
                return interfaceVlanRepository;
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
        public GenericRepository<InterfaceBandwidth> InterfaceBandwidthRepository
        {
            get
            {
                if (this.interfaceBandwidthRepository == null)
                {
                    this.interfaceBandwidthRepository = new GenericRepository<InterfaceBandwidth>(context);
                }
                return interfaceBandwidthRepository;
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
using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using System.Threading.Tasks;

namespace SCM.Services.SCMServices
{
    public class InterfaceService : BaseService, IInterfaceService
    {
        public InterfaceService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<Interface> GetByIDAsync(int key)
        {
            return await UnitOfWork.InterfaceRepository.GetByIDAsync(key);
        }

        public async Task<int> AddAsync(Interface iface)
        {
            this.UnitOfWork.InterfaceRepository.Insert(iface);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> UpdateAsync(Interface iface)
        {
            this.UnitOfWork.InterfaceRepository.Update(iface);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(Interface iface)
        {
            this.UnitOfWork.InterfaceRepository.Delete(iface);
            return await this.UnitOfWork.SaveAsync();
        }
      
        /// <summary>
        /// Validates an interface.
        /// </summary>
        /// <param name="iface"></param>
        /// <returns></returns>
        public async Task<ServiceResult> ValidateInterface(Interface iface)
        {
            var validationResult = new ServiceResult();
            validationResult.IsSuccess = true;

            var dbPortResult = await UnitOfWork.PortRepository.GetAsync(q => q.ID == iface.ID, 
                includeProperties: "BundleInterfacePort.BundleInterface", AsTrackable:false);
            var port = dbPortResult.SingleOrDefault();
            
            if (port == null)
            {
                validationResult.Add("The port was not found.");
                validationResult.IsSuccess = false;

                return validationResult;
            }

            if (port.BundleInterfacePort != null)
            {
                validationResult.Add("You cannot create an interface for this port "
                    + "because the port is a member of bundle interface "
                    + port.BundleInterfacePort.BundleInterface.Name);
                validationResult.IsSuccess = false;

                return validationResult;
            }

            if (!iface.IsLayer3)
            {

                var dbVrfResult = await UnitOfWork.VrfRepository.GetAsync(q => q.VrfID == iface.VrfID,
                    includeProperties: "Interfaces.Port,BundleInterfaces,InterfaceVlans.Interface.Port,BundleInterfaceVlans.BundleInterface",
                    AsTrackable: false);
                var vrf = dbVrfResult.SingleOrDefault();

                if (vrf != null)
                {

                    if (vrf.Interfaces.Count() > 0)
                    {
                        var intface = vrf.Interfaces.Single();

                        if (intface.ID != iface.ID)
                        {
                            validationResult.Add("The selected VRF cannot be bound to the interface because the VRF is already bound to interface "
                                + intface.Port.Type + intface.Port.Name);

                            validationResult.IsSuccess = false;
                        }
                    }

                    else if (vrf.BundleInterfaces.Count() > 0)
                    {
                        var bundleIface = vrf.BundleInterfaces.Single();
                        validationResult.Add("The selected VRF cannot be bound to the interface because the VRF is already bound to bundle interface "
                            + bundleIface.Name);

                        validationResult.IsSuccess = false;
                    }

                    else if (vrf.InterfaceVlans.Count() > 0)
                    {
                        var ifaceVlan = vrf.InterfaceVlans.Single();
                        validationResult.Add("The selected VRF cannot be bound to the interface because the VRF is already bound to vlan "
                            + ifaceVlan.VlanTag + " of interface " + ifaceVlan.Interface.Port.Type + ifaceVlan.Interface.Port.Name);

                        validationResult.IsSuccess = false;
                    }

                    else if (vrf.BundleInterfaceVlans.Count() > 0)
                    {
                        var bundleIfaceVlan = vrf.BundleInterfaceVlans.Single();
                        validationResult.Add("The selected VRF cannot be bound to the interface because the VRF is already bound to vlan "
                            + bundleIfaceVlan.VlanTag + " of bundle interface " + bundleIfaceVlan.BundleInterface.Name);

                        validationResult.IsSuccess = false;
                    }
                }
            }

            return validationResult;
        }

        /// <summary>
        /// Validate changes to an interface
        /// </summary>
        /// <param name="iface"></param>
        /// <returns></returns>
        public async Task<ServiceResult> ValidateInterfaceChanges(Interface iface, Interface currentIface)
        {

            var validationResult = new ServiceResult();
            validationResult.IsSuccess = true;

            if (currentIface == null)
            {
                validationResult.Add("Unable to save changes. The interface was deleted by another user.");
                validationResult.IsSuccess = false;

                return validationResult;
            }

            if (!iface.IsTagged)
            {
                if (currentIface.IsTagged && currentIface.InterfaceVlans.Count > 0)
                {
                    validationResult.Add("You cannot change this interface to untagged because "
                    + "there are vlans configured. Delete the vlans first.");
                    validationResult.IsSuccess = false;

                    return validationResult;
                }
            }

            return await ValidateInterface(iface);
        }
    }
}
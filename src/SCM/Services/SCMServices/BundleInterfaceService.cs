using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using System.Threading.Tasks;

namespace SCM.Services.SCMServices
{
    public class BundleInterfaceService : BaseService, IBundleInterfaceService
    {
        public BundleInterfaceService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<BundleInterface> GetByIDAsync(int key)
        {
            return await UnitOfWork.BundleInterfaceRepository.GetByIDAsync(key);
        }

        public async Task<int> AddAsync(BundleInterface bundleIfacePort)
        {
            this.UnitOfWork.BundleInterfaceRepository.Insert(bundleIfacePort);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> UpdateAsync(BundleInterface bundleIfacePort)
        {
            this.UnitOfWork.BundleInterfaceRepository.Update(bundleIfacePort);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(BundleInterface bundleIfacePort)
        {
            this.UnitOfWork.BundleInterfaceRepository.Delete(bundleIfacePort);
            return await this.UnitOfWork.SaveAsync();
        }
        /// <summary>
        /// Validates a new bundle interface.
        /// </summary>
        /// <param name="iface"></param>
        /// <returns></returns>
        public async Task<ServiceResult> ValidateBundleInterface(BundleInterface bundleIface)
        {
            var validationResult = new ServiceResult();
            validationResult.IsSuccess = true;

            var dbVrfResult = await UnitOfWork.VrfRepository.GetAsync(q => q.VrfID == bundleIface.VrfID,
                includeProperties: "Interfaces.Port,BundleInterfaces,InterfaceVlans.Interface.Port,BundleInterfaceVlans.BundleInterface",
                AsTrackable: false);
            var vrf = dbVrfResult.SingleOrDefault();

            if (vrf != null)
            {

                if (vrf.Interfaces.Count() > 0)
                {
                    var intface = vrf.Interfaces.Single();
                    validationResult.Add("The selected VRF cannot be bound to the interface because the VRF is already bound to interface "
                        + intface.Port.Type + intface.Port.Name);

                    validationResult.IsSuccess = false;
                }

                else if (vrf.BundleInterfaces.Count() > 0)
                {
                    var bundleIntface = vrf.BundleInterfaces.Single();
                    if (bundleIface.BundleInterfaceID != bundleIntface.BundleInterfaceID)
                    {
                        validationResult.Add("The selected VRF cannot be bound to the interface because the VRF is already bound to bundle interface "
                            + bundleIntface.Name);

                        validationResult.IsSuccess = false;
                    }
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

            return validationResult;
        }
        /// <summary>
        /// Validate changes to a bundle interface
        /// </summary>
        /// <param name="bundleIface"></param>
        /// <returns></returns>
        public async Task<ServiceResult> ValidateBundleInterfaceChanges(BundleInterface bundleIface, 
            BundleInterface currentBundleIface)
        {

            var validationResult = new ServiceResult();
            validationResult.IsSuccess = true;

            if (currentBundleIface == null)
            {
                validationResult.Add("Unable to save changes. The bundle interface was deleted by another user.");
                validationResult.IsSuccess = false;

                return validationResult;
            }

            if (!bundleIface.IsTagged)
            {
                if (currentBundleIface.IsTagged && currentBundleIface.BundleInterfaceVlans.Count > 0)
                {
                    validationResult.Add("You cannot change this interface to untagged because "
                    + "there are vlans configured. Delete the vlans first.");
                    validationResult.IsSuccess = false;

                    return validationResult;
                }
            }

            return await ValidateBundleInterface(bundleIface);
        }
    }  
}
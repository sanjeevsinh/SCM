using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using System.Threading.Tasks;

namespace SCM.Services.SCMServices
{
    public class BundleInterfaceVlanService : BaseService, IBundleInterfaceVlanService
    {
        public BundleInterfaceVlanService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<BundleInterfaceVlan> GetByIDAsync(int key)
        {
            return await UnitOfWork.BundleInterfaceVlanRepository.GetByIDAsync(key);
        }

        public async Task<int> AddAsync(BundleInterfaceVlan bundleIfaceVlan)
        {
            this.UnitOfWork.BundleInterfaceVlanRepository.Insert(bundleIfaceVlan);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> UpdateAsync(BundleInterfaceVlan bundleIfaceVlan)
        {
            this.UnitOfWork.BundleInterfaceVlanRepository.Update(bundleIfaceVlan);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(BundleInterfaceVlan bundleIfaceVlan)
        {
            this.UnitOfWork.BundleInterfaceVlanRepository.Delete(bundleIfaceVlan);
            return await this.UnitOfWork.SaveAsync();
        }

        /// <summary>
        /// Validates a new bundle interface vlan.
        /// </summary>
        /// <param name="bundleIfaceVlan"></param>
        /// <returns></returns>
        public async Task<ServiceValidationResult> ValidateBundleInterfaceVlan(BundleInterfaceVlan bundleIfaceVlan)
        {
            var validationResult = new ServiceValidationResult();
            validationResult.IsValid = true;

            var dbBundleInterfaceResult = await UnitOfWork.BundleInterfaceRepository.GetAsync(q => q.BundleInterfaceID == bundleIfaceVlan.BundleInterfaceID, 
                AsTrackable:false);
            var bundleIface = dbBundleInterfaceResult.SingleOrDefault();

            if (bundleIface == null)
            {
                validationResult.Add("The bundle inteface was not found.");
                validationResult.IsValid = false;

                return validationResult;
            }

            if (!bundleIface.IsTagged)
            {
                validationResult.Add("A vlan cannot be created for an untagged bundle interface.");
                validationResult.IsValid = false;

                return validationResult;
            }

            var dbVrfResult = await UnitOfWork.VrfRepository.GetAsync(q => q.VrfID == bundleIfaceVlan.VrfID,
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

                    validationResult.IsValid = false;
                }

                else if (vrf.BundleInterfaces.Count() > 0)
                {
                    var bundleIntface = vrf.BundleInterfaces.Single();
                    validationResult.Add("The selected VRF cannot be bound to the interface because the VRF is already bound to bundle interface "
                        + bundleIntface.Name);

                    validationResult.IsValid = false;
                }

                else if (vrf.InterfaceVlans.Count() > 0)
                {
                    var intfaceVlan = vrf.InterfaceVlans.Single();
                    validationResult.Add("The selected VRF cannot be bound to the interface because the VRF is already bound to vlan "
                        + intfaceVlan.VlanTag + " of interface " + intfaceVlan.Interface.Port.Type + intfaceVlan.Interface.Port.Name);

                    validationResult.IsValid = false;
                }

                else if (vrf.BundleInterfaceVlans.Count() > 0)
                {
                    var bundleIntfaceVlan = vrf.BundleInterfaceVlans.Single();

                    if (bundleIfaceVlan.BundleInterfaceVlanID != bundleIntfaceVlan.BundleInterfaceVlanID)
                    {
                        validationResult.Add("The selected VRF cannot be bound to the interface because the VRF is already bound to vlan "
                            + bundleIfaceVlan.VlanTag + " of bundle interface " + bundleIntfaceVlan.BundleInterface.Name);

                        validationResult.IsValid = false;
                    }
                }
            }

            return validationResult;
        }

        /// <summary>
        /// Validate changes to an bundle interface vlan
        /// </summary>
        /// <param name="bundleIfaceVlan"></param>
        /// <param name="currentBundleIfaceVlan"></param>
        /// <returns></returns>
        public async Task<ServiceValidationResult> ValidateBundleInterfaceVlanChanges(BundleInterfaceVlan bundleIfaceVlan, BundleInterfaceVlan currentBundleIfaceVlan)
        {

            var validationResult = new ServiceValidationResult();
            validationResult.IsValid = true;

            if (currentBundleIfaceVlan == null)
            {
                validationResult.Add("Unable to save changes. The vlan was deleted by another user.");
                validationResult.IsValid = false;

                return validationResult;
            }

            return await ValidateBundleInterfaceVlan(bundleIfaceVlan);
        }
    }
}
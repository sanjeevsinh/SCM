using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using System.Threading.Tasks;

namespace SCM.Services.SCMServices
{
    public class InterfaceVlanService : BaseService, IInterfaceVlanService
    {
        public InterfaceVlanService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<InterfaceVlan> GetByIDAsync(int key)
        {
            return await UnitOfWork.InterfaceVlanRepository.GetByIDAsync(key);
        }

        public async Task<int> AddAsync(InterfaceVlan ifaceVlan)
        {
            this.UnitOfWork.InterfaceVlanRepository.Insert(ifaceVlan);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> UpdateAsync(InterfaceVlan ifaceVlan)
        {
            this.UnitOfWork.InterfaceVlanRepository.Update(ifaceVlan);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(InterfaceVlan ifaceVlan)
        {
            this.UnitOfWork.InterfaceVlanRepository.Delete(ifaceVlan);
            return await this.UnitOfWork.SaveAsync();
        }

        /// <summary>
        /// Validates a new interface vlan.
        /// </summary>
        /// <param name="ifaceVlan"></param>
        /// <returns></returns>
        public async Task<ServiceResult> ValidateInterfaceVlan(InterfaceVlan ifaceVlan)
        {
            var validationResult = new ServiceResult();
            validationResult.IsSuccess = true;

            var dbInterfaceResult = await UnitOfWork.InterfaceRepository.GetAsync(q => q.ID == ifaceVlan.InterfaceID, includeProperties: "Port");
            var iface = dbInterfaceResult.SingleOrDefault();

            if (iface == null)
            {
                validationResult.Add("The inteface was not found.");
                validationResult.IsSuccess = false;

                return validationResult;
            }

            if (!iface.IsTagged)
            {
                validationResult.Add("A vlan cannot be created for an untagged interface.");
                validationResult.IsSuccess = false;

                return validationResult;
            }

            var dbVrfResult = await UnitOfWork.VrfRepository.GetAsync(q => q.VrfID == ifaceVlan.VrfID,
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
                    var bundleIface = vrf.BundleInterfaces.Single();
                    validationResult.Add("The selected VRF cannot be bound to the interface because the VRF is already bound to bundle interface "
                        + bundleIface.Name);

                    validationResult.IsSuccess = false;
                }

                else if (vrf.InterfaceVlans.Count() > 0)
                {
                    var intfaceVlan = vrf.InterfaceVlans.Single();

                    if (ifaceVlan.InterfaceVlanID != intfaceVlan.InterfaceVlanID)
                    {
                        validationResult.Add("The selected VRF cannot be bound to the interface because the VRF is already bound to vlan "
                            + intfaceVlan.VlanTag + " of interface " + intfaceVlan.Interface.Port.Type + intfaceVlan.Interface.Port.Name);

                        validationResult.IsSuccess = false;
                    }
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
        /// Validate changes to an interface vlan
        /// </summary>
        /// <param name="ifaceVlan"></param>
        /// <param name="currentIfaceVlan"></param>
        /// <returns></returns>
        public async Task<ServiceResult> ValidateInterfaceVlanChanges(InterfaceVlan ifaceVlan, InterfaceVlan currentIfaceVlan)
        {

            var validationResult = new ServiceResult();
            validationResult.IsSuccess = true;

            if (currentIfaceVlan == null)
            {
                validationResult.Add("Unable to save changes. The vlan was deleted by another user.");
                validationResult.IsSuccess = false;

                return validationResult;
            }

            return await ValidateInterfaceVlan(ifaceVlan);
        }
    }
}
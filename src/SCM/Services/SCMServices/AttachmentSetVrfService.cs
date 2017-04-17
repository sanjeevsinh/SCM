using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using SCM.Models.ServiceModels;
using System.Threading.Tasks;

namespace SCM.Services.SCMServices
{
    public class AttachmentSetVrfService : BaseService, IAttachmentSetVrfService
    {
        public AttachmentSetVrfService(IUnitOfWork unitOfWork, 
            IAttachmentService attachmentService, IVifService vifService) : base(unitOfWork)
        {
            AttachmentService = attachmentService;
            VifService = vifService;
        }

        private IAttachmentService AttachmentService { get; set; }
        private IVifService VifService { get; set; }

        public async Task<IEnumerable<AttachmentSetVrf>> GetAllAsync()
        {
            return await this.UnitOfWork.AttachmentSetVrfRepository.GetAsync();
        }

        public async Task<IEnumerable<AttachmentSetVrf>> GetAllByAttachmentSetID(int id)
        {
            return await UnitOfWork.AttachmentSetVrfRepository.GetAsync(q => q.AttachmentSetID == id,
               includeProperties: "Vrf.Device.Location.SubRegion.Region,"
               + "Vrf.Device.Plane,"
               + "Vrf.Tenant,"
               + "Vrf.Interfaces.Port,"
               + "Vrf.Interfaces.ContractBandwidthPool,"
               + "Vrf.InterfaceVlans.Interface.Port,"
               + "Vrf.InterfaceVlans.ContractBandwidthPool,"
               + "Vrf.MultiPorts.ContractBandwidthPool,"
               + "Vrf.MultiPortVlans.MultiPort,"
               + "Vrf.MultiPortVlans.ContractBandwidthPool");
        }

        public async Task<AttachmentSetVrf> GetByIDAsync(int id)
        {
            var dbResult = await UnitOfWork.AttachmentSetVrfRepository.GetAsync(q => q.AttachmentSetVrfID == id, includeProperties:
               "Vrf.Device.Location.SubRegion.Region,"
               + "Vrf.Device.Plane,"
               + "Vrf.Tenant,"
               + "Vrf.Interfaces.Port,"
               + "Vrf.Interfaces.ContractBandwidthPool,"
               + "Vrf.InterfaceVlans.Interface.Port,"
               + "Vrf.InterfaceVlans.ContractBandwidthPool,"
               + "Vrf.MultiPorts.ContractBandwidthPool,"
               + "Vrf.MultiPortVlans.MultiPort,"
               + "Vrf.MultiPortVlans.ContractBandwidthPool");

            return dbResult.SingleOrDefault();
        }

        public async Task<int> AddAsync(AttachmentSetVrf attachmentSetVrf)
        {
            this.UnitOfWork.AttachmentSetVrfRepository.Insert(attachmentSetVrf);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> UpdateAsync(AttachmentSetVrf attachmentSetVrf)
        {
            this.UnitOfWork.AttachmentSetVrfRepository.Update(attachmentSetVrf);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(AttachmentSetVrf attachmentSetVrf)
        {
            this.UnitOfWork.AttachmentSetVrfRepository.Delete(attachmentSetVrf);
            return await this.UnitOfWork.SaveAsync();
        }

        /// <summary>
        /// Get a collection of VRFs which can be used to satisfy an Attachment Set VRF request.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Vrf>> GetCandidateVrfs(AttachmentSetVrfRequest request)
        {
            var attachmentSet = await UnitOfWork.AttachmentSetRepository.GetByIDAsync(request.AttachmentSetID);

            var vrfs = await UnitOfWork.VrfRepository.GetAsync(q => q.Device.LocationID == request.LocationID && q.TenantID == request.TenantID,
                includeProperties:"Device");

            // Filter vrfs by plane if plane is specified

            if (request.PlaneID != null)
            {
                vrfs = vrfs.Where(q => q.Device.PlaneID == request.PlaneID).ToList();
            }

            return vrfs;
        }

        public async Task<ServiceResult> ValidateAsync(AttachmentSetVrf attachmentSetVrf)
        {
            var result = new ServiceResult { IsSuccess = true };

            var vpnAttachmentSets = await UnitOfWork.VpnAttachmentSetRepository.GetAsync(q => q.AttachmentSetID == attachmentSetVrf.AttachmentSetID,
                includeProperties: "Vpn", AsTrackable: false);
            if (vpnAttachmentSets.Count() > 0)
            {
                result.Add("The VRF cannot be added because the attachment set is bound to the following VPNs: ");
                result.AddRange(vpnAttachmentSets.Select(v => v.Vpn.Name + "."));
                result.Add("Remove the Attachment Set from the VPNs first.");
                result.IsSuccess = false;

                return result;
            }

            var attachmentSet = await UnitOfWork.AttachmentSetRepository.GetByIDAsync(attachmentSetVrf.AttachmentSetID);
            var vrfDbResult = await UnitOfWork.VrfRepository.GetAsync(q => q.VrfID == attachmentSetVrf.VrfID, 
                includeProperties:"Interfaces,"
                + "InterfaceVlans,"
                + "MultiPorts,"
                + "MultiPortVlans");

            var vrf = vrfDbResult.Single();

            if (vrf.MultiPorts.Count > 0)
            {
                if (vrf.MultiPorts.Where(q => q.IsLayer3 != attachmentSet.IsLayer3).Count() > 0)
                {
                    result.Add($"VRF '{vrf.Name}' cannot be added to attachment set '{attachmentSet.Name}'.");
                    result.Add("The protocol layer of the attachment set and the VRF do not match.");
                    result.IsSuccess = false;

                    return result;
                }
            }

            if (vrf.MultiPortVlans.Count > 0)
            {
                if (vrf.MultiPortVlans.Where(q => q.IsLayer3 != attachmentSet.IsLayer3).Count() > 0)
                {
                    result.Add($"VRF '{vrf.Name}' cannot be added to attachment set '{attachmentSet.Name}'.");
                    result.Add("The protocol layer of the attachment set and the VRF do not match.");
                    result.IsSuccess = false;

                    return result;
                }
            }

            if (vrf.Interfaces.Count > 0)
            {
                if (vrf.Interfaces.Where(q => q.IsLayer3 != attachmentSet.IsLayer3).Count() > 0) {
                    result.Add($"VRF '{vrf.Name}' cannot be added to attachment set '{attachmentSet.Name}'.");
                    result.Add("The protocol layer of the attachment set and the VRF do not match.");
                    result.IsSuccess = false;

                    return result;
                }
            }

            if (vrf.InterfaceVlans.Count > 0)
            {
                if (vrf.InterfaceVlans.Where(q => q.IsLayer3 != attachmentSet.IsLayer3).Count() > 0)
                {
                    result.Add($"VRF '{vrf.Name}' cannot be added to attachment set '{attachmentSet.Name}'.");
                    result.Add("The protocol layer of the attachment set and the VRF do not match.");
                    result.IsSuccess = false;

                    return result;
                }
            }

            return result;
        }

        public async Task<ServiceResult> ValidateDeleteAsync(AttachmentSetVrf attachmentSetVrf)
        {
            var result = new ServiceResult { IsSuccess = true };

            var vpnAttachmentSets = await UnitOfWork.VpnAttachmentSetRepository.GetAsync(q => q.AttachmentSetID == attachmentSetVrf.AttachmentSetID,
                includeProperties: "Vpn", AsTrackable: false);
            if (vpnAttachmentSets.Count() > 0)
            {
                result.Add("The VRF cannot be removed because the attachment set is bound to the following VPNs: ");
                result.AddRange(vpnAttachmentSets.Select(v => v.Vpn.Name + "."));
                result.Add("Remove the Attachment Set from the VPNs first.");
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<ServiceResult> CheckVrfsConfiguredCorrectlyAsync(AttachmentSet attachmentSet)
        {

            var validationResult = new ServiceResult { IsSuccess = true };

            var attachmentSetVrfs = await this.UnitOfWork.AttachmentSetVrfRepository.GetAsync(q => q.AttachmentSetID == attachmentSet.AttachmentSetID,
                includeProperties: "Vrf.Device.Location.SubRegion.Region,Vrf.Device.Plane");

            var attachmentRedundancy = attachmentSet.AttachmentRedundancy.Name;

            if (attachmentRedundancy == "Bronze")
            {
                if (attachmentSetVrfs.Count != 1)
                {
                    validationResult.Add($"One, and no more than one, VRF for bronze attachment set '{attachmentSet.Name}' must be defined.");
                    validationResult.IsSuccess = false;
                }
            }
            else
            {
                if (attachmentRedundancy == "Silver")
                {
                    if (attachmentSetVrfs.Count != 2)
                    {
                        validationResult.Add($"Two, and no more than two, VRFs for silver attachment set '{attachmentSet.Name}' must be defined. "
                            + "Each VRF must be in the same location.");
                        validationResult.IsSuccess = false;
                        return validationResult;
                    }

                    var locationA = attachmentSetVrfs[0].Vrf.Device.Location;
                    var locationB = attachmentSetVrfs[1].Vrf.Device.Location;
                    var planeA = attachmentSetVrfs[0].Vrf.Device.Plane;
                    var planeB = attachmentSetVrfs[1].Vrf.Device.Plane;

                    if (locationA.LocationID != locationB.LocationID)
                    {
                        validationResult.Add($"The location for each VRF in silver attachment set '{attachmentSet.Name}' must be the same.");
                        validationResult.IsSuccess = false;
                    }

                    if (planeA.PlaneID == planeB.PlaneID)
                    {
                        validationResult.Add($"The plane for each VRF in silver attachment set '{attachmentSet.Name}' must be different.");
                        validationResult.IsSuccess = false;
                    }
                }

                else if (attachmentRedundancy == "Gold")
                {

                    if (attachmentSetVrfs.Count != 2)
                    {
                        validationResult.Add($"Two, and no more than two, VRFs for gold attachment set '{attachmentSet.Name}' must be defined. "
                            + "Each VRF must be in a different location.");
                        validationResult.IsSuccess = false;
                        return validationResult;
                    }

                    var locationA = attachmentSetVrfs[0].Vrf.Device.Location;
                    var locationB = attachmentSetVrfs[1].Vrf.Device.Location;
                    var planeA = attachmentSetVrfs[0].Vrf.Device.Plane;
                    var planeB = attachmentSetVrfs[1].Vrf.Device.Plane;
                    var subRegionA = locationA.SubRegion;
                    var subRegionB = locationB.SubRegion;

                    if (subRegionA.SubRegionID != subRegionB.SubRegionID)
                    {
                        validationResult.Add($"The sub-region for each VRF in gold attachment set '{attachmentSet.Name}' must be the same.");
                        validationResult.IsSuccess = false;
                    }

                    if (locationA.LocationID == locationB.LocationID)
                    {
                        validationResult.Add($"The location for each VRF in gold attachment set '{attachmentSet.Name}' must be different.");
                        validationResult.IsSuccess = false;
                    }

                    if (planeA.PlaneID == planeB.PlaneID)
                    {
                        validationResult.Add($"The plane for each VRF in gold attachment set '{attachmentSet.Name}' must be different.");
                        validationResult.IsSuccess = false;
                    }
                }
                else if (attachmentRedundancy == "Custom")
                {

                    if (attachmentSetVrfs.Count == 0)
                    {
                        validationResult.Add($"At least one VRF is required for custom attachment set '{attachmentSet.Name}'.");
                        validationResult.IsSuccess = false;
                    }
                }
            }

            return validationResult;
        }
    }
}
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
            IAttachmentSetService attachmentSetService) : base(unitOfWork)
        {
            AttachmentSetService = attachmentSetService;
        }

        private IAttachmentSetService AttachmentSetService { get; set; }

        public async Task<IEnumerable<AttachmentSetVrf>> GetAllAsync()
        {
            return await this.UnitOfWork.AttachmentSetVrfRepository.GetAsync();
        }

        public async Task<IEnumerable<AttachmentSetVrf>> GetAllByAttachmentSetID(int id)
        {
            return await UnitOfWork.AttachmentSetVrfRepository.GetAsync(q => q.AttachmentSetID == id,
               includeProperties: "AttachmentSet,"
               + "Vrf.Device.Location.SubRegion.Region,"
               + "Vrf.Device.Plane,"
               + "Vrf.Tenant,"
               + "Vrf.Attachments.ContractBandwidthPool,"
               + "Vrf.Attachments.Interfaces.Ports,"
               + "Vrf.Vifs.ContractBandwidthPool,"
               + "Vrf.Vifs.Attachment.Interfaces.Ports", AsTrackable: false);
        }

        public async Task<AttachmentSetVrf> GetByIDAsync(int id)
        {
            var dbResult = await UnitOfWork.AttachmentSetVrfRepository.GetAsync(q => q.AttachmentSetVrfID == id,
                includeProperties: "AttachmentSet,"
               + "Vrf.Device.Location.SubRegion.Region,"
               + "Vrf.Device.Plane,"
               + "Vrf.Tenant,"
               + "Vrf.Attachments.ContractBandwidthPool,"
               + "Vrf.Attachments.Interfaces.Ports,"
               + "Vrf.Vifs.ContractBandwidthPool,"
               + "Vrf.Vifs.Attachment.Interfaces.Ports", AsTrackable: false);

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

        /// <summary>
        /// Validate a new Attachment Set VRF request
        /// </summary>
        /// <param name="attachmentSetVrf"></param>
        /// <returns></returns>
        public async Task<ServiceResult> ValidateNewAsync(AttachmentSetVrf attachmentSetVrf)
        {
            var result = new ServiceResult { IsSuccess = true };

            var attachmentSet = await UnitOfWork.AttachmentSetRepository.GetByIDAsync(attachmentSetVrf.AttachmentSetID);
            var vrfDbResult = await UnitOfWork.VrfRepository.GetAsync(q => q.VrfID == attachmentSetVrf.VrfID, 
                includeProperties:"Attachments,"
                + "Vifs");

            var vrf = vrfDbResult.Single();

            if (vrf.Attachments.Count > 0)
            {
                if (vrf.Attachments.Where(q => q.IsLayer3 != attachmentSet.IsLayer3).Count() > 0)
                {
                    result.Add($"VRF '{vrf.Name}' cannot be added to attachment set '{attachmentSet.Name}'.");
                    result.Add("The protocol layer of the attachment set and the VRF do not match.");
                    result.IsSuccess = false;

                    return result;
                }
            }

            if (vrf.Vifs.Count > 0)
            {
                if (vrf.Vifs.Where(q => q.IsLayer3 != attachmentSet.IsLayer3).Count() > 0)
                {
                    result.Add($"VRF '{vrf.Name}' cannot be added to attachment set '{attachmentSet.Name}'.");
                    result.Add("The protocol layer of the attachment set and the VRF do not match.");
                    result.IsSuccess = false;

                    return result;
                }
            }

            return result;
        }

        /// <summary>
        /// Validate all Attachment Set VRFs for a given VPN.
        /// </summary>
        /// <param name="vpn"></param>
        /// <returns></returns>
        public async Task<ServiceResult> CheckVrfsConfiguredCorrectlyAsync(Vpn vpn)
        {
            var result = new ServiceResult { IsSuccess = true };
            var tasks = new List<Task<ServiceResult>>();

            var attachmentSets = await AttachmentSetService.GetAllByVpnIDAsync(vpn.VpnID);

            foreach (var attachmentSet in attachmentSets)
            {
                tasks.Add(CheckVrfsConfiguredCorrectlyAsync(attachmentSet));
            }

            await Task.WhenAll(tasks);

            foreach (var t in tasks)
            {
                if (!t.Result.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.AddRange(t.Result.GetMessageList());
                }
            }

            return result;
        }

        public async Task<ServiceResult> CheckVrfsConfiguredCorrectlyAsync(AttachmentSet attachmentSet)
        {
            var validationResult = new ServiceResult { IsSuccess = true };

            var attachmentSetVrfs = await this.UnitOfWork.AttachmentSetVrfRepository.GetAsync(q => q.AttachmentSetID == attachmentSet.AttachmentSetID,
                includeProperties: "Vrf.Device.Location.SubRegion.Region,"
                + "Vrf.Device.Plane");

            var attachmentRedundancy = attachmentSet.AttachmentRedundancy.Name;

            if (attachmentRedundancy == "Bronze")
            {
                if (attachmentSetVrfs.Count != 1)
                {
                    validationResult.Add($"One, and no more than one, VRF for bronze attachment set '{attachmentSet.Name}' " 
                       + $"belonging to tenant '{attachmentSet.Tenant.Name}' must be defined.");
                    validationResult.IsSuccess = false;
                }
            }
            else
            {
                if (attachmentRedundancy == "Silver")
                {
                    if (attachmentSetVrfs.Count != 2)
                    {
                        validationResult.Add($"Two, and no more than two, VRFs for silver attachment set '{attachmentSet.Name}' " 
                            + $"belonging to tenant '{attachmentSet.Tenant.Name}' must be defined. "
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
                        validationResult.Add($"The location for each VRF in silver attachment set '{attachmentSet.Name}' " 
                            + $"belonging to tenant '{attachmentSet.Tenant.Name}' must be the same.");
                        validationResult.IsSuccess = false;
                    }

                    if (planeA.PlaneID == planeB.PlaneID)
                    {
                        validationResult.Add($"The plane for each VRF in silver attachment set '{attachmentSet.Name}' " 
                            + $"belonging to tenant '{attachmentSet.Tenant.Name}'must be different.");
                        validationResult.IsSuccess = false;
                    }
                }

                else if (attachmentRedundancy == "Gold")
                {

                    if (attachmentSetVrfs.Count != 2)
                    {
                        validationResult.Add($"Two, and no more than two, VRFs for gold attachment set '{attachmentSet.Name}' " 
                            + $"belonging to tenant '{attachmentSet.Tenant.Name}' must be defined. "
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
                        validationResult.Add($"The sub-region for each VRF in gold attachment set '{attachmentSet.Name}' " 
                            + $"belonging to tenant '{attachmentSet.Tenant.Name}' must be the same.");
                        validationResult.IsSuccess = false;
                    }

                    if (locationA.LocationID == locationB.LocationID)
                    {
                        validationResult.Add($"The location for each VRF in gold attachment set '{attachmentSet.Name}' "
                            + $"belonging to tenant '{attachmentSet.Tenant.Name}' must be different.");
                        validationResult.IsSuccess = false;
                    }

                    if (planeA.PlaneID == planeB.PlaneID)
                    {
                        validationResult.Add($"The plane for each VRF in gold attachment set '{attachmentSet.Name}' "
                            + $"belonging to tenant '{attachmentSet.Tenant.Name}' must be different.");
                        validationResult.IsSuccess = false;
                    }
                }
                else if (attachmentRedundancy == "Custom")
                {

                    if (attachmentSetVrfs.Count == 0)
                    {
                        validationResult.Add($"At least one VRF is required for custom attachment set '{attachmentSet.Name}' " 
                            + $"belonging to tenant '{attachmentSet.Tenant.Name}'.");
                        validationResult.IsSuccess = false;
                    }
                }
            }

            return validationResult;
        }
    }
}
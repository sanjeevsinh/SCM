using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using SCM.Models.ServiceModels;
using System.Threading.Tasks;
using AutoMapper;

namespace SCM.Services.SCMServices
{
    public class VifService : BaseService, IVifService
    {
        public VifService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public async Task<Vif> GetByIDAsync(int id)
        {
            var dbResult = await UnitOfWork.InterfaceVlanRepository.GetAsync(q => q.InterfaceVlanID == id, includeProperties:
                "Interface.Port,Vrf,ContractBandwidthPool.ContractBandwidth,Tenant",
                AsTrackable: false);

            return Mapper.Map<Vif>(dbResult.SingleOrDefault());
        }

        public async Task<List<Vif>> GetAllByAttachmentIDAsync(int id)
        {
            var vifs = await UnitOfWork.InterfaceVlanRepository.GetAsync(q => q.InterfaceID == id,
                includeProperties: "Interface.Port,Vrf,ContractBandwidthPool.ContractBandwidth,Tenant", AsTrackable: false);

            return Mapper.Map<List<Vif>>(vifs);
        }

        public async Task<ServiceResult> AddAsync(VifRequest request)
        {
            var result = new ServiceResult { IsSuccess = true };

            var ifaceVlan = Mapper.Map<InterfaceVlan>(request);
            this.UnitOfWork.InterfaceVlanRepository.Insert(ifaceVlan);

            await this.UnitOfWork.SaveAsync();

            return result;
        }

        public async Task<int> DeleteAsync(Vif vif)
        {
            var ifaceVlan = Mapper.Map<InterfaceVlan>(vif);
            this.UnitOfWork.InterfaceVlanRepository.Delete(ifaceVlan);
            return await this.UnitOfWork.SaveAsync();
        }

        /// <summary>
        /// Validates a Vif.
        /// </summary>
        /// <param name="ifaceVlan"></param>
        /// <returns></returns>
        public async Task<ServiceResult> ValidateAsync(VifRequest request)
        {
            var validationResult = new ServiceResult { IsSuccess = true };

            var dbInterfaceResult = await UnitOfWork.InterfaceRepository.GetAsync(q => q.InterfaceID == request.AttachmentID, includeProperties: "Port");
            var iface = dbInterfaceResult.SingleOrDefault();

            if (iface == null)
            {
                validationResult.Add("The interface was not found.");
                validationResult.IsSuccess = false;

                return validationResult;
            }

            if (!iface.IsTagged)
            {
                validationResult.Add("A vif cannot be created for an untagged interface.");
                validationResult.IsSuccess = false;

                return validationResult;
            }

            return validationResult;
        }
    }
}
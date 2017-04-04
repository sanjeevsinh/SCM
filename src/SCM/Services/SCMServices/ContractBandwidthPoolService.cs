using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Models.ServiceModels;
using SCM.Data;
using AutoMapper;

namespace SCM.Services.SCMServices
{
    public class ContractBandwidthPoolService : BaseService, IContractBandwidthPoolService
    {
        public ContractBandwidthPoolService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public async Task<IEnumerable<ContractBandwidthPool>> GetAllAsync()
        {
            return await this.UnitOfWork.ContractBandwidthPoolRepository.GetAsync();
        }

        public async Task<ContractBandwidthPool> GetByIDAsync(int id)
        {
            return await this.UnitOfWork.ContractBandwidthPoolRepository.GetByIDAsync(id);
        }

        public async Task<int> AddAsync(ContractBandwidthPool contractBandwidthPool)
        {
            this.UnitOfWork.ContractBandwidthPoolRepository.Insert(contractBandwidthPool);
            return await this.UnitOfWork.SaveAsync();
        }
 
        public async Task<int> UpdateAsync(ContractBandwidthPool contractBandwidthPool)
        {
            this.UnitOfWork.ContractBandwidthPoolRepository.Update(contractBandwidthPool);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(ContractBandwidthPool contractBandwidthPool)
        {
            this.UnitOfWork.ContractBandwidthPoolRepository.Delete(contractBandwidthPool);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<ServiceResult> ValidateAsync(int contractBandwidthPoolID)
        {
            var result = new ServiceResult { IsSuccess = true };

            var dbResult = await UnitOfWork.ContractBandwidthPoolRepository.GetAsync(q =>
                 q.ContractBandwidthPoolID == contractBandwidthPoolID,
                 includeProperties: "ContractBandwidth,Interfaces.Port,InterfaceVlans.Interface.Port,MultiPorts,MultiPortVlans.MultiPort");

            var contractBandwidthPool = dbResult.SingleOrDefault();

            if (contractBandwidthPool == null)
            {
                result.Add("The requested contract bandwidth pool was not found.");
                result.IsSuccess = false;

                return result;
            }

            if (contractBandwidthPool.Interfaces.Count > 0)
            {
                result.Add("The requested contract bandwidth pool cannot be used because it is already in use for the following attachments :");
                var attachments = Mapper.Map<List<Attachment>>(contractBandwidthPool.Interfaces.ToList());
                result.AddRange(attachments.Select(q => $"{q.Vrf.Device.Name}, {q.Name}"));
                result.IsSuccess = false;

                return result;
            }

            if (contractBandwidthPool.InterfaceVlans.Count > 0)
            {
                result.Add("The requested contract bandwidth pool cannot be used because it is already in use for the following vifs :");
                var vifs = Mapper.Map<List<Vif>>(contractBandwidthPool.InterfaceVlans.ToList());
                result.AddRange(vifs.Select(q => $"{q.Vrf.Device.Name}, {q.Name}"));
                result.IsSuccess = false;

                return result;
            }

            if (contractBandwidthPool.MultiPorts.Count > 0)
            {
                result.Add("The requested contract bandwidth pool cannot be used because it is already in use for the following multi-port attachments :");
                var attachments = Mapper.Map<List<Attachment>>(contractBandwidthPool.MultiPorts.ToList());
                result.AddRange(attachments.Select(q => $"{q.Vrf.Device.Name}, {q.Name}"));
                result.IsSuccess = false;
            }

            if (contractBandwidthPool.MultiPortVlans.Count > 0)
            {
                result.Add("The requested contract bandwidth pool cannot be used because it is already in use for the following multi-port vifs :");
                var attachments = Mapper.Map<List<Attachment>>(contractBandwidthPool.Interfaces.ToList());
                result.AddRange(attachments.Select(q => $"{q.Vrf.Device.Name}, {q.Name}"));
                result.IsSuccess = false;
            }

            return result;
        }

        public ServiceResult ValidateDelete(ContractBandwidthPool contractBandwidthPool)
        {
            var result = new ServiceResult { IsSuccess = true };
            if (contractBandwidthPool.Interfaces.Count() > 0)
            {
                result.Add("The contract bandwidth pool cannot be deleted because the following attachments which reference the pool are defined:");
                var attachments = Mapper.Map<List<Attachment>>(contractBandwidthPool.Interfaces.ToList());
                result.AddRange(attachments.Select(q => $"{q.Vrf.Device.Name}, {q.Name}"));
                result.IsSuccess = false;
            }

            else if (contractBandwidthPool.InterfaceVlans.Count() > 0)
            {
                result.Add("The contract bandwidth pool cannot be deleted because the following vifs which reference the pool are defined:");
                var vifs = Mapper.Map<List<Vif>>(contractBandwidthPool.InterfaceVlans.ToList());
                result.AddRange(vifs.Select(q => $"{q.Vrf.Device.Name}, {q.Name}"));
                result.IsSuccess = false;
            }

            else if (contractBandwidthPool.MultiPorts.Count() > 0)
            {
                result.Add("The contract bandwidth pool cannot be deleted because the following multi-port attachments which reference the pool are defined:");
                var attachments = Mapper.Map<List<Attachment>>(contractBandwidthPool.MultiPorts.ToList());
                result.AddRange(attachments.Select(q => $"{q.Vrf.Device.Name}, {q.Name}"));
                result.IsSuccess = false;
            }

            else if (contractBandwidthPool.MultiPortVlans.Count() > 0)
            {
                result.Add("The contract bandwidth pool cannot be deleted because the following multi-port vifs which reference the pool are defined:");
                var vifs = Mapper.Map<List<Vif>>(contractBandwidthPool.MultiPortVlans.ToList());
                result.AddRange(vifs.Select(q => $"{q.Vrf.Device.Name}, {q.Name}"));
                result.IsSuccess = false;
            }

            return result;
        }
    }
}

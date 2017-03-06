using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using SCM.Models.NetModels;
using System.Threading.Tasks;
using AutoMapper;

namespace SCM.Services.SCMServices
{
    public class PortService : BaseService, IPortService
    {
        public PortService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public async Task<Port> GetByIDAsync(int key)
        {
            return await UnitOfWork.PortRepository.GetByIDAsync(key);
        }

        public async Task<int> AddAsync(Port port)
        {
            this.UnitOfWork.PortRepository.Insert(port);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> UpdateAsync(Port port)
        {
            this.UnitOfWork.PortRepository.Update(port);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(Port port)
        {
            this.UnitOfWork.PortRepository.Delete(port);
            return await this.UnitOfWork.SaveAsync();
        }
        public ServiceResult ValidateDelete (Port port)
        {
            var result = new ServiceResult { IsSuccess = true };

            if (port.TenantID != null)
            {
                result.Add("The port cannot be deleted because it is assigned to a tenant.");
                result.Add("Delete the attachment resource first for this port and try again.");
                result.IsSuccess = false;
            }

            return result;
        }
    }
}
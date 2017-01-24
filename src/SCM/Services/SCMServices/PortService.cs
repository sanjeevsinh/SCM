﻿using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using System.Threading.Tasks;

namespace SCM.Services.SCMServices
{
    public class PortService : BaseService, IPortService
    {
        public PortService(IUnitOfWork unitOfWork) : base(unitOfWork)
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
    }
}
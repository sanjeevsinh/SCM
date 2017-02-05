﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Data;

namespace SCM.Services.SCMServices
{
    public class BgpPeerService : BaseService, IBgpPeerService
    {
        public BgpPeerService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<BgpPeer>> GetAllAsync()
        {
            return await this.UnitOfWork.BgpPeerRepository.GetAsync();
        }

        public async Task<BgpPeer> GetByIDAsync(int id)
        {
            return await this.UnitOfWork.BgpPeerRepository.GetByIDAsync(id);
        }

        public async Task<int> AddAsync(BgpPeer bgpPeer)
        {
            this.UnitOfWork.BgpPeerRepository.Insert(bgpPeer);
            return await this.UnitOfWork.SaveAsync();
        }
 
        public async Task<int> UpdateAsync(BgpPeer bgpPeer)
        {
            this.UnitOfWork.BgpPeerRepository.Update(bgpPeer);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(BgpPeer bgpPeer)
        {
            this.UnitOfWork.BgpPeerRepository.Delete(bgpPeer);
            return await this.UnitOfWork.SaveAsync();
        }
    }
}

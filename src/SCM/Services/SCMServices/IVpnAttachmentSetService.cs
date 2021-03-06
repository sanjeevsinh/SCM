﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Data;

namespace SCM.Services.SCMServices
{
    public interface IVpnAttachmentSetService
    {
        IUnitOfWork UnitOfWork { get; }

        Task<IEnumerable<VpnAttachmentSet>> GetAllAsync();
        Task<VpnAttachmentSet> GetByIDAsync(int id);
        Task<int> AddAsync(VpnAttachmentSet attachmentSetVpn);
        Task<int> UpdateAsync(VpnAttachmentSet attachmentSetVpn);
        Task<int> DeleteAsync(VpnAttachmentSet attachmentSetVpn);
        ServiceResult ValidateNew(VpnAttachmentSet vpnAttachmentSet, Vpn vpn, AttachmentSet attachmentSet);
    }
}

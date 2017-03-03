using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Data;

namespace SCM.Services.SCMServices
{
    public interface ITenantAttachmentsService
    {
        IUnitOfWork UnitOfWork { get; }
        Task<TenantAttachments> GetByTenantAsync(Tenant tenant);
    }
}

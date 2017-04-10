using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Models.ServiceModels;
using SCM.Data;
using System.Linq.Expressions;

namespace SCM.Services.SCMServices
{
    public interface IAttachmentOrVifService
    {
        IUnitOfWork UnitOfWork { get; }
        Task<IEnumerable<AttachmentOrVif>> GetAllByVpnIDAsync(int vpnID);
        Task<AttachmentOrVif> GetByIDAsync(int id, bool? vif = false, bool? attachmentIsMultiPort = false);
    }
}

using AutoMapper;
using SCM.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCM.Services.SCMServices
{
    public abstract class BaseService
    {
        public IUnitOfWork UnitOfWork { get; set; }
        public IMapper Mapper { get; set; }
        public INetworkSyncService NetSync { get; set; }

        public BaseService(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }
        public BaseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
        }

        public BaseService(IUnitOfWork unitOfWork, IMapper mapper, INetworkSyncService netsync)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
            NetSync = netsync;
        }
    }
}

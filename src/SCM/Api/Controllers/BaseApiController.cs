using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace SCM.Api.Controllers
{
    [ValidateModel]
    public abstract class BaseApiController : Controller
    {

        public BaseApiController(IMapper mapper)
        {
            Mapper = mapper;
        }

        internal IMapper Mapper { get; set; }
    }
}
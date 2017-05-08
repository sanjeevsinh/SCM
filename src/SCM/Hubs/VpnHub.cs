using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SCM.Services.SCMServices;
using SCM.Models;
using AutoMapper;
using SCM.Models.ViewModels;

namespace SCM.Hubs
{
    public class VpnHub : Hub<IVpnHub>
    {
        private VpnHub(IMapper mapper)
        {
            Mapper = mapper;
        }

        private readonly IMapper Mapper;

        public override Task OnConnected()
        {
            // Set connection id for just connected client only
            return Clients.Client(Context.ConnectionId).SetConnectionId(Context.ConnectionId);
        } 
    }
}

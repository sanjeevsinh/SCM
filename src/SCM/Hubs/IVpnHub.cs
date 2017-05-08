using Microsoft.AspNetCore.SignalR.Hubs;
using SCM.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCM.Hubs
{
    public interface IVpnHub
    {
        Task SetConnectionId(string connectionId);
    }
}

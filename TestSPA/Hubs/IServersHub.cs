using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestSPA.Models;

namespace TestSPA.Hubs
{
    public interface IServersHub
    {
        Task SendServerAdded(VirtualServer server);
        Task SendServerChanged(VirtualServer server);
        Task SendServersRemoved(IEnumerable<VirtualServer> servers);
    }
}

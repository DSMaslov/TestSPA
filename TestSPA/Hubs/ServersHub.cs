using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestSPA.Models;

namespace TestSPA.Hubs
{
    public class ServersHub: Hub<IServersHub>
    {
        public async Task ServerAdded(VirtualServer server)
        {
            await Clients.All.SendServerAdded(server);
        }

        public async Task ServerChanged(VirtualServer server)
        {
            await Clients.All.SendServerChanged(server);
        }

        public async Task ServersRemoved(IEnumerable<VirtualServer> servers)
        {
            await Clients.All.SendServersRemoved(servers);
        }
    }
}

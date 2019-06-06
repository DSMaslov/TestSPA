using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestSPA.Hubs;
using TestSPA.Models;
using TestSPA.Repositories;

namespace TestSPA.Services
{
    public class ServersService
    {
        private readonly ServersRepository _repository;
        private readonly IHubContext<ServersHub, IServersHub> _hubContext;

        public ServersService(ServersRepository repository, IHubContext<ServersHub, IServersHub> hubContext)
        {
            _repository = repository;
            _hubContext = hubContext;
        }

        public async Task<IEnumerable<VirtualServer>> GetVirtualServers() => await _repository.GetVirtualServers();
            
        public async Task<VirtualServer> GetVirtualServer(int id) => await _repository.GetVirtualServer(id);
        
        public async Task<VirtualServer> AddVirtualServer()
        {
            var addedServer =  await _repository.AddVirtualServer(new VirtualServer { CreateDateTime = DateTime.Now });

            await _hubContext.Clients.All.SendServerAdded(addedServer);

            return addedServer;
        }

        public async Task SelectForRemove(int id, bool selected)
        {
            var changedServer = await _repository.SelectForRemove(id, selected);

            await _hubContext.Clients.All.SendServerChanged(changedServer);
        }

        public async Task RemoveSelected()
        {
            var removedServers = await _repository.RemoveSelected();

            await _hubContext.Clients.All.SendServersRemoved(removedServers);
        }
    }
}

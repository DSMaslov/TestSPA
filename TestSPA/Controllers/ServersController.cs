using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestSPA.Models;
using TestSPA.Repositories;

namespace TestSPA.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class ServersController : ControllerBase
    {
        private readonly ServersRepository _repository;

        public ServersController(ServersRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [HttpGet("list")]
        public async Task<IEnumerable<VirtualServer>> List()
        {
            return await _repository.GetVirtualServers();
        }

        [HttpGet("{id}")]
        public async Task<VirtualServer> Get(int id)
        {
            return await _repository.GetVirtualServer(id);
        }

        [HttpPost("add")]
        public async Task<VirtualServer> Add()
        {
            var newServer = new VirtualServer { CreateDateTime = DateTime.Now };

            return await _repository.AddVirtualServer(newServer);
        }

        [HttpPost("selectForRemove")]
        public async Task SelectForRemove([FromBody]int[] ids)
        {
            await _repository.SelectForRemove(ids);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestSPA.Models;
using TestSPA.Services;

namespace TestSPA.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class ServersController : ControllerBase
    {
        private readonly ServersService _serversService;

        public ServersController(ServersService serversService)
        {
            _serversService = serversService;
        }

        [HttpGet]
        [HttpGet("list")]
        public async Task<IEnumerable<VirtualServer>> List() => await _serversService.GetVirtualServers();
        
        [HttpGet("{id}")]
        public async Task<VirtualServer> Get(int id) => await _serversService.GetVirtualServer(id);
        
        [HttpPost("add")]
        public async Task<VirtualServer> Add() => await _serversService.AddVirtualServer();
        
        [HttpPost("selectForRemove")]
        public async Task SelectForRemove([FromBody]int[] ids) => await _serversService.SelectForRemove(ids);
    }
}

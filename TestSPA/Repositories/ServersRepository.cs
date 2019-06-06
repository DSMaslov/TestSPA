using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TestSPA.Models;

namespace TestSPA.Repositories
{
    public class ServersRepository
    {
        private readonly string _connectionString;

        public ServersRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<VirtualServer>> GetVirtualServers()
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var query = "SELECT VirtualServerId, CreateDateTime, RemoveDateTime, SelectedForRemove FROM VirtualServers";
                return await db.QueryAsync<VirtualServer>(query);
            }
        }

        public async Task<VirtualServer> GetVirtualServer(int id)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var query = "SELECT VirtualServerId, CreateDateTime, RemoveDateTime, SelectedForRemove FROM VirtualServers Where Id = @Id";
                return (await db.QueryAsync<VirtualServer>(query, new { Id = id})).FirstOrDefault();
            }
        }

        public async Task<VirtualServer> AddVirtualServer(VirtualServer data)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var query = @"INSERT INTO VirtualServers (CreateDateTime, RemoveDateTime, SelectedForRemove)
                              OUTPUT inserted.VirtualServerId, inserted.CreateDateTime, inserted.RemoveDateTime, inserted.SelectedForRemove 
                              VALUES (@CreateDateTime, @RemoveDateTime, @SelectedForRemove)";

                return (await db.QueryAsync<VirtualServer>(query, data)).FirstOrDefault();
            }
        }
        
        public async Task SelectForRemove(int[] ids)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var query = $@"UPDATE VirtualServers SET SelectedForRemove = @True Where VirtualServerId in @Ids";

                await db.ExecuteAsync(query, new { Ids = ids, True = true });
            }
        }
    }
}

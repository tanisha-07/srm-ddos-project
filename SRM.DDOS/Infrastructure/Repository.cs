using Dapper;
using System.Data;
using System.Net;
using SRM.DDOS.API.Infrastructure.Abstraction;

namespace SRM.DDOS.API.Infrastructure
{
    public class Repository : IRepository
    {
        private readonly IDapperContext _context;
        public Repository(IDapperContext context) 
        {
            _context = context;
        }
        public async Task<bool> AddUpdateWhitelistedIP(string ipAddress, bool isActive = true, string? reason = null)
        {
            using var db = _context.Connection;
            var parameters = new DynamicParameters();

            parameters.Add("@pIPAddress", ipAddress);
            parameters.Add("@pIsActive", isActive);
            parameters.Add("@pAppKey", 1);
            parameters.Add("@pReason", reason);
            //return await db.ExecuteScalarAsync<bool>("[ddos].[spSaveUpdateWhitelistedIP]", parameters, commandType: CommandType.StoredProcedure);
            return await db.ExecuteScalarAsync<bool>("[dbo.spSaveUpdateWhitelistedIP]", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> AddUpdateBlacklistedIP(string ipAddress, bool isActive = true, string? reason = default)
        {
            using var db = _context.Connection;
            var parameters = new DynamicParameters();

            parameters.Add("@pIPAddress", ipAddress);
            parameters.Add("@pIsActive", isActive);
            parameters.Add("@pAppKey", 1);
            parameters.Add("@pReason", reason);
            return await db.ExecuteScalarAsync<bool>("dbo.spSaveUpdateBlacklistedIP", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<string?> GetAllBlacklistedIPs(bool? isActive = default)
        {
            using var db = _context.Connection;
            var parameters = new DynamicParameters();
            parameters.Add("@pIsActive", isActive.Value);
            parameters.Add("@pAppKey", null);
            var ips = await db.QueryAsync<string>("dbo.spGetAllBlacklistedIPs", parameters, commandType: CommandType.StoredProcedure);
            return string.Join(";", ips.ToList());
            //return "";
        }
        public async Task<bool?> SaveTraffic(string ipAddress)
        {
            using var db = _context.Connection;
            var parameters = new DynamicParameters();
            parameters.Add("@pIPAddress", ipAddress);
            parameters.Add("@pAppKey", null);
            await db.ExecuteScalarAsync<string>("dbo.spSaveTraffic", parameters, commandType: CommandType.StoredProcedure);
            return true;
        }
        public async Task<IEnumerable<string>> GetAllBlockedCountries(bool? isActive = default)
        {
            using var db = _context.Connection;
            var parameters = new DynamicParameters();
            parameters.Add("@pIsActive", isActive.Value);
            parameters.Add("@pAppKey", null);
            var ips = await db.QueryAsync<string>("dbo.spGetAllBlockedCountries", parameters, commandType: CommandType.StoredProcedure);
            return ips.ToList();
            //return "";
        }

        public async Task<string?> GetAllWhitelistedIPs(bool? isActive = null)
        {
            using var db = _context.Connection;
            var parameters = new DynamicParameters();
            parameters.Add("@pIsActive", isActive);
            parameters.Add("@pAppKey", null);
            var ips = await db.QueryAsync<string>("dbo.[spGetAllWhitelistedIPs]", parameters, commandType: CommandType.StoredProcedure);
            return string.Join(";", ips.ToList());
        }
    }
}

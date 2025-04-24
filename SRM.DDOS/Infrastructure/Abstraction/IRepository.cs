using System.Net;

namespace SRM.DDOS.API.Infrastructure.Abstraction
{
    public interface IRepository
    {
        Task<bool?> SaveTraffic(string ipAddress);
        Task<bool> AddUpdateBlacklistedIP(string ipAddress, bool isActive = true, string? reason = default);
        Task<string?> GetAllBlacklistedIPs(bool? isActive = default);
        Task<bool> AddUpdateWhitelistedIP(string ipAddress, bool isActive = true, string? reason = default);
        Task<string?> GetAllWhitelistedIPs(bool? isActive = default);
        Task<IEnumerable<string>> GetAllBlockedCountries(bool? isActive = default);
    }
}

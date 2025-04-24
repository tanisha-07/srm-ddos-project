using System.Net;

namespace SRM.DDOS.API.Infrastructure.Abstraction
{
    public interface IIpWhitelistingServices
    {
        Task<bool> IsWhitelisted(IPAddress? ipAddress);
    }
}

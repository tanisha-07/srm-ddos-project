using System.Net;

namespace SRM.DDOS.API.Infrastructure.Abstraction
{
    public interface IIpBlockingServices
    {
        Task<bool> IsBlocked(IPAddress? ipAddress);
    }
}

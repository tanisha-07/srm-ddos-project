using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Net;
using SRM.DDOS.API.Infrastructure.Abstraction;

namespace SRM.DDOS.API.Infrastructure
{
    public sealed class IpWhitelistingServices : IIpWhitelistingServices
    {
        private const char SplitChar = ';';
        private readonly ILogger<IpWhitelistingServices> _logger;
        private readonly IRepository _repository;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        private byte[][] _whitelistedlist;

        public IpWhitelistingServices(
            ILogger<IpWhitelistingServices> logger,
            IRepository repository,
            IMemoryCache memoryCache,
            IConfiguration configuration)
        {
            _logger = logger;
            _repository = repository;
            _memoryCache = memoryCache;
            _configuration = configuration;
        }
        private async Task<bool> InitializeWhitelistedList()
        {
            if (!_memoryCache.TryGetValue<string>(Constants.CacheKeyNames.IpWhitelistKey, out string? whitelistedIps)
                || string.IsNullOrEmpty(whitelistedIps))
            {
                whitelistedIps = await _repository.GetAllWhitelistedIPs(true) ?? "";
                _memoryCache.Set(Constants.CacheKeyNames.IpWhitelistKey, whitelistedIps, TimeSpan.FromDays(1));
            }

            if (!string.IsNullOrEmpty(whitelistedIps))
            {
                var _Ips = whitelistedIps.Split(SplitChar);
                _whitelistedlist = new byte[_Ips.Length][];
                for (var i = 0; i < _Ips.Length; i++)
                {
                    if (IPAddress.TryParse(_Ips[i], out IPAddress? address) && address is not null)
                    {
                        _whitelistedlist[i] = address.GetAddressBytes();
                    }
                    else
                    {
                        _logger.LogError("Unable to parse this IPAddress {ipaddress}", _Ips[i]);
                    }
                }
                return true;
            }
            return false;
        }

        private bool IsIpRestrictionEnabled()
        {
            var isEnabled = _configuration["IsIpRestrictionEnabled"];
            if (!string.IsNullOrEmpty(isEnabled) && bool.TryParse(isEnabled, out bool result))
            {
                return result;
            }
            // By Default IP restriction will be false, if no DB entry or wrong DB entry
            return false;
        }
        public async Task<bool> IsWhitelisted(IPAddress? ipAddress)
        {
            var isWhitlelistedIp = false;
            if (!IsIpRestrictionEnabled())
            {
                return true;
            }

            if (ipAddress is null)
            {
                _logger.LogWarning("IpWhitelistedServices: IpAddress is null.");
                return isWhitlelistedIp;
            }
            try
            {
                _logger.LogInformation(
                      "IpWhitelistingServices: Incoming Request from Remote IP address: {RemoteIp}", ipAddress);
                var isInitialized = await InitializeWhitelistedList();
                if (_whitelistedlist is null || _whitelistedlist.Length == 0) { return isWhitlelistedIp; }

                var bytes = ipAddress.GetAddressBytes();
                foreach (var address in _whitelistedlist.Where(x => x is not null))
                {
                    if (address.SequenceEqual(bytes))
                    {
                        isWhitlelistedIp = true;
                        break;
                    }
                }
                if (!isWhitlelistedIp)
                {
                    _logger.LogWarning(
                        "IpWhitelistedServices: Forbidden Request from Remote IP address: {RemoteIp}", ipAddress);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IpBlockingService: Failure.");
            }

            return isWhitlelistedIp;
        }
    }
}

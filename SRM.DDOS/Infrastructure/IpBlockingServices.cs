using IPinfo.Models;
using IPinfo;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System.Net;
using SRM.DDOS.API.Infrastructure.Abstraction;

namespace SRM.DDOS.API.Infrastructure
{
    public sealed class IpBlockingServices : IIpBlockingServices
    {
        private const char SplitChar = ';';
        private readonly ILogger<IpBlockingServices> _logger;
        private readonly IRepository _repository;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        private byte[][] _blockedlist;
        private IEnumerable<string> _blockedCountries;
        public IpBlockingServices(
            ILogger<IpBlockingServices> logger,
            IRepository repository,
            IMemoryCache memoryCache,
            IConfiguration configuration)
        {
            _logger = logger;
            _repository = repository;
            _memoryCache = memoryCache;
            _configuration = configuration;
        }

        private async Task<bool> InitializeBlockedList()
        {
            if (!_memoryCache.TryGetValue<string>(Constants.CacheKeyNames.IpBlacklistedKey, out string? blockedIps)
                || string.IsNullOrEmpty(blockedIps))
            {
                blockedIps = await _repository.GetAllBlacklistedIPs(true) ?? "";
                _memoryCache.Set(Constants.CacheKeyNames.IpBlacklistedKey, blockedIps, TimeSpan.FromMinutes(1));
            }

            if (!string.IsNullOrEmpty(blockedIps))
            {
                var _Ips = blockedIps.Split(SplitChar);
                _blockedlist = new byte[_Ips.Length][];
                for (var i = 0; i < _Ips.Length; i++)
                {
                    if (IPAddress.TryParse(_Ips[i], out IPAddress? address) && address is not null)
                    {
                        _blockedlist[i] = address.GetAddressBytes();
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
        private async Task<bool> InitializeBlockedCountries()
        {
            if (!_memoryCache.TryGetValue<IEnumerable<string>>(Constants.CacheKeyNames.BlockedCountriesKey, out IEnumerable<string> blockedCountries)
                || blockedCountries == null)
            {
                blockedCountries = await _repository.GetAllBlockedCountries(true);
                _memoryCache.Set(Constants.CacheKeyNames.BlockedCountriesKey, blockedCountries, TimeSpan.FromMinutes(1));
            }

            if (blockedCountries != null)
            {
                _blockedCountries = blockedCountries;
                return true;
            }
            return false;
        }

        public async Task<bool> IsBlocked(IPAddress? ipAddress)
        {
            var isBadIp = false;
            if (ipAddress is null)
            {
                _logger.LogWarning("IpBlockingService: IpAddress is null.");
                return isBadIp;
            }
            try
            {
                await _repository.SaveTraffic(ipAddress.ToString());
                _logger.LogInformation(
                       "IpBlockingService: Incoming Request from Remote IP address: {RemoteIp}", ipAddress);
                var isInitialized = await InitializeBlockedList();
                if (_blockedlist is null || _blockedlist.Length == 0) { return isBadIp; }

                var bytes = ipAddress.GetAddressBytes();
                foreach (var address in _blockedlist.Where(x => x is not null))
                {
                    if (address.SequenceEqual(bytes))
                    {
                        isBadIp = true;
                        break;
                    }
                }
                if (isBadIp)
                {
                    _logger.LogWarning(
                        "IpBlockingService: Forbidden Request from Remote IP address: {RemoteIp}", ipAddress);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IpBlockingService: Failure.");
            }

            return isBadIp;
        }

        public async Task<bool> IsGeoFenced(string remoteIp)
        {
            await InitializeBlockedCountries();
            string token = "27026d25c4443e";
            IPinfoClient client = new IPinfoClient.Builder()
                .AccessToken(token)
                .Build();
            IPResponse ipResponse = await client.IPApi.GetDetailsAsync(remoteIp);
            var country = ipResponse.Country;
            return _blockedCountries.Contains(country);
        }

    }
}

namespace SRM.DDOS.API.Infrastructure
{
    public static class Constants
    {
        public static class Errors
        {
            public const string IsIPBlocked = nameof(IsIPBlocked);
        }
        public static class CacheKeyNames
        {
            public static string IpWhitelistKey = nameof(IpWhitelistKey);
            public static string IpBlacklistedKey = nameof(IpBlacklistedKey);
            public static string BlockedCountriesKey = nameof(BlockedCountriesKey);
        }
    }
}

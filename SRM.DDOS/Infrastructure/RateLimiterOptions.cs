
namespace SRM.DDOS.API.Infrastructure
{
    public sealed class CustomRateLimiterOptions
    {
        public const string RateLimiter = "RateLimiter";
        public int DevicePermitLimit { get; set; } = 100;
        public int DeviceWindowInSeconds { get; set; } = 10;

        public int IpPermitLimit { get; set; } = 100;
        public int IpWindowInSeconds { get; set; } = 10;
        public bool DeviceAutoReplenishment { get; set; } = false;
        public bool IpAutoReplenishment { get; set; } = false;
    }
}

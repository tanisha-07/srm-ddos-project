using Microsoft.AspNetCore.OutputCaching;
using SRM.DDOS.API.Infrastructure.Abstraction;

namespace SRM.DDOS.API.Infrastructure
{
    public static class MiddlewareExtensions
    {
        public static IServiceCollection AddOutputCaching(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.Configure<OutputCacheOptions>(configuration.GetSection(nameof(OutputCacheOptions)));
            services.AddOutputCache(opt =>
            {
                opt.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(Convert.ToDouble(configuration["OutputCacheOptions:DefaultExpirationInSeconds"]));
                /*opt.AddPolicy(
                    Constants.CachePolicyNames.LookUpData,
                    builder => builder.Expire(TimeSpan.FromSeconds(Convert.ToDouble(configuration["OutputCacheOptions:LookUpDataExpirationInSeconds"]))));
                opt.AddPolicy(
                    Constants.CachePolicyNames.Reports,
                    builder => builder.Expire(TimeSpan.FromSeconds(Convert.ToDouble(configuration["OutputCacheOptions:ReportsExpirationInSeconds"]))));*/
            });
            return services;
        }
        public static IApplicationBuilder UseIpBlocking(this IApplicationBuilder builder) => builder.UseMiddleware<IpBlockMiddleware>();
        /// <summary>
        /// Uses the IP white-list.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseIpWhiteList(this IApplicationBuilder builder) => builder.UseMiddleware<IpWhitelistedMiddleware>();
        /// <summary>
        /// Add Dependencies.
        /// </summary>
        /// <param name="services">The builder.</param>
        /// <returns></returns>
        public static void AddDependencies(this IServiceCollection services)
        {
            services.AddScoped<IDapperContext, SQLDapperContext>();
            services.AddScoped<IRepository, Repository>();
            services.AddScoped<IIpBlockingServices, IpBlockingServices>();
            services.AddScoped<IIpBlockingServices, IpBlockingServices>();
            services.AddScoped<IIpWhitelistingServices, IpWhitelistingServices>();
        }
    }
}

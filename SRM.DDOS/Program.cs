using Microsoft.AspNetCore.RateLimiting;
using System.Configuration;
using System.Net;
using System.Threading.RateLimiting;
using SRM.DDOS.API.Infrastructure;
using SRM.DDOS.API.Infrastructure.Abstraction;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOutputCaching(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.AddDependencies();
builder.Services.Configure<IISServerOptions>(options =>
{
    options.AutomaticAuthentication = false;
});
//Apply CORS
builder.Services.AddCors(options => options.AddPolicy("CorsPolicy",
   policy =>
   {
       policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
   }));
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

//Apply rate limits
builder.Services.Configure<CustomRateLimiterOptions>(
    builder.Configuration.GetSection(CustomRateLimiterOptions.RateLimiter));
var rateLimiterOptions = new CustomRateLimiterOptions();
builder.Configuration.GetSection(CustomRateLimiterOptions.RateLimiter).Bind(rateLimiterOptions);
builder.Services.AddRateLimiter(limiterOptions =>
{
    limiterOptions.OnRejected = (context, cancellationToken) =>
    {
        var userAgent = context.HttpContext.Request.Headers.UserAgent.ToString();
        IPAddress? clientIP = context.HttpContext.Connection.RemoteIpAddress;
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.RequestServices.GetService<ILoggerFactory>()?
            .CreateLogger("Microsoft.AspNetCore.RateLimitingMiddleware")
            .LogWarning("OnRejected: UserAgent: {useragent} IpAddress: {ipaddress}.", userAgent, clientIP?.ToString());
        var repo = context.HttpContext.RequestServices.GetService<IRepository>();
        var res = repo.AddUpdateBlacklistedIP(clientIP.ToString(), true, "RateLimiter").Result;
        return new ValueTask();
    };
    limiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    limiterOptions.GlobalLimiter = PartitionedRateLimiter.CreateChained(
        PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        {
            var userAgent = httpContext.Request.Headers.UserAgent.ToString();
            return RateLimitPartition.GetFixedWindowLimiter(
                userAgent, _ => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = rateLimiterOptions.DeviceAutoReplenishment,
                    PermitLimit = rateLimiterOptions.DevicePermitLimit,
                    Window = TimeSpan.FromSeconds(rateLimiterOptions.DeviceWindowInSeconds)
                });
        }),
        PartitionedRateLimiter.Create<HttpContext, IPAddress>(httpContext =>
        {
            IPAddress? clientIP = httpContext.Connection.RemoteIpAddress;
            if (!IPAddress.IsLoopback(clientIP!))
            {
                return RateLimitPartition.GetFixedWindowLimiter(
                    clientIP!, _ => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = rateLimiterOptions.IpAutoReplenishment,
                        PermitLimit = rateLimiterOptions.IpPermitLimit,
                        Window = TimeSpan.FromSeconds(rateLimiterOptions.IpWindowInSeconds)
                    });
            }
            return RateLimitPartition.GetNoLimiter(IPAddress.Loopback);
        }));
});
var app = builder.Build();
app.UseCors(builder =>
{
    builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader();
});

// Configure the HTTP request pipeline.
    app.UseSwagger();
    app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseOutputCache();

app.UseAuthorization();

app.MapControllers();
app.UseIpWhiteList();

app.UseIpBlocking();
//if (!app.Environment.IsDevelopment())
//{
    app.UseRateLimiter();
//}

app.Run();

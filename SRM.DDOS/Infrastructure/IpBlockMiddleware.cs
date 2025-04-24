using IPinfo;
using IPinfo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Net;
using SRM.DDOS.API.Infrastructure.Abstraction;

namespace SRM.DDOS.API.Infrastructure
{
    public sealed class IpBlockMiddleware
    {
        private readonly RequestDelegate _next;

        public IpBlockMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IIpBlockingServices blockingService)
        {
            var remoteIp = context.Connection.RemoteIpAddress;
            if (remoteIp is null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }            
            if (!IPAddress.IsLoopback(remoteIp))
            {
                var isBlocked = await blockingService.IsBlocked(remoteIp);

                if (isBlocked)
                {
                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status403Forbidden,
                        Title = Constants.Errors.IsIPBlocked
                    };
                    context.Response.StatusCode = problemDetails.Status.Value;
                    await context.Response.WriteAsJsonAsync(problemDetails);
                    return;
                }
            }

            await _next.Invoke(context);
        }
    }
}

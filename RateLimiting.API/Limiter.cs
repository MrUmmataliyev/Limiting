using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace RateLimiting.API
{
    public class Limiter
    {
        private int _requestLimit;
        private readonly RequestDelegate _next;
        private DateTime _resetTime;

        public Limiter(RequestDelegate next)
        {
            _requestLimit = 0;
            _next = next;
            _resetTime = DateTime.UtcNow.AddSeconds(10);
        }

        public async Task InvokeAsync(HttpContext context)
        { 
            var currentTime = DateTime.UtcNow;

            if (currentTime == _resetTime && _requestLimit != 0)
            {
                _requestLimit = 0;
                _resetTime = currentTime.AddSeconds(20);
            }

            if (_requestLimit < 5)
            {
                _requestLimit++;
                await _next(context);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Error occured 429");
                return;
            }
        }
    }
}

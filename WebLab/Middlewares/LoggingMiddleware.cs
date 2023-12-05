using System;
using Serilog;
using Serilog.Context;

namespace WebLab.Middlewares
{
	public class LoggingMiddleware
	{
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            var response = context.Response;
            var statusCode = response.StatusCode;

            if (statusCode < 200 || statusCode >= 300)
            {
                var request = context.Request;
                Log.Logger
                    .ForContext("RequestPath", request.Path + request.QueryString)
                    .ForContext("StatusCode", statusCode)
                    .Information("");
            }
        }
    }

}


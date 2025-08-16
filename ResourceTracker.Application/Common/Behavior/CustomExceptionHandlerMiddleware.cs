using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResourceTracker.Application.Common.Exceptions;
using System.Net;
using System.Text.Json;

namespace ResourceTracker.Application.Common.Behavior
{
    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomExceptionHandlerMiddleware> _logger;

        public CustomExceptionHandlerMiddleware(RequestDelegate next, ILogger<CustomExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BadRequestException ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                var result = JsonSerializer.Serialize(new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = ex.Message,
                    Errors = ex.Errors
                });

                await context.Response.WriteAsync(result);
            }
        }
    }
}

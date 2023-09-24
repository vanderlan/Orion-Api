using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;
using Orion.Domain.Exceptions;

namespace Orion.Api.Middleware
{
    public class OrionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;
        private readonly IHostEnvironment _env;

        public OrionMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IHostEnvironment env)
        {
            _env = env;
            _loggerFactory = loggerFactory;
            _next = next;

            _logger = _loggerFactory.CreateLogger<OrionMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                if (_next != null)
                {
                    await _next(context);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = GetStatusCodeByException(exception);

            var errorResponse = new ExceptionResponse(exception.Message, NotificationType.Error);

            if (exception is not BusinessException && _env.IsDevelopment())
            {
                errorResponse = new ExceptionResponse(exception.Message, NotificationType.Error);
            }
            if (exception is UnauthorizedUserException)
            {
                errorResponse = new ExceptionResponse(exception.Message, NotificationType.Error);
            }

            if (exception is BusinessException businessException)
            {
                errorResponse.Title = businessException.Title;
            }

            await ProccessResponseAsync(context, statusCode, errorResponse);
        }

        private static HttpStatusCode GetStatusCodeByException(Exception exception)
        {
            return exception is NotFoundException ? HttpStatusCode.NotFound :
                exception is ConflictException ? HttpStatusCode.Conflict :
                exception is UnauthorizedUserException ? HttpStatusCode.Unauthorized :
                exception is BusinessException ? HttpStatusCode.BadRequest
                : HttpStatusCode.InternalServerError;
        }

        private async Task ProccessResponseAsync(HttpContext context, HttpStatusCode status, ExceptionResponse errorResponse)
        {
            var errrorReturn = JsonConvert.SerializeObject(errorResponse);

            _logger.LogError(errrorReturn);

            context.Response.StatusCode = (int)status;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(errrorReturn);
        }
    }
}

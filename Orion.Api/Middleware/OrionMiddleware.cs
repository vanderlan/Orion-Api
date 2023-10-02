using Newtonsoft.Json;
using Orion.Domain.Exceptions;
using System.Net;

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

            await ProccessResponseAsync(context, statusCode, errorResponse, exception);
        }

        private static HttpStatusCode GetStatusCodeByException(Exception exception)
        {
            return exception is NotFoundException ? HttpStatusCode.NotFound :
                exception is ConflictException ? HttpStatusCode.Conflict :
                exception is UnauthorizedUserException ? HttpStatusCode.Unauthorized :
                exception is BusinessException ? HttpStatusCode.BadRequest
                : HttpStatusCode.InternalServerError;
        }

        private async Task ProccessResponseAsync(HttpContext context, HttpStatusCode statusCode, ExceptionResponse errorResponse, Exception exception)
        {
            var errrorReturn = JsonConvert.SerializeObject(errorResponse);

            if(statusCode == HttpStatusCode.InternalServerError)
                _logger.LogError(exception, "Internal Server Error: {message}", errorResponse.Message);

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(errrorReturn);
        }
    }
}

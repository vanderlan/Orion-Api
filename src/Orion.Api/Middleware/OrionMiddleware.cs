using Newtonsoft.Json;
using Orion.Domain.Exceptions;
using System.Net;

namespace Orion.Api.Middleware;

public class OrionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<OrionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public OrionMiddleware(RequestDelegate next, ILogger<OrionMiddleware> logger, IHostEnvironment env)
    {
        _env = env;
        _next = next;
        _logger = logger;
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
        return exception switch
        {
            NotFoundException => HttpStatusCode.NotFound,
            ConflictException => HttpStatusCode.Conflict,
            UnauthorizedUserException => HttpStatusCode.Unauthorized,
            BusinessException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError
        };
    }

    private async Task ProccessResponseAsync(HttpContext context, HttpStatusCode statusCode, ExceptionResponse errorResponse, Exception exception)
    {
        var errrorReturn = JsonConvert.SerializeObject(errorResponse);

        if (statusCode == HttpStatusCode.InternalServerError)
            foreach (var error in errorResponse.Errors)
                _logger.LogError(exception, "Internal Server Error: {message}", error);
       
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(errrorReturn);
    }
}

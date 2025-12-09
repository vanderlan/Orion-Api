using Newtonsoft.Json;
using Company.Orion.Domain.Core.Exceptions;
using System.Net;

namespace Company.Orion.Api.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            if (next != null)
            {
                await next(context);
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
        if (statusCode == HttpStatusCode.InternalServerError)
        {
            errorResponse.Title = "Unexpected error";
            foreach (var error in errorResponse.Errors)
                logger.LogError(exception, "Internal Server Error: {Message}", error);
        }
       
        var errrorReturn = JsonConvert.SerializeObject(errorResponse);
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(errrorReturn);
    }
}

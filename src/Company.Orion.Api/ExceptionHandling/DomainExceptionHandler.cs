using Company.Orion.Domain.Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Company.Orion.Api.ExceptionHandling;

/// <summary>
/// Translates the exceptions thrown by the domain/application layers into RFC 9457 Problem Details
/// responses (application/problem+json). Plugged in by <c>app.UseExceptionHandler()</c>.
/// Business exceptions carry messages meant for the client; anything else is logged and answered
/// with a generic 500 so internal details are never leaked.
/// </summary>
public sealed class DomainExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<DomainExceptionHandler> logger) : IExceptionHandler
{
    internal const string UnexpectedErrorTitle = "Unexpected error";
    internal const string UnexpectedErrorDetail = "An unexpected error occurred while processing the request. Please try again later.";

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var statusCode = GetStatusCode(exception);
        var problemDetails = CreateProblemDetails(exception, statusCode);

        Log(httpContext, exception, statusCode);

        httpContext.Response.StatusCode = statusCode;

        var written = await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails
        });

        if (!written)
        {
            //The client does not accept JSON (Accept header) but the error must still have a body.
            await httpContext.Response.WriteAsJsonAsync(problemDetails, options: null, contentType: "application/problem+json", cancellationToken);
        }

        return true;
    }

    public static int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            ConflictException => StatusCodes.Status409Conflict,
            UnauthorizedUserException => StatusCodes.Status401Unauthorized,
            RefreshTokenRotationException => StatusCodes.Status500InternalServerError,
            BusinessException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };
    }

    private static ProblemDetails CreateProblemDetails(Exception exception, int statusCode)
    {
        //Type and Title (when null) are completed by the framework defaults for the status code.
        if (exception is BusinessException businessException)
        {
            return new ProblemDetails
            {
                Status = statusCode,
                Title = businessException.Title,
                Detail = businessException.Message
            };
        }

        return new ProblemDetails
        {
            Status = statusCode,
            Title = UnexpectedErrorTitle,
            Detail = UnexpectedErrorDetail
        };
    }

    private void Log(HttpContext httpContext, Exception exception, int statusCode)
    {
        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception on {Method} {Path}: {Message}",
                httpContext.Request.Method, httpContext.Request.Path, exception.Message);
            return;
        }

        logger.LogWarning("Request {Method} {Path} failed with {StatusCode}: {Message}",
            httpContext.Request.Method, httpContext.Request.Path, statusCode, exception.Message);
    }
}

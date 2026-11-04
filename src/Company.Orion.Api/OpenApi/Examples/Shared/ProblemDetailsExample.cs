using Company.Orion.Api.ExceptionHandling;
using Microsoft.AspNetCore.Mvc;

namespace Company.Orion.Api.OpenApi.Examples.Shared;

/// <summary>
/// Example of the RFC 9457 payload produced by the <see cref="DomainExceptionHandler"/>,
/// with a message coherent with the status code being documented.
/// </summary>
public sealed class ProblemDetailsExample : IResponseExampleProvider<ProblemDetails>
{
    public ProblemDetails GetExample(int statusCode)
    {
        var (type, title, detail) = statusCode switch
        {
            StatusCodes.Status400BadRequest => ("https://tools.ietf.org/html/rfc9110#section-15.5.1", "Validation Error", "The current password is invalid."),
            StatusCodes.Status401Unauthorized => ("https://tools.ietf.org/html/rfc9110#section-15.5.2", "Authentication Error", "Invalid e-mail or password."),
            StatusCodes.Status403Forbidden => ("https://tools.ietf.org/html/rfc9110#section-15.5.4", "Forbidden", "The current user does not have the required profile."),
            StatusCodes.Status404NotFound => ("https://tools.ietf.org/html/rfc9110#section-15.5.5", "Not Found", $"Resource Not Found with id: {ExampleData.OperatorPublicId}"),
            StatusCodes.Status409Conflict => ("https://tools.ietf.org/html/rfc9110#section-15.5.10", "Conflict", "The e-mail informed is already in use."),
            _ => ("https://tools.ietf.org/html/rfc9110#section-15.6.1", DomainExceptionHandler.UnexpectedErrorTitle, DomainExceptionHandler.UnexpectedErrorDetail)
        };

        return new ProblemDetails
        {
            Type = type,
            Title = title,
            Status = statusCode,
            Detail = detail,
            Instance = "GET /Users/" + ExampleData.OperatorPublicId,
            Extensions = { ["traceId"] = ExampleData.TraceId }
        };
    }
}

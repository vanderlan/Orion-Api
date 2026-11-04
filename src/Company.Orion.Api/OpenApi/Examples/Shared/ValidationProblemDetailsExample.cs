using Microsoft.AspNetCore.Mvc;

namespace Company.Orion.Api.OpenApi.Examples.Shared;

/// <summary>
/// Example of the 400 payload produced by FluentValidation / model binding failures.
/// </summary>
public sealed class ValidationProblemDetailsExample : IExampleProvider<ValidationProblemDetails>
{
    public ValidationProblemDetails GetExample()
    {
        var errors = new Dictionary<string, string[]>
        {
            ["Email"] = ["The e-mail must be informed."],
            ["Password"] = ["The password must be informed."]
        };

        return new ValidationProblemDetails(errors)
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            Title = "Validation Error",
            Status = StatusCodes.Status400BadRequest,
            Instance = "POST /Users",
            Extensions = { ["traceId"] = ExampleData.TraceId }
        };
    }
}

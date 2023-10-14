using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Orion.Domain.Exceptions;
using System.Text.Json;

namespace Orion.Api.Attributes;

/// <summary>
/// This filter changes the FluentValidation default response
/// </summary>
public class CustomValidationAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState.Values.Where(v => v.Errors.Count > 0)
                    .SelectMany(v => v.Errors)
                    .Select(v => v.ErrorMessage)
                    .ToArray();

            var response = new ExceptionResponse(errors, NotificationType.Error)
            {
                Title = "Validation Error"
            };

            context.Result = new JsonResult(response, new JsonSerializerOptions())
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }
    }
}

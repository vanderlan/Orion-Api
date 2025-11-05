using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Company.Orion.Api.Filters;

/// <summary>
/// Validates action arguments using FluentValidation automatically,
/// returning 400 with ValidationProblemDetails when invalid.
/// </summary>
public sealed class AutoValidationActionFilter(IServiceProvider services) : IAsyncActionFilter
{
    private readonly IServiceProvider _services = services;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

        foreach (var kv in context.ActionArguments)
        {
            var arg = kv.Value;
            if (arg is null) 
                continue;

            var argType = arg.GetType();
            var validatorType = typeof(IValidator<>).MakeGenericType(argType);

            if (_services.GetService(validatorType) is IValidator validator)
            {
                var validationContext = new ValidationContext<object>(arg);
                var result = await validator.ValidateAsync(validationContext);

                if (!result.IsValid)
                {
                    foreach (var grp in result.Errors.GroupBy(e => e.PropertyName))
                    {
                        errors[grp.Key] = [.. grp.Select(e => e.ErrorMessage)];
                    }
                }
            }
        }

        if (errors.Count > 0)
        {
            var problem = new ValidationProblemDetails(errors)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "One or more validation errors occurred."
            };

            context.Result = new BadRequestObjectResult(problem);
            return;
        }

        await next();
    }
}

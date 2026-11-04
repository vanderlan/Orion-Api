using Company.Orion.Crosscutting.Resources;
using Company.Orion.Crosscutting.Resources.Messages;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Localization;

namespace Company.Orion.Api.Filters;

/// <summary>
/// Validates the action arguments with FluentValidation automatically and, when invalid, answers
/// 400 with a <see cref="ValidationProblemDetails"/> built by the framework's
/// <see cref="ProblemDetailsFactory"/> (same shape used for model binding errors).
/// </summary>
public sealed class AutoValidationActionFilter(
    IServiceProvider services,
    ProblemDetailsFactory problemDetailsFactory,
    IStringLocalizer<OrionResources> localizer) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var modelState = new ModelStateDictionary();

        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null)
                continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());

            if (services.GetService(validatorType) is not IValidator validator)
                continue;

            var result = await validator.ValidateAsync(new ValidationContext<object>(argument), context.HttpContext.RequestAborted);

            foreach (var error in result.Errors)
            {
                modelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
        }

        if (modelState.ErrorCount > 0)
        {
            var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(
                context.HttpContext,
                modelState,
                statusCode: StatusCodes.Status400BadRequest,
                title: localizer[MessagesKeys.ExceptionsTitles.ValidationError]);

            context.Result = new BadRequestObjectResult(problemDetails);
            return;
        }

        await next();
    }
}

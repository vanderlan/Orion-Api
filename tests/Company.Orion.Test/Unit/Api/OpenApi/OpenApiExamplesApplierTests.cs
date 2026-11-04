using System;
using System.Collections.Generic;
using Company.Orion.Api.Models;
using Company.Orion.Api.OpenApi.Examples;
using Company.Orion.Application.Core.UseCases.Auth.Commands.LoginWithCredentials;
using Company.Orion.Application.Core.UseCases.Users.Commands.Create;
using Company.Orion.Domain.Core.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Xunit;

namespace Company.Orion.Test.Unit.Api.OpenApi;

public class OpenApiExamplesApplierTests
{
    private const string JsonMediaType = "application/json";

    private readonly OpenApiExamplesApplier _applier;

    public OpenApiExamplesApplierTests()
    {
        var services = new ServiceCollection();
        services.AddOpenApiExamples();

        _applier = new OpenApiExamplesApplier(services.BuildServiceProvider(), Options.Create(new JsonOptions()));
    }

    [Fact]
    public void Apply_WhenRequestBodyHasASingleExampleProvider_FillsTheExample()
    {
        //arrange
        var operation = CreateOperation();
        var apiDescription = CreateApiDescription(bodyType: typeof(LoginWithCredentialsRequest));

        //act
        _applier.Apply(operation, apiDescription);

        //assert
        var example = operation.RequestBody.Content[JsonMediaType].Example;

        Assert.NotNull(example);
        Assert.Null(operation.RequestBody.Content[JsonMediaType].Examples);
        Assert.Equal("adm@orion.com", example["email"]?.GetValue<string>());
        Assert.False(string.IsNullOrWhiteSpace(example["password"]?.GetValue<string>()));
    }

    [Fact]
    public void Apply_WhenRequestBodyHasAMultipleExamplesProvider_FillsTheNamedExamples()
    {
        //arrange
        var operation = CreateOperation();
        var apiDescription = CreateApiDescription(bodyType: typeof(UserCreateRequest));

        //act
        _applier.Apply(operation, apiDescription);

        //assert
        var mediaType = operation.RequestBody.Content[JsonMediaType];

        Assert.Null(mediaType.Example);
        Assert.NotNull(mediaType.Examples);
        Assert.Equal(2, mediaType.Examples.Count);

        var admin = Assert.IsType<OpenApiExample>(mediaType.Examples["admin"]);
        var @operator = Assert.IsType<OpenApiExample>(mediaType.Examples["operator"]);

        Assert.Equal("Admin user", admin.Summary);
        Assert.Equal("jane.admin@orion.com", admin.Value["email"]?.GetValue<string>());
        Assert.Equal("operator@orion.com", @operator.Value["email"]?.GetValue<string>());
    }

    [Fact]
    public void Apply_WhenResponseTypeHasAnExampleProvider_FillsTheResponseExample()
    {
        //arrange
        var operation = CreateOperation();
        var apiDescription = CreateApiDescription(responses: [(200, typeof(UserApiTokenModel)), (401, typeof(void))]);

        //act
        _applier.Apply(operation, apiDescription);

        //assert
        var example = operation.Responses["200"].Content[JsonMediaType].Example;

        Assert.NotNull(example);
        Assert.NotNull(example["token"]);
        Assert.NotNull(example["refreshToken"]);
        Assert.Null(operation.Responses["401"].Content[JsonMediaType].Example);
    }

    [Fact]
    public void Apply_WhenResponseTypeHasAStatusAwareProvider_UsesTheStatusCodeOfEachResponse()
    {
        //arrange
        var operation = CreateOperation();
        var apiDescription = CreateApiDescription(responses: [(200, typeof(ProblemDetails)), (401, typeof(ProblemDetails))]);

        //act
        _applier.Apply(operation, apiDescription);

        //assert
        var ok = operation.Responses["200"].Content[JsonMediaType].Example;
        var unauthorized = operation.Responses["401"].Content[JsonMediaType].Example;

        Assert.Equal(200, ok["status"]?.GetValue<int>());
        Assert.Equal(401, unauthorized["status"]?.GetValue<int>());
        Assert.Equal("Authentication Error", unauthorized["title"]?.GetValue<string>());
        Assert.NotNull(unauthorized["traceId"]);
    }

    [Fact]
    public void Apply_WhenThereIsNoProviderForTheType_KeepsTheOperationUntouched()
    {
        //arrange
        var operation = CreateOperation();
        var apiDescription = CreateApiDescription(bodyType: typeof(UserFilter), responses: [(200, typeof(UserFilter))]);

        //act
        _applier.Apply(operation, apiDescription);

        //assert
        Assert.Null(operation.RequestBody.Content[JsonMediaType].Example);
        Assert.Null(operation.RequestBody.Content[JsonMediaType].Examples);
        Assert.Null(operation.Responses["200"].Content[JsonMediaType].Example);
    }

    [Fact]
    public void Apply_WhenOperationHasNoBodyAndNoResponses_DoesNotThrow()
    {
        //arrange
        var operation = new OpenApiOperation();
        var apiDescription = CreateApiDescription();

        //act
        var exception = Record.Exception(() => _applier.Apply(operation, apiDescription));

        //assert
        Assert.Null(exception);
    }

    [Fact]
    public void AddOpenApiExamples_RegistersEveryProviderOfTheApiAssembly()
    {
        //arrange
        var services = new ServiceCollection();

        //act
        services.AddOpenApiExamples();
        var serviceProvider = services.BuildServiceProvider();

        //assert
        Assert.NotNull(serviceProvider.GetService<IExampleProvider<LoginWithCredentialsRequest>>());
        Assert.NotNull(serviceProvider.GetService<IExampleProvider<UserApiTokenModel>>());
        Assert.NotNull(serviceProvider.GetService<IMultipleExamplesProvider<UserCreateRequest>>());
        Assert.NotNull(serviceProvider.GetService<IResponseExampleProvider<ProblemDetails>>());
        Assert.NotNull(serviceProvider.GetService<OpenApiExamplesApplier>());
        Assert.Null(serviceProvider.GetService<IExampleProvider<UserFilter>>());
    }

    private static OpenApiOperation CreateOperation()
    {
        return new OpenApiOperation
        {
            RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    [JsonMediaType] = new OpenApiMediaType()
                }
            },
            Responses = new OpenApiResponses
            {
                ["200"] = new OpenApiResponse
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        [JsonMediaType] = new OpenApiMediaType()
                    }
                },
                ["401"] = new OpenApiResponse
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        [JsonMediaType] = new OpenApiMediaType()
                    }
                }
            }
        };
    }

    private static ApiDescription CreateApiDescription(Type bodyType = null, params (int StatusCode, Type Type)[] responses)
    {
        var apiDescription = new ApiDescription();

        if (bodyType is not null)
        {
            apiDescription.ParameterDescriptions.Add(new ApiParameterDescription
            {
                Name = "body",
                Type = bodyType,
                Source = BindingSource.Body
            });
        }

        foreach (var (statusCode, type) in responses)
        {
            apiDescription.SupportedResponseTypes.Add(new ApiResponseType
            {
                StatusCode = statusCode,
                Type = type
            });
        }

        return apiDescription;
    }
}

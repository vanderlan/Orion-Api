using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Company.Orion.Api.Middleware;
using Company.Orion.Domain.Core.Exceptions;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading.Tasks;
using System;

namespace Company.Orion.Test.Unit.Api.Middleware;

public class ExceptionMiddlewareTests
{
    private readonly Mock<RequestDelegate> _nextMock;
    private readonly Mock<ILogger<ExceptionMiddleware>> _loggerMock;
    private readonly ExceptionMiddleware _middleware;

    public ExceptionMiddlewareTests()
    {
        _nextMock = new Mock<RequestDelegate>();
        _loggerMock = new Mock<ILogger<ExceptionMiddleware>>();
        _middleware = new ExceptionMiddleware(_nextMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Invoke_WhenNoExceptionOccurs_CallNext()
    {
        // Arrange
        var context = new DefaultHttpContext();

        // Act
        await _middleware.Invoke(context);

        // Assert
        _nextMock.Verify(next => next(context), Times.Once);
    }

    [Fact]
    public async Task Invoke_WhenExceptionOccurs_HandleException()
    {
        // Arrange
        var context = new DefaultHttpContext();
        _nextMock.Setup(next => next(context)).Throws(new Exception("Test Exception"));

        // Act
        await _middleware.Invoke(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
    }

    [Theory]
    [InlineData(typeof(NotFoundException), HttpStatusCode.NotFound)]
    [InlineData(typeof(ConflictException), HttpStatusCode.Conflict)]
    [InlineData(typeof(UnauthorizedUserException), HttpStatusCode.Unauthorized)]
    [InlineData(typeof(RefreshTokenRotationException), HttpStatusCode.InternalServerError)]
    [InlineData(typeof(BusinessException), HttpStatusCode.BadRequest)]
    [InlineData(typeof(Exception), HttpStatusCode.InternalServerError)]
    public async Task HandleExceptionAsync_SetCorrectStatusCode(Type exceptionType, HttpStatusCode expectedStatusCode)
    {
        // Arrange
        var context = new DefaultHttpContext();
        var exception = (Exception)Activator.CreateInstance(exceptionType, "Test Exception");

        // Act
        await InvokeHandleExceptionAsync(context, exception);

        // Assert
        Assert.Equal((int)expectedStatusCode, context.Response.StatusCode);
    }

    [Theory]
    [InlineData(typeof(Exception), HttpStatusCode.InternalServerError)]
    [InlineData(typeof(ArgumentNullException), HttpStatusCode.InternalServerError)]
    public async Task HandleExceptionAsync_WithNoBusinessException_SetInternalServerError(Type exceptionType, HttpStatusCode expectedStatusCode)
    {
        // Arrange
        var context = new DefaultHttpContext();
        var exception = (Exception)Activator.CreateInstance(exceptionType, "Test Exception");

        // Act
        await InvokeHandleExceptionAsync(context, exception);

        // Assert
        Assert.Equal((int)expectedStatusCode, context.Response.StatusCode);
    }

    [Fact]
    public async Task HandleExceptionAsync_WithRefreshTokenRotationException_KeepsTheExceptionTitle()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var exception = new RefreshTokenRotationException("The rotation has failed", "Authentication Error", new TimeoutException());

        // Act
        await InvokeHandleExceptionAsync(context, exception);

        // Assert
        var response = await ReadResponseBodyAsync(context);

        Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
        Assert.Equal("Authentication Error", response["Title"].ToString());
        Assert.Equal("The rotation has failed", response["Errors"][0].ToString());
    }

    [Fact]
    public async Task HandleExceptionAsync_WithUnexpectedException_SetsTheDefaultTitle()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // Act
        await InvokeHandleExceptionAsync(context, new Exception("Test Exception"));

        // Assert
        var response = await ReadResponseBodyAsync(context);

        Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
        Assert.Equal("Unexpected error", response["Title"].ToString());
    }

    private static async Task<JObject> ReadResponseBodyAsync(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        using var reader = new StreamReader(context.Response.Body);

        return JObject.Parse(await reader.ReadToEndAsync());
    }

    private async Task InvokeHandleExceptionAsync(HttpContext context, Exception exception)
    {
        var method = typeof(ExceptionMiddleware).GetMethod("HandleExceptionAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        await (Task)method.Invoke(_middleware, [context, exception]);
    }
}

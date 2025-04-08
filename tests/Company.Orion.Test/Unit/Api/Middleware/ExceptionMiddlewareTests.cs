using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Company.Orion.Api.Middleware;
using Company.Orion.Domain.Core.Exceptions;
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

    private async Task InvokeHandleExceptionAsync(HttpContext context, Exception exception)
    {
        var method = typeof(ExceptionMiddleware).GetMethod("HandleExceptionAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        await (Task)method.Invoke(_middleware, [context, exception]);
    }
}

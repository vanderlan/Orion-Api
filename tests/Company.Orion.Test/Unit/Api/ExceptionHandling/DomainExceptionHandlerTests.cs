using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Company.Orion.Api.ExceptionHandling;
using Company.Orion.Domain.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Company.Orion.Test.Unit.Api.ExceptionHandling;

public class DomainExceptionHandlerTests
{
    private readonly Mock<IProblemDetailsService> _problemDetailsServiceMock = new();
    private readonly DomainExceptionHandler _handler;
    private ProblemDetailsContext _writtenContext;

    public DomainExceptionHandlerTests()
    {
        _problemDetailsServiceMock
            .Setup(service => service.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .Callback<ProblemDetailsContext>(context => _writtenContext = context)
            .ReturnsAsync(true);

        _handler = new DomainExceptionHandler(_problemDetailsServiceMock.Object, Mock.Of<ILogger<DomainExceptionHandler>>());
    }

    [Theory]
    [InlineData(typeof(NotFoundException), HttpStatusCode.NotFound)]
    [InlineData(typeof(ConflictException), HttpStatusCode.Conflict)]
    [InlineData(typeof(UnauthorizedUserException), HttpStatusCode.Unauthorized)]
    [InlineData(typeof(BusinessException), HttpStatusCode.BadRequest)]
    [InlineData(typeof(RefreshTokenRotationException), HttpStatusCode.InternalServerError)]
    [InlineData(typeof(InvalidOperationException), HttpStatusCode.InternalServerError)]
    public void GetStatusCode_ForEachExceptionType_ReturnsTheMappedStatusCode(Type exceptionType, HttpStatusCode expected)
    {
        //arrange
        var exception = CreateException(exceptionType);

        //act
        var statusCode = DomainExceptionHandler.GetStatusCode(exception);

        //assert
        Assert.Equal((int)expected, statusCode);
    }

    [Fact]
    public async Task TryHandleAsync_WithBusinessException_WritesProblemDetailsWithTitleAndDetail()
    {
        //arrange
        var httpContext = new DefaultHttpContext();
        var exception = new ConflictException("The e-mail is already in use", "Conflict");

        //act
        var handled = await _handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        //assert
        Assert.True(handled);
        Assert.Equal(StatusCodes.Status409Conflict, httpContext.Response.StatusCode);
        Assert.NotNull(_writtenContext);
        Assert.Same(exception, _writtenContext.Exception);
        Assert.Equal(StatusCodes.Status409Conflict, _writtenContext.ProblemDetails.Status);
        Assert.Equal("Conflict", _writtenContext.ProblemDetails.Title);
        Assert.Equal("The e-mail is already in use", _writtenContext.ProblemDetails.Detail);
    }

    [Fact]
    public async Task TryHandleAsync_WithUnexpectedException_DoesNotLeakTheExceptionMessage()
    {
        //arrange
        var httpContext = new DefaultHttpContext();
        var exception = new InvalidOperationException("connection string is invalid");

        //act
        await _handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        //assert
        Assert.Equal(StatusCodes.Status500InternalServerError, httpContext.Response.StatusCode);
        Assert.Equal(DomainExceptionHandler.UnexpectedErrorTitle, _writtenContext.ProblemDetails.Title);
        Assert.DoesNotContain("connection string", _writtenContext.ProblemDetails.Detail);
    }

    [Fact]
    public async Task TryHandleAsync_WhenProblemDetailsServiceCannotWrite_WritesProblemJsonAsFallback()
    {
        //arrange
        _problemDetailsServiceMock
            .Setup(service => service.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .ReturnsAsync(false);

        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();

        //act
        var handled = await _handler.TryHandleAsync(httpContext, new NotFoundException("123"), CancellationToken.None);

        //assert
        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = JObject.Parse(await new StreamReader(httpContext.Response.Body).ReadToEndAsync());

        Assert.True(handled);
        Assert.StartsWith("application/problem+json", httpContext.Response.ContentType);
        Assert.Equal(StatusCodes.Status404NotFound, httpContext.Response.StatusCode);
        Assert.Equal(404, body.Value<int>("status"));
        Assert.Equal("Resource Not Found with id: 123", body.Value<string>("detail"));
    }

    private static Exception CreateException(Type exceptionType)
    {
        return exceptionType == typeof(NotFoundException)
            ? new NotFoundException("id")
            : (Exception)Activator.CreateInstance(exceptionType, "message");
    }
}

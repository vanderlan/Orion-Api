using Orion.Domain.Core.Exceptions;
using System;
using Xunit;

namespace Orion.Domain.Core.Tests.Exceptions;

public class BusinessExceptionTests
{
    [Fact]
    public void Constructor_WithMessageAndTitle_SetsProperties()
    {
        // Arrange
        var message = "Some message";
        var title = "Some title";

        // Act
        var exception = new BusinessException(message, title);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Equal(title, exception.Title);
    }

    [Theory]
    [InlineData("Some message")]
    public void Constructor_WithMessage_SetsMessageAndNullTitle(string message)
    {
        // Act
        var exception = new BusinessException(message);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Null(exception.Title);
    }

    [Theory]
    [InlineData("Some message", "Some inner exception message")]
    public void Constructor_WithMessageAndInnerException_SetsProperties(string message, string innerMessage)
    {
        // Arrange
        var innerException = new Exception(innerMessage);

        // Act
        var exception = new BusinessException(message, innerException);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Equal(innerException, exception.InnerException);
        Assert.Null(exception.Title);
    }
}

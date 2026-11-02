using System;
using Company.Orion.Domain.Core.Exceptions;
using Xunit;

namespace Company.Orion.Test.Unit.Domain.Exceptions;

public class RefreshTokenRotationExceptionTests
{
    [Fact]
    public void Constructor_WithMessageAndTitle_SetsProperties()
    {
        // Arrange
        var message = "Some message";
        var title = "Some title";

        // Act
        var exception = new RefreshTokenRotationException(message, title);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Equal(title, exception.Title);
    }

    [Fact]
    public void Constructor_WithMessage_SetsMessageAndNullTitle()
    {
        //arrange
        var message = "The message test";

        // Act
        var exception = new RefreshTokenRotationException(message);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Null(exception.Title);
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_SetsProperties()
    {
        // Arrange
        var innerMessage = "Some message inner exception";
        var message = "Error message";

        var innerException = new Exception(innerMessage);

        // Act
        var exception = new RefreshTokenRotationException(message, innerException);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Equal(innerException, exception.InnerException);
        Assert.Null(exception.Title);
    }

    [Fact]
    public void Constructor_WithMessageTitleAndInnerException_SetsProperties()
    {
        // Arrange
        var message = "Error message";
        var title = "Some title";
        var innerException = new Exception("Some message inner exception");

        // Act
        var exception = new RefreshTokenRotationException(message, title, innerException);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Equal(title, exception.Title);
        Assert.Equal(innerException, exception.InnerException);
    }

    [Fact]
    public void RefreshTokenRotationException_IsABusinessException()
    {
        // Arrange & Act
        var exception = new RefreshTokenRotationException("Some message");

        // Assert
        Assert.IsAssignableFrom<BusinessException>(exception);
    }
}

using System;
using Company.Orion.Domain.Core.Exceptions;
using Xunit;

namespace Company.Orion.Test.Unit.Domain.Exceptions;

public class BusinessExceptionTests
{
    [Fact]
    public void Constructor_WithMessageAndTitle_SetsProperties()
    {
        // Arrange
        var title = "Validation error";
        var message = "The message test";

        // Act
        var exception = new BusinessException(message, title);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Equal(title, exception.Title);
    }

    [Fact]
    public void Constructor_WithMessage_SetsMessageAndNullTitle()
    {
        // Act
        var message = "The message test";
        
        var exception = new BusinessException(message);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Null(exception.Title);
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_SetsProperties()
    {
        // Arrange
        var innerMessage = "Validation error";
        var message = "The message test";
        
        var innerException = new Exception(innerMessage);

        // Act
        var exception = new BusinessException(message, innerException);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Equal(innerException, exception.InnerException);
        Assert.Null(exception.Title);
    }
}

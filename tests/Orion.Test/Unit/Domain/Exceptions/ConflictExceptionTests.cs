using System;
using Orion.Domain.Core.Exceptions;
using Xunit;

namespace Orion.Test.Unit.Domain.Exceptions;

public class ConflictExceptionTests
{
    [Fact]
    public void Constructor_WithMessageAndTitle_SetsProperties()
    {
        // Arrange
        var message = "Some message";
        var title = "Some title";

        // Act
        var exception = new ConflictException(message, title);

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
        var exception = new ConflictException(message);

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
        var exception = new ConflictException(message, innerException);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Equal(innerException, exception.InnerException);
        Assert.Null(exception.Title);
    }
}
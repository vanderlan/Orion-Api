using System;
using Company.Orion.Domain.Core.Exceptions;
using Xunit;

namespace Company.Orion.Test.Unit.Domain.Exceptions;

public class UnauthorizedUseExceptionTests
{
    [Fact]
    public void Constructor_WithMessageAndTitle_SetsProperties()
    {
        // Arrange
        var title = "Unauthorized user";
        var message = "The user can't access this resource";

        // Act
        var exception = new UnauthorizedUserException(message, title);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Equal(title, exception.Title);
    }

    [Fact]
    public void Constructor_WithMessage_SetsMessageAndNullTitle()
    {
        //Arrange 
        var message = "Unauthorized user";

        // Act
        var exception = new UnauthorizedUserException(message);

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
        var exception = new UnauthorizedUserException(message, innerException);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Equal(innerException, exception.InnerException);
        Assert.Null(exception.Title);
    }
}
using System;
using Company.Orion.Domain.Core.Exceptions;
using Xunit;

namespace Company.Orion.Test.Unit.Domain.Exceptions;

public class NotFoundExceptionTests
{
    [Theory]
    [InlineData("10")]
    [InlineData("50")]
    [InlineData("35")]
    public void Constructor_WithMessage_SetsMessageAndNullTitle(string idNotfound)
    {
        // Act
        var exception = new NotFoundException(idNotfound);

        // Assert
        Assert.Equal($"Resource Not Found with id: {idNotfound}", exception.Message);
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
        var exception = new NotFoundException(message, innerException);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Equal(innerException, exception.InnerException);
        Assert.Null(exception.Title);
    }
}
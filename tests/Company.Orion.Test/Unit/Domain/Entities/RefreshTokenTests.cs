using System;
using Company.Orion.Domain.Core.Entities;
using Xunit;

namespace Company.Orion.Test.Unit.Domain.Entities;

public class RefreshTokenTests
{
    private static readonly DateTime Now = new(2025, 3, 21, 12, 0, 0, DateTimeKind.Utc);
    private static readonly TimeSpan Lifetime = TimeSpan.FromDays(7);

    [Theory]
    [InlineData(0, false)]
    [InlineData(6, false)]
    [InlineData(7, true)]
    [InlineData(30, true)]
    public void IsExpired_ComparesTheCreationDatePlusLifetimeWithNow(int ageInDays, bool expected)
    {
        //arrange
        var refreshToken = new RefreshToken
        {
            Email = "adm@orion.com",
            Refreshtoken = "token",
            CreatedAt = Now.AddDays(-ageInDays)
        };

        //act & assert
        Assert.Equal(expected, refreshToken.IsExpired(Now, Lifetime));
    }
}

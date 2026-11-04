using System.ComponentModel;
using Company.Orion.Domain.Core.Entities.Enuns;
using Company.Orion.Domain.Core.Extensions;
using Xunit;
using static Company.Orion.Domain.Core.Authentication.AuthorizationConfiguration;

namespace Company.Orion.Test.Unit.Domain.Extensions;

public class EnumExtensionsTests
{
    private enum Sample
    {
        [Description("described")]
        WithDescription,
        WithoutDescription
    }

    [Theory]
    [InlineData(UserProfile.Admin, Roles.Admin)]
    [InlineData(UserProfile.Operator, Roles.Operator)]
    public void Description_ReturnsTheDescriptionAttributeOfTheProfile(UserProfile profile, string expected)
    {
        Assert.Equal(expected, profile.Description());
    }

    [Fact]
    public void Description_WithoutAttribute_ReturnsEmpty()
    {
        Assert.Equal("described", Sample.WithDescription.Description());
        Assert.Equal(string.Empty, Sample.WithoutDescription.Description());
    }
}

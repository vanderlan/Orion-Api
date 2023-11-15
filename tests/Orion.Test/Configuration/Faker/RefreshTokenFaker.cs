using System;
using Bogus;
using Orion.Domain.Core.Entities;

namespace Orion.Test.Configuration.Faker;

public static class RefreshTokenFaker
{
    public static RefreshToken Get(string forUserEmail = null)
    {
        return new Faker<RefreshToken>()
           .RuleFor(o => o.Email, f => forUserEmail ?? f.Internet.Email())
           .RuleFor(o => o.Refreshtoken, f => Guid.NewGuid().ToString())
           .RuleFor(o => o.PublicId, f => Guid.NewGuid().ToString())
           .Generate();
    }
}

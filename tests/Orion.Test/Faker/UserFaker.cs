using System;
using Bogus;
using Orion.Api.AutoMapper.Input;
using Orion.Domain.Core.Entities;
using Orion.Domain.Core.Entities.Enuns;

namespace Orion.Test.Faker;

public class UserFaker
{
    public static User Get()
    {
        return new Faker<User>()
             .RuleFor(o => o.Name, f => f.Name.FullName())
             .RuleFor(o => o.Email, f => f.Internet.Email())
             .RuleFor(o => o.Password, f => f.Internet.Password())
             .RuleFor(o => o.Profile, f => UserProfile.Admin)
             .RuleFor(o => o.PublicId, f => Guid.NewGuid().ToString())
             .Generate();
    }

    public static UserInput GetInput()
    {
        return new Faker<UserInput>()
           .RuleFor(o => o.Name, f => f.Name.FullName())
           .RuleFor(o => o.Email, f => f.Internet.Email())
           .RuleFor(o => o.Password, f => f.Internet.Password())
           .RuleFor(o => o.Profile, f => UserProfile.Admin)
           .RuleFor(o => o.PublicId, f => Guid.NewGuid().ToString())
           .Generate();
    }
}

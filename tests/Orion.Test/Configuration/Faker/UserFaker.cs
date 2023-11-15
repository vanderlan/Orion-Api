using System;
using Bogus;
using Orion.Application.Core.Commands.UserCreate;
using Orion.Application.Core.Commands.UserUpdate;
using Orion.Domain.Core.Entities;
using Orion.Domain.Core.Entities.Enuns;
using Orion.Domain.Core.Extensions;

namespace Orion.Test.Configuration.Faker;

public static class UserFaker
{
    public static User GetDefaultSystemUser()
    {
        return new Faker<User>()
             .RuleFor(o => o.Name, f => f.Name.FullName())
             .RuleFor(o => o.Email, f => f.Internet.Email())
             .RuleFor(o => o.Password, f => "123".ToSha512())
             .RuleFor(o => o.Profile, f => UserProfile.Admin)
             .RuleFor(o => o.PublicId, f => Guid.NewGuid().ToString())
             .Generate();
    }

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

    public static UserCreateRequest GetUserCreateRequest()
    {
        return new Faker<UserCreateRequest>()
           .RuleFor(o => o.Name, f => f.Name.FullName())
           .RuleFor(o => o.Email, f => f.Internet.Email())
           .RuleFor(o => o.Password, f => f.Internet.Password())
           .RuleFor(o => o.Profile, f => UserProfile.Admin)
           .Generate();
    }

    public static UserUpdateRequest GetUserUpdateRequest()
    {
        return new Faker<UserUpdateRequest>()
            .RuleFor(o => o.Name, f => f.Name.FullName())
            .RuleFor(o => o.Email, f => f.Internet.Email())
            .RuleFor(o => o.Profile, f => UserProfile.Admin)
            .Generate();
    }
}

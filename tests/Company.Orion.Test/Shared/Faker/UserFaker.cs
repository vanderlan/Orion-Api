using System;
using Bogus;
using Company.Orion.Application.Core.UseCases.User.Commands.Create;
using Company.Orion.Application.Core.UseCases.User.Commands.Update;
using Company.Orion.Domain.Core.Entities;
using Company.Orion.Domain.Core.Entities.Enuns;
using Company.Orion.Domain.Core.Extensions;

namespace Company.Orion.Test.Shared.Faker;

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

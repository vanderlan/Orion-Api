using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Orion.Domain.Core.Repositories.UnitOfWork;
using Orion.Test.Integration.Setup;
using Orion.Test.Shared.Faker;
using Xunit;

namespace Orion.Test.Integration.UnitOfWork;

public class UnitOfWorkTests(IntegrationTestsFixture fixture) : IntegrationTestsBootstrapper(fixture)
{
    [Fact]
    public async Task CommitAsync_WithUniqueIndexViolation_ThrowsDbUpdateException()
    {
        // arrange
        using var scope = ServiceProvider.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();

        var user = UserFaker.Get();
        var user2 = UserFaker.Get();

        user2.Email = user.Email;

        await unitOfWork.UserRepository.AddAsync(user);
        await unitOfWork.UserRepository.AddAsync(user2);

        // act & assert
        await Assert.ThrowsAsync<DbUpdateException>(() => unitOfWork.CommitAsync());
    }

    //[Fact]
    //public async Task CommitAsync_WithValidInsert_SaveRegistry()
    //{
    //    // arrange
    //    using var scope = ServiceProvider.CreateScope();
    //    var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();

    //    var validUser = UserFaker.Get();

    //    // act
    //    await unitOfWork.UserRepository.AddAsync(validUser);

    //    Assert.Equal(0, validUser.UserId);

    //    // act
    //    await unitOfWork.CommitAsync();

    //    Assert.NotEqual(0, validUser.UserId);
    //}
}
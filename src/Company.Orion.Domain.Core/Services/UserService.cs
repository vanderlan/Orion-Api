using Microsoft.Extensions.Localization;
using System.Linq;
using System.Threading.Tasks;
using Company.Orion.Domain.Core.Extensions;
using Company.Orion.Crosscutting.Resources;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Company.Orion.Domain.Core.Entities;
using Company.Orion.Domain.Core.Exceptions;
using Company.Orion.Domain.Core.Filters;
using Company.Orion.Domain.Core.Repositories.UnitOfWork;
using Company.Orion.Domain.Core.Services.Interfaces;
using Company.Orion.Domain.Core.ValueObjects.Pagination;
using static Company.Orion.Crosscutting.Resources.Messages.MessagesKeys;

namespace Company.Orion.Domain.Core.Services;

public class UserService(IUnitOfWork unitOfWork, IStringLocalizer<OrionResources> resourceMessages)
    : IUserService
{
    public async Task<User> AddAsync(User user)
    {
        await ValidateUser(user);

        user.Password = user.Password.ToSha512();

        var added = await unitOfWork.UserRepository.AddAsync(user);
        await unitOfWork.CommitAsync();

        return added;
    }

    public async Task DeleteAsync(string publicId)
    {
        var user = await FindByIdAsync(publicId);

        if (user == null)
            throw new NotFoundException(publicId);

        await unitOfWork.UserRepository.DeleteAsync(publicId);
        await unitOfWork.CommitAsync();
    }

    public async Task<User> FindByIdAsync(string publicId)
    {
        return await unitOfWork.UserRepository.GetByIdAsync(publicId);
    }

    public async Task<(User User, RefreshToken RefreshToken)> SignInWithCredentialsAsync(string email, string password)
    {
        var user = await unitOfWork.UserRepository.LoginAsync(email, password.ToSha512());

        if (user is not null)
        {
            var refreshToken = await AddRefreshTokenAsync(user.Email);
            return (user, refreshToken);
        }

        throw new UnauthorizedUserException(resourceMessages[UserMessages.InvalidCredentials], resourceMessages[ExceptionsTitles.AuthenticationError]);
    }

    public async Task UpdateAsync(User user)
    {
        var entitySaved = await FindByIdAsync(user.PublicId);

        await ValidateUser(user);

        entitySaved.Email = user.Email;
        entitySaved.Name = user.Name;
        entitySaved.Profile = user.Profile;

        unitOfWork.UserRepository.Update(entitySaved);

        await unitOfWork.CommitAsync();
    }

    private async Task<RefreshToken> AddRefreshTokenAsync(string userEmail)
    {
        var existentRefreshToken = (await unitOfWork.RefreshTokenRepository.SearchByAsync(x => x.Email == userEmail)).FirstOrDefault();

        if (existentRefreshToken is not null)
            return existentRefreshToken;

        var refreshToken = new RefreshToken
        {
            Email = userEmail,
            Refreshtoken = Guid.NewGuid().ToString().ToSha512()
        };

        var addedRefreshToken = await unitOfWork.RefreshTokenRepository.AddAsync(refreshToken);
        await unitOfWork.CommitAsync();

        return addedRefreshToken;
    }

    public async Task<(User User, RefreshToken RefreshToken)> SignInWithRefreshTokenAsync(string refreshToken, string expiredToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new UnauthorizedUserException(
                resourceMessages[UserMessages.InvalidRefreshToken],
                resourceMessages[ExceptionsTitles.AuthenticationError]
            );
        }

        var email = GetClaimFromJwtToken(expiredToken, ClaimTypes.Email);

        var userRefreshToken = (await unitOfWork.RefreshTokenRepository.SearchByAsync(x => x.Refreshtoken.Equals(refreshToken) && x.Email == email)).FirstOrDefault();

        if (userRefreshToken is not null)
        {
            var user = (await unitOfWork.UserRepository.SearchByAsync(x => x.Email == userRefreshToken.Email)).FirstOrDefault();

            if (user is not null)
            {
                await unitOfWork.RefreshTokenRepository.DeleteAsync(userRefreshToken.PublicId);
                var newRefreshToken = await AddRefreshTokenAsync(user.Email);

                return (user, newRefreshToken);
            }
        }

        throw new UnauthorizedUserException(resourceMessages[UserMessages.InvalidRefreshToken], resourceMessages[ExceptionsTitles.AuthenticationError]);
    }

    private string GetClaimFromJwtToken(string jtwToken, string claimName)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(jtwToken);
            var claimValue = jwtSecurityToken.Claims.First(claim => claim.Type == claimName).Value;

            return claimValue;
        }
        catch (Exception)
        {
            throw new UnauthorizedUserException(resourceMessages[UserMessages.InvalidRefreshToken], resourceMessages[ExceptionsTitles.AuthenticationError]);
        }
    }

    private async Task ValidateUser(User user)
    {
        var userFound = await unitOfWork.UserRepository.FindByEmailAsync(user.Email);

        if (userFound != null && userFound.PublicId != user.PublicId)
            throw new ConflictException(resourceMessages[UserMessages.EmailExists], resourceMessages[ExceptionsTitles.ValidationError]);
    }

    public async Task<PagedList<User>> ListPaginateAsync(UserFilter filter)
    {
        return await unitOfWork.UserRepository.ListPaginateAsync(filter);
    }

    public async Task ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        var user = await FindByIdAsync(userId) ?? throw new NotFoundException(userId);

        if (user.Password != currentPassword.ToSha512())
            throw new BusinessException(resourceMessages[UserMessages.InvalidPassword]);

        user.Password = newPassword.ToSha512();

        unitOfWork.UserRepository.Update(user);

        await unitOfWork.CommitAsync();
    }
}

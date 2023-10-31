using Microsoft.Extensions.Localization;
using System.Linq;
using System.Threading.Tasks;
using Orion.Domain.Core.Extensions;
using Orion.Croscutting.Resources;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Orion.Domain.Core.Entities;
using Orion.Domain.Core.Exceptions;
using Orion.Domain.Core.Filters;
using Orion.Domain.Core.Repositories.UnitOfWork;
using Orion.Domain.Core.Services.Interfaces;
using Orion.Domain.Core.ValueObjects.Pagination;
using static Orion.Croscutting.Resources.Messages.MessagesKeys;

namespace Orion.Domain.Core.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStringLocalizer<OrionResources> _messages;

    public UserService(IUnitOfWork unitOfWork, IStringLocalizer<OrionResources> resourceMessages)
    {
        _unitOfWork = unitOfWork;
        _messages = resourceMessages;
    }

    public async Task<User> AddAsync(User user)
    {
        await ValidateUser(user, validatePassword: true);

        user.Password = user.Password.ToSha512();

        var added = await _unitOfWork.UserRepository.AddAsync(user);
        await _unitOfWork.CommitAsync();

        return added;
    }

    public async Task DeleteAsync(string publicId)
    {
        await _unitOfWork.UserRepository.DeleteAsync(publicId);
        await _unitOfWork.CommitAsync();
    }

    public async Task<User> FindByIdAsync(string publicId)
    {
        return await _unitOfWork.UserRepository.GetByIdAsync(publicId);
    }

    public async Task<(User User, RefreshToken RefreshToken)> SignInWithCredentialsAsync(string email, string password)
    {
        var user = await _unitOfWork.UserRepository.LoginAsync(email, password.ToSha512());

        if (user is not null)
        {
            var refreshToken = await AddRefreshTokenAsync(user.Email);
            return (user, refreshToken);
        }

        throw new UnauthorizedUserException(_messages[UserMessages.InvalidCredentials], _messages[ExceptionsTitles.AuthenticationError]);
    }

    public async Task UpdateAsync(User user)
    {
        var entitySaved = await FindByIdAsync(user.PublicId);

        await ValidateUser(user, validatePassword: false);

        entitySaved.Email = user.Email;
        entitySaved.Name = user.Name;

        _unitOfWork.UserRepository.Update(entitySaved);

        await _unitOfWork.CommitAsync();
    }

    private async Task<RefreshToken> AddRefreshTokenAsync(string userEmail)
    {
        var existantRefreshToken = (await _unitOfWork.RefreshTokenRepository.SearchByAsync(x => x.Email == userEmail)).FirstOrDefault();

        if (existantRefreshToken is not null)
            return existantRefreshToken;

        var refreshToken = new RefreshToken
        {
            Email = userEmail,
            Refreshtoken = Guid.NewGuid().ToString().ToSha512()
        };

        var addedRefreshToken = await _unitOfWork.RefreshTokenRepository.AddAsync(refreshToken);
        await _unitOfWork.CommitAsync();

        return addedRefreshToken;
    }

    public async Task<(User User, RefreshToken RefreshToken)> SignInWithRefreshTokenAsync(string refreshToken, string expiredToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new UnauthorizedUserException(
                _messages[UserMessages.InvalidRefreshToken],
                _messages[ExceptionsTitles.AuthenticationError]
            );
        }

        var email = GetClaimFromJwtToken(expiredToken, ClaimTypes.Email);

        var userRefreshToken = (await _unitOfWork.RefreshTokenRepository.SearchByAsync(x => x.Refreshtoken.Equals(refreshToken) && x.Email == email)).FirstOrDefault();

        if (userRefreshToken is not null)
        {
            var user = (await _unitOfWork.UserRepository.SearchByAsync(x => x.Email == userRefreshToken.Email)).FirstOrDefault();

            if (user is not null)
            {
                await _unitOfWork.RefreshTokenRepository.DeleteAsync(userRefreshToken.PublicId);
                var newRefreshToken = await AddRefreshTokenAsync(user.Email);

                return (user, newRefreshToken);
            }
        }

        throw new UnauthorizedUserException(_messages[UserMessages.InvalidRefreshToken], _messages[ExceptionsTitles.AuthenticationError]);
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
            throw new UnauthorizedUserException(_messages[UserMessages.InvalidRefreshToken], _messages[ExceptionsTitles.AuthenticationError]);
        }
    }

    private async Task ValidateUser(User user, bool validatePassword)
    {
        if (string.IsNullOrEmpty(user.Password) && validatePassword)
            throw new BusinessException(_messages[UserMessages.EmptyPasword]);

        var userFound = await _unitOfWork.UserRepository.FindByEmailAsync(user.Email);

        if (userFound != null && userFound.PublicId != user.PublicId)
            throw new ConflictException(_messages[UserMessages.EmailExists], _messages[ExceptionsTitles.ValidationError]);
    }

    public async Task<PagedList<User>> ListPaginateAsync(UserFilter filter)
    {
        return await _unitOfWork.UserRepository.ListPaginateAsync(filter);
    }
}

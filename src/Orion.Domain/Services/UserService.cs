using Microsoft.Extensions.Localization;
using System.Linq;
using System.Threading.Tasks;
using Orion.Domain.Exceptions;
using Orion.Domain.Extensions;
using Orion.Domain.Services.Interfaces;
using Orion.Domain.Repositories.UnitOfWork;
using Orion.Domain.Entities.Filter;
using Orion.Domain.Entities.ValueObjects.Pagination;
using Orion.Resources;
using Orion.Domain.Entities;
using static Orion.Resources.Messages.MessagesKeys;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace Orion.Domain.Services;

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
        await ValidateUser(user);

        user.Password = user.Password.ToSha512();

        var added = await _unitOfWork.UserRepository.AddAsync(user);
        await _unitOfWork.CommitAsync();

        return added;
    }

    private async Task ValidateUser(User user)
    {
        if (string.IsNullOrEmpty(user.Password))
            throw new BusinessException(_messages[UserMessages.EmptyPasword]);

        var userFound = await _unitOfWork.UserRepository.FindByEmailAsync(user.Email);

        if (userFound != null && userFound.PublicId != user.PublicId)
            throw new ConflictException(_messages[UserMessages.EmailExists], _messages[ExceptionsTitles.ValidationError]);
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

    public async Task<User> LoginAsync(string email, string password)
    {
        var user = await _unitOfWork.UserRepository.LoginAsync(email, password.ToSha512());

        return user ?? throw new UnauthorizedUserException(_messages[UserMessages.InvalidCredentials], _messages[ExceptionsTitles.AuthenticationError]);
    }

    public async Task UpdateAsync(User user)
    {
        var entitySaved = await FindByIdAsync(user.PublicId);

        await ValidateUser(user);

        entitySaved.Email = user.Email;
        entitySaved.Name = user.Name;
        entitySaved.Password = user.Password.ToSha512();

        _unitOfWork.UserRepository.Update(entitySaved);

        await _unitOfWork.CommitAsync();
    }

    public async Task<RefreshToken> AddRefreshTokenAsync(RefreshToken refreshToken)
    {
        var existantRefresToken = await _unitOfWork.RefreshTokenRepository.SearchByAsync(x => x.Email == refreshToken.Email);

        if (existantRefresToken.Any())
            return existantRefresToken.First();

        var added = await _unitOfWork.RefreshTokenRepository.AddAsync(refreshToken);
        await _unitOfWork.CommitAsync();

        return added;
    }

    public async Task<User> GetUserByRefreshTokenAsync(string refreshToken, string expiredToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new UnauthorizedUserException(
                _messages[UserMessages.InvalidRefreshToken],
                _messages[ExceptionsTitles.AuthenticationError]
            );
        }

        var email = GetClaimFromJtwToken(expiredToken, JwtRegisteredClaimNames.Email);

        var refreshTokens = (await _unitOfWork.RefreshTokenRepository.SearchByAsync(x => x.Refreshtoken.Equals(refreshToken) && x.Email == email)).ToList();

        if (refreshTokens != null && refreshTokens.Any())
        {
            var user = (await _unitOfWork.UserRepository.SearchByAsync(x => x.Email == refreshTokens.First().Email)).FirstOrDefault();

            if (user is not null)
            {
                await _unitOfWork.RefreshTokenRepository.DeleteAsync(refreshTokens.First().PublicId);
                await _unitOfWork.CommitAsync();

                return user;
            }
        }

        throw new UnauthorizedUserException(_messages[UserMessages.InvalidRefreshToken], _messages[ExceptionsTitles.AuthenticationError]);
    }

    private string GetClaimFromJtwToken(string jtwToken, string claimName)
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

    public async Task<PagedList<User>> ListPaginateAsync(BaseFilter<User> filter)
    {
        return await _unitOfWork.UserRepository.ListPaginateAsync(filter);
    }
}

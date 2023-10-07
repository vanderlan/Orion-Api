using Microsoft.Extensions.Localization;
using System.Linq;
using System.Threading.Tasks;
using Orion.Domain.Exceptions;
using Orion.Domain.Extensions;
using Orion.Domain.Interfaces;
using Orion.Domain.Repositories.UnitOfWork;
using Orion.Entities.Domain;
using Orion.Entities.Filter;
using Orion.Entities.ValueObjects.Pagination;
using Orion.Resources;
using static Orion.Resources.Messages.MessagesKeys;

namespace Orion.Domain.Implementation
{
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

        public async Task<User> GetUserByRefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new UnauthorizedUserException(
                    _messages[UserMessages.InvalidRefreshToken],
                    _messages[ExceptionsTitles.AuthenticationError]
                );
            }

            var token = await _unitOfWork.RefreshTokenRepository.SearchByAsync(x => x.Refreshtoken.Equals(refreshToken));

            if (token != null && token.Any())
            {
                var user = await _unitOfWork.UserRepository.SearchByAsync(x => x.Email == token.First().Email);

                if (user.Any())
                {
                    await _unitOfWork.RefreshTokenRepository.DeleteAsync(token.First().PublicId);
                    await _unitOfWork.CommitAsync();

                    return user?.First() ?? throw new UnauthorizedUserException(_messages[UserMessages.InvalidRefreshToken], _messages[ExceptionsTitles.AuthenticationError]);
                }
            }

            throw new UnauthorizedUserException(_messages[UserMessages.InvalidRefreshToken], _messages[ExceptionsTitles.AuthenticationError]);
        }

        public async Task<PagedList<User>> ListPaginateAsync(UserFilter filter)
        {
            return await _unitOfWork.UserRepository.ListPaginateAsync(filter);
        }
    }
}

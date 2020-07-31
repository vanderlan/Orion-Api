using Microsoft.Extensions.Localization;
using System.Linq;
using System.Threading.Tasks;
using VBaseProject.Data.UnitOfWork;
using VBaseProject.Entities.Domain;
using VBaseProject.Entities.Filter;
using VBaseProject.Entities.ValueObjects.Pagination;
using VBaseProject.Resources;
using VBaseProject.Service.Exceptions;
using VBaseProject.Service.Extensions;
using VBaseProject.Service.Interfaces;
using static VBaseProject.Resources.Messages.MessagesKeys;

namespace VBaseProject.Service.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWorkEntity _unitOfWork;
        private readonly IStringLocalizer<VBaseProjectResources> _messages;

        public UserService(IUnitOfWorkEntity unitOfWork, IStringLocalizer<VBaseProjectResources> resourceMessages)
        {
            _unitOfWork = unitOfWork;
            _messages = resourceMessages;
        }

        public async Task<User> AddAsync(User user)
        {
            if (string.IsNullOrEmpty(user.Password))
                throw new BusinessException(_messages[UserMessages.EmptyPasword]);

            await ValidateUser(user);

            user.Password = user.Password.ToSHA512();
            var added = await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();

            return added;
        }

        private async Task ValidateUser(User user)
        {
            var userFound = await _unitOfWork.UserRepository.FindByEmailAsync(user.Email);

            if (userFound != null)
            {
                if (userFound.PublicId != user.PublicId)
                    throw new ConflictException(_messages[UserMessages.EmailExists], _messages[ExceptionsTitles.ValidationError]);
            }
        }

        public async Task DeleteAsync(string id)
        {
            await _unitOfWork.UserRepository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();
        }

        public async Task<User> FindByIdAsync(string id)
        {
            return await _unitOfWork.UserRepository.FindByIdAsync(id);
        }

        public async Task<User> LoginAsync(string email, string password)
        {
            var user = await _unitOfWork.UserRepository.LoginAsync(email, password.ToSHA512());

            return user ?? throw new UnauthorizedUserException(_messages[UserMessages.InvalidCredentials], _messages[ExceptionsTitles.AuthenticationError]);
        }

        public async Task UpdateAsync(User user)
        {
            var entitySaved = await FindByIdAsync(user.PublicId);

            await ValidateUser(user);

            entitySaved.Email = user.Email;
            entitySaved.LastName = user.LastName;
            entitySaved.FirstName = user.FirstName;

            _unitOfWork.UserRepository.Update(entitySaved);

            await _unitOfWork.CommitAsync();
        }

        public async Task<RefreshToken> AddRefreshToken(RefreshToken refreshToken)
        {
            var existentRt = await _unitOfWork.RefreshTokenRepository.GetBy(x => x.Email == refreshToken.Email);

            if (existentRt.Any())
                return existentRt.First();

            var added = await _unitOfWork.RefreshTokenRepository.AddAsync(refreshToken);
            await _unitOfWork.CommitAsync();

            return added;
        }

        public async Task<User> GetUserByRefreshToken(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new UnauthorizedUserException(
                    _messages[UserMessages.InvalidRefreshToken], 
                    _messages[ExceptionsTitles.AuthenticationError]
                );
            }

            var token = await _unitOfWork.RefreshTokenRepository.GetBy(x => x.Refreshtoken.Equals(refreshToken));

            if (token != null && token.Any())
            {
                var user = await _unitOfWork.UserRepository.GetBy(x => x.Email == token.First().Email);

                if (user.Any())
                {
                    await _unitOfWork.RefreshTokenRepository.DeleteAsync(token.First().PublicId);
                    await _unitOfWork.CommitAsync();

                    return user?.First() ?? throw new UnauthorizedUserException(_messages[UserMessages.InvalidRefreshToken], _messages[ExceptionsTitles.AuthenticationError]);
                }
            }

            throw new UnauthorizedUserException(_messages[UserMessages.InvalidRefreshToken], _messages[ExceptionsTitles.AuthenticationError]);
        }

        public async Task<PagedList<User>> ListPaginate(UserFilter filter)
        {
            return await  _unitOfWork.UserRepository.ListPaginate(filter);
        }
    }
}

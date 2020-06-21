using Microsoft.Extensions.Localization;
using System.Linq;
using System.Threading.Tasks;
using VBaseProject.Data.UnitOfWork;
using VBaseProject.Entities.Domain;
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
        private readonly IStringLocalizer<VBaseProjectResources> _resourceMessages;

        public UserService(IUnitOfWorkEntity unitOfWork, IStringLocalizer<VBaseProjectResources> resourceMessages)
        {
            _unitOfWork = unitOfWork;
            _resourceMessages = resourceMessages;
        }

        public async Task<User> AddAsync(User entity)
        {
            if (string.IsNullOrEmpty(entity.Password))
            {
                throw new BusinessException(_resourceMessages[UserMessages.EmptyPasword]);
            }

            entity.Password = entity.Password.ToSHA512();
            var added = await _unitOfWork.UserRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();

            return added;
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

            return user ?? throw new UnauthorizedUserException(_resourceMessages[UserMessages.InvalidCredentials], _resourceMessages[AutheticationMessages.AuthenticationError]);
        }

        public async Task UpdateAsync(User entity)
        {
            var entitySaved = await FindByIdAsync(entity.PublicId);

            entitySaved.Email = entity.FirstName;
            entitySaved.LastName = entity.LastName;
            entitySaved.FirstName = entity.FirstName;

            _unitOfWork.UserRepository.Update(entitySaved);

            await _unitOfWork.CommitAsync();
        }

        public async Task<RefreshToken> AddRefreshToken(RefreshToken refreshToken)
        {
            var added = await _unitOfWork.RefreshTokenRepository.AddAsync(refreshToken);
            await _unitOfWork.CommitAsync();

            return added;
        }

        public async Task<User> GetUserByRefreshToken(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new UnauthorizedUserException(_resourceMessages[UserMessages.InvalidRefreshToken], _resourceMessages[AutheticationMessages.AuthenticationError]);
            }

            var token = await _unitOfWork.RefreshTokenRepository.GetBy(x => x.Refreshtoken.Equals(refreshToken));

            if (token != null && token.Any())
            {
                var user = await _unitOfWork.UserRepository.GetBy(x => x.Email == token.First().Email);

                if (user.Any())
                {
                    await _unitOfWork.RefreshTokenRepository.DeleteAsync(token.First().PublicId);
                    await _unitOfWork.CommitAsync();

                    return user?.First() ?? throw new UnauthorizedUserException(_resourceMessages[UserMessages.InvalidRefreshToken], _resourceMessages[AutheticationMessages.AuthenticationError]);
                }
            }

            throw new UnauthorizedUserException(_resourceMessages[UserMessages.InvalidRefreshToken], _resourceMessages[AutheticationMessages.AuthenticationError]);
        }
    }
}

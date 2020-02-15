using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VBaseProject.Data.UnitOfWork;
using VBaseProject.Entities.Domain;
using VBaseProject.Resources;
using VBaseProject.Service.Exceptions;
using VBaseProject.Service.Extensions;
using VBaseProject.Service.Interfaces;

namespace VBaseProject.Service.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWorkEntity unitOfWork;
        private readonly IStringLocalizer<VBaseProjectResources> messages;

        public UserService(IUnitOfWorkEntity unitOfWork, IStringLocalizer<VBaseProjectResources> messages)
        {
            this.unitOfWork = unitOfWork;
            this.messages = messages;
        }

        public async Task<User> AddAsync(User entity)
        {
            if (string.IsNullOrEmpty(entity.Password))
                //TODO: Use messages
                throw new BusinessException("User.EmptyPassword");

            entity.Password = entity.Password.ToSHA512();
            var added = await unitOfWork.UserRepository.AddAsync(entity);
            await unitOfWork.CommitAsync();

            return added;
        }

        public async Task DeleteAsync(string id)
        {
            await unitOfWork.UserRepository.DeleteAsync(id);
            await unitOfWork.CommitAsync();
        }

        public async Task<User> FindByIdAsync(string id)
        {
            return await unitOfWork.UserRepository.FindByIdAsync(id);
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await unitOfWork.UserRepository.GetBy(x => x.UserId > 0);
        }

        public async Task<User> LoginAsync(string email, string password)
        {
            return await unitOfWork.UserRepository.LoginAsync(email, password.ToSHA512());
        }

        public async Task UpdateAsync(User entity)
        {
            var entitySaved = await FindByIdAsync(entity.PublicId);

            entitySaved.Email = entity.FirstName;
            entitySaved.LastName = entity.LastName;
            entitySaved.FirstName = entity.FirstName;

            unitOfWork.UserRepository.Update(entitySaved);

            await unitOfWork.CommitAsync();
        }

        public async Task<RefreshToken> AddRefreshToken(RefreshToken refreshToken)
        {
            var added = await unitOfWork.RefreshTokenRepository.AddAsync(refreshToken);
            await unitOfWork.CommitAsync();

            return added;
        }

        public async Task<User> GetUserByRefreshToken(string refresh)
        {
            var refreshToken = await unitOfWork.RefreshTokenRepository.GetBy(x => x.Refreshtoken == refresh);

            if (refreshToken != null && refreshToken.Any())
            {
                var user = await unitOfWork.UserRepository.GetBy(x => x.Email == refreshToken.First().Email);

                if (user.Any())
                {
                    return user.First();
                }
            }
            return null;
        }
    }
}

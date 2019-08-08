using Invest.Data.UnitOfWork;
using Invest.Entities.Domain;
using Invest.Service.Interfaces;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Invest.Service.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWorkEntity unitOfWork;

        public UserService(IUnitOfWorkEntity unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<User> AddAsync(User entity)
        {
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
            return await unitOfWork.UserRepository.LoginAsync(email, ToSHA512(password));
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

        public static string ToSHA512(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            var crypt = new SHA512Managed();
            byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(text), 0, Encoding.ASCII.GetByteCount(text));
            var stringBuilder = new StringBuilder();

            foreach (byte theByte in crypto)
            {
                stringBuilder.Append(theByte.ToString("x2"));
            }

            return stringBuilder.ToString();
        }
    }
}

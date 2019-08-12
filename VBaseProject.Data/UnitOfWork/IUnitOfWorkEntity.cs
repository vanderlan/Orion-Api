using System;
using System.Threading.Tasks;
using VBaseProject.Data.Repository.Interfaces;

namespace VBaseProject.Data.UnitOfWork
{
    public interface IUnitOfWorkEntity : IDisposable
    {
        ICustomerRepository CustomerRepository { get; }
        IUserRepository UserRepository { get; }
        IRefreshTokenRepository RefreshTokenRepository { get; }
        Task CommitAsync();
        void DiscardChanges();
    }
}

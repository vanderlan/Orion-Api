using VBaseProject.Data.Repository.Interfaces;
using System;
using System.Threading.Tasks;

namespace VBaseProject.Data.UnitOfWork
{
    public interface IUnitOfWorkEntity : IDisposable
    {
        ICustomerRepository CustomerRepository { get; }
        IUserRepository UserRepository { get; }
        Task CommitAsync();
        void DiscardChanges();
    }
}

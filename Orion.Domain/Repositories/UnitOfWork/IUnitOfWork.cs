using System.Threading.Tasks;

namespace Orion.Domain.Repositories.UnitOfWork
{
    public interface IUnitOfWork
    {
        ICustomerRepository CustomerRepository { get; }
        IUserRepository UserRepository { get; }
        IRefreshTokenRepository RefreshTokenRepository { get; }
        Task CommitAsync();
        void DiscardChanges();
    }
}

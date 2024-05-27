using System;
using System.Threading.Tasks;

namespace Company.Orion.Domain.Core.Repositories.UnitOfWork;

public interface IUnitOfWork: IDisposable
{
    IUserRepository UserRepository { get; }
    IRefreshTokenRepository RefreshTokenRepository { get; }
    Task CommitAsync();
    void DiscardChanges();
}

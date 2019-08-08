using Invest.Data.Repository.Interfaces;
using System;
using System.Threading.Tasks;

namespace Invest.Data.UnitOfWork
{
    public interface IUnitOfWorkEntity : IDisposable
    {
        IAssetRepository AssetRepository { get; }
        IUserRepository UserRepository { get; }
        Task CommitAsync();
        void DiscardChanges();
    }
}

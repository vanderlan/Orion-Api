using VBaseProject.Data.Context;
using VBaseProject.Data.Repository.Generic;
using VBaseProject.Domain.Repositories;
using VBaseProject.Entities.Domain;

namespace VBaseProject.Data.Repository.Implementations
{
    internal class RefreshTokenRepository : BaseEntityRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(DataContext context) : base(context)
        {
        }
    }
}

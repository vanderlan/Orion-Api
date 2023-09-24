using Orion.Data.Context;
using Orion.Data.Repository.Generic;
using Orion.Domain.Repositories;
using Orion.Entities.Domain;

namespace Orion.Data.Repository.Implementations
{
    internal class RefreshTokenRepository : BaseEntityRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(DataContext context) : base(context)
        {
        }
    }
}

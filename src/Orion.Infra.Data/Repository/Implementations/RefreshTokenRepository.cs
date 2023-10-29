using Orion.Domain.Core.Entities;
using Orion.Domain.Core.Repositories;
using Orion.Infra.Data.Context;
using Orion.Infra.Data.Repository.Generic;

namespace Orion.Infra.Data.Repository.Implementations;

internal class RefreshTokenRepository : BaseEntityRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(DataContext context) : base(context)
    {
    }
}

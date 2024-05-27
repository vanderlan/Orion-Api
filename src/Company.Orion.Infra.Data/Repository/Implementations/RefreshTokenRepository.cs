using Company.Orion.Domain.Core.Entities;
using Company.Orion.Domain.Core.Repositories;
using Company.Orion.Infra.Data.Context;
using Company.Orion.Infra.Data.Repository.Generic;

namespace Company.Orion.Infra.Data.Repository.Implementations;

internal class RefreshTokenRepository(DataContext context)
    : BaseEntityRepository<RefreshToken>(context), IRefreshTokenRepository;

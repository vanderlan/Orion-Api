using Company.Orion.Domain.Core.Entities;
using Company.Orion.Domain.Core.Repositories;
using Company.Orion.Infra.Data.Mongo.Context;
using Company.Orion.Infra.Data.Mongo.Repository.Generic;

namespace Company.Orion.Infra.Data.Mongo.Repository.Implementations;

internal class MongoRefreshTokenRepository(MongoDataContext context)
    : MongoBaseEntityRepository<RefreshToken>(context), IRefreshTokenRepository;

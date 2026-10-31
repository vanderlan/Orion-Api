using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Company.Orion.Domain.Core.Entities;
using Company.Orion.Domain.Core.Filters;
using Company.Orion.Domain.Core.Repositories;
using Company.Orion.Domain.Core.ValueObjects.Pagination;
using Company.Orion.Infra.Data.Mongo.Context;
using Company.Orion.Infra.Data.Mongo.Repository.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Company.Orion.Infra.Data.Mongo.Repository.Implementations;

public sealed class MongoUserRepository(MongoDataContext context) : MongoBaseEntityRepository<User>(context), IUserRepository
{
    public async Task<User> LoginAsync(string email, string password)
    {
        return await Collection
            .Find(user => user.Email == email && user.Password == password)
            .FirstOrDefaultAsync();
    }

    public async Task<User> FindByEmailAsync(string email)
    {
        return await Collection.Find(user => user.Email == email).FirstOrDefaultAsync();
    }

    public async Task<PagedList<User>> ListPaginateAsync(UserFilter filter)
    {
        var filterDefinition = ApplyFilters(filter);

        var pagination = (filter.Page * filter.Quantity) - filter.Quantity;

        var entityList = await Collection.Find(filterDefinition)
            .SortBy(user => user.CreatedAt)
            .Skip(pagination)
            .Limit(filter.Quantity)
            .ToListAsync();

        var count = await Collection.CountDocumentsAsync(filterDefinition);

        return new PagedList<User>(entityList, (int)count);
    }

    private static FilterDefinition<User> ApplyFilters(UserFilter filter)
    {
        var filterBuilder = Builders<User>.Filter;
        var filterDefinition = filterBuilder.Empty;

        if (!string.IsNullOrWhiteSpace(filter.Query))
        {
            //Case insensitive "contains", equivalent to the Contains used by the relational providers.
            filterDefinition &= filterBuilder.Regex(user => user.Name,
                new BsonRegularExpression(Regex.Escape(filter.Query), "i"));
        }

        return filterDefinition;
    }
}

using Invest.Data.Context;
using Invest.Data.Repository.Generic;
using Invest.Data.Repository.Interfaces;
using Invest.Entities.Domain;

namespace Invest.Data.Repository.Implementations
{
    internal class AssetRepository : BaseEntityRepository<Asset>, IAssetRepository
    {
        public AssetRepository(DataContext context) : base(context)
        {
        }

    }
}

using Invest.Data.UnitOfWork;
using Invest.Entities.Domain;
using Invest.Service.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Invest.Service.Implementation
{
    public class AssetService : IAssetService
    {
        private readonly IUnitOfWorkEntity unitOfWork;

        public AssetService(IUnitOfWorkEntity unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<Asset> AddAsync(Asset entity)
        {
            var added = await unitOfWork.AssetRepository.AddAsync(entity);
            await unitOfWork.CommitAsync();

            return added;
        }

        public async Task DeleteAsync(string id)
        {
            await unitOfWork.AssetRepository.DeleteAsync(id);
            await unitOfWork.CommitAsync();
        }

        public async Task<Asset> FindByIdAsync(string id)
        {
            return await unitOfWork.AssetRepository.FindByIdAsync(id);
        }

        public async Task<IEnumerable<Asset>> GetAll()
        {
            return await unitOfWork.AssetRepository.GetBy(x => x.AssetId > 0);
        }

        public async Task UpdateAsync(Asset entity)
        {
            var entitySaved = await FindByIdAsync(entity.PublicId);

            entitySaved.Code = entity.Code;
            entitySaved.Description = entity.Description;
            entitySaved.StockExchangeId = entity.StockExchangeId;
            entitySaved.Price = entity.Price;
            entitySaved.CompanyId = entity.CompanyId;

            unitOfWork.AssetRepository.Update(entitySaved);
            await unitOfWork.CommitAsync();
        }
    }
}

using AutoMapper;
using Invest.Entities.Domain;

namespace Invest.Api.AutoMapper.Config
{
    public class InputToDomainProfile : Profile
    {
        public InputToDomainProfile()
        {
            CreateMap<AssetInput, Asset>()
                .ForMember(x => x.AssetId, opt => opt.Ignore())
                .ForMember(x => x.LastUpdated, opt => opt.Ignore())
                .ForMember(x => x.CreatedAt, opt => opt.Ignore());
        }
    }
}

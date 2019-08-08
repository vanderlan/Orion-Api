using AutoMapper;
using Invest.Entities.Domain;

namespace Invest.Api.AutoMapper.Config
{
    public class DomainToOutputProfile : Profile
    {
        public DomainToOutputProfile()
        {
            CreateMap<Asset, AssetOutput>();

            CreateMap<User, UserOutput>()
                .ForMember(x => x.ProfileDescription, opt => opt.Ignore());
        }
    }
}

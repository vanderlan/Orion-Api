using AutoMapper;
using Orion.Api.AutoMapper.Input;
using Orion.Domain.Entities;

namespace Orion.Api.AutoMapper.Config
{
    public class InputToDomainProfile : Profile
    {
        public InputToDomainProfile()
        {
            CreateMap<CustomerInput, Customer>()
                .ForMember(x => x.CustomerId, opt => opt.Ignore())
                .ForMember(x => x.LastUpdated, opt => opt.Ignore())
                .ForMember(x => x.CreatedAt, opt => opt.Ignore());

            CreateMap<UserInput, User>()
              .ForMember(x => x.UserId, opt => opt.Ignore())
              .ForMember(x => x.LastUpdated, opt => opt.Ignore())
              .ForMember(x => x.CreatedAt, opt => opt.Ignore());
        }
    }
}

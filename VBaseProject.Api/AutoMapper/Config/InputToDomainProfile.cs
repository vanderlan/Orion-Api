using AutoMapper;
using VBaseProject.Api.AutoMapper.Input;
using VBaseProject.Entities.Domain;

namespace VBaseProject.Api.AutoMapper.Config
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

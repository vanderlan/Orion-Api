using AutoMapper;
using VBaseProject.Entities.Domain;

namespace VBaseProject.Api.AutoMapper.Config
{
    public class DomainToOutputProfile : Profile
    {
        public DomainToOutputProfile()
        {
            CreateMap<Customer, CustomerOutput>();

            CreateMap<User, UserOutput>()
                .ForMember(x => x.ProfileDescription, opt => opt.Ignore());
        }
    }
}

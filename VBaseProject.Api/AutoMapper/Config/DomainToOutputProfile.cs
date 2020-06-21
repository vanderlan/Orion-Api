using AutoMapper;
using VBaseProject.Api.AutoMapper.Output;
using VBaseProject.Entities.Domain;
using VBaseProject.Entities.ValueObjects.Pagination;

namespace VBaseProject.Api.AutoMapper.Config
{
    public class DomainToOutputProfile : Profile
    {
        public DomainToOutputProfile()
        {
            CreateMap<Customer, CustomerOutput>();
            CreateMap<PagedList<Customer>, PagedList<CustomerOutput>>();
            CreateMap<PagedList<User>, PagedList<UserOutput>>();

            CreateMap<User, UserOutput>()
                .ForMember(x => x.ProfileDescription, opt => opt.Ignore());
        }
    }
}

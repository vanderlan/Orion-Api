using AutoMapper;
using Orion.Api.AutoMapper.Output;
using Orion.Domain.Entities;
using Orion.Domain.Entities.ValueObjects.Pagination;

namespace Orion.Api.AutoMapper.Config;

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

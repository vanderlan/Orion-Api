using AutoMapper;
using Moq;
using Orion.Api.AutoMapper.Config;

namespace Orion.Test.Controllers.BaseController
{
    public class BaseControllerTest
    {
        protected readonly IMapper Mapper;

        public BaseControllerTest()
        {
            var mockMapper = new Mock<IMapper>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new InputToDomainProfile());
                mc.AddProfile(new DomainToOutputProfile());
            });

            Mapper = mappingConfig.CreateMapper();
        }
    }
}

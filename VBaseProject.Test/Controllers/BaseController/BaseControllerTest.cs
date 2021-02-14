using AutoMapper;
using Moq;
using VBaseProject.Api.AutoMapper.Config;

namespace VBaseProject.Test.Controllers.BaseController
{
    public class BaseControllerTest
    {
        protected readonly IMapper _mapper;

        public BaseControllerTest()
        {
            var mockMapper = new Mock<IMapper>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new InputToDomainProfile());
                mc.AddProfile(new DomainToOutputProfile());
            });

            _mapper = mappingConfig.CreateMapper();
        }
    }
}

using Bogus;
using Orion.Api.AutoMapper.Input;
using Orion.Entities.Domain;

namespace Orion.Test.MotherObjects
{
    public class CustomerFaker
    {
        public static Customer Get()
        {
            return new Faker<Customer>()
                 .RuleFor(o => o.Name, f => f.Name.FullName())
                 .Generate();
        }

        public static CustomerInput GetInput()
        {
            return new Faker<CustomerInput>()
                 .RuleFor(o => o.Name, f => f.Name.FullName())
                 .Generate();
        }
    }
}

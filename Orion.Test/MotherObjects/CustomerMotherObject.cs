using Orion.Api.AutoMapper.Input;
using Orion.Entities.Domain;

namespace Orion.Test.MotherObjects
{
    public class CustomerMotherObject
    {
        public static Customer ValidCustomer()
        {
            return new Customer
            {
                Name = "Lionel Messi",
            };
        }

        public static Customer ValidCustomer2()
        {
            return new Customer
            {
                Name = "Tony Stark",
            };
        }

        public static CustomerInput ValidCustomerInput()
        {
            return new CustomerInput
            {
                Name = "Lionel Messi",
            };
        }
    }
}

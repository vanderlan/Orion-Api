using VBaseProject.Entities.Domain;

namespace VBaseProject.Test.MotherObjects
{
    public class CustomerMotherObject
    {
        public static Customer ValidCustomer()
        {
            return new Customer
            {
                Name = "Michael",
                Address = "Wall Street, 110",
                PhoneNumber = "+55989898989"
            };
        }

        public static Customer ValidCustomer2()
        {
            return new Customer
            {
                Name = "Tony Stark",
                Address = "Orchard Road, Singapura",
                PhoneNumber = "+3345656678"
            };
        }

        public static Customer InvalidCustomerWihoutName()
        {
            return new Customer
            {
                Name = null,
                Address = "Wall Street, 110",
                PhoneNumber = "+55989898989"
            };
        }
    }
}

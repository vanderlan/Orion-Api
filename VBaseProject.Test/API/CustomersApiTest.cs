using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;
using VBaseProject.Api.AutoMapper.Output;
using VBaseProject.Test.Configuration;
using VBaseProject.Test.MotherObjects;
using Xunit;

namespace VBaseProject.Test.API
{
    public class CustomersApiTest : ApiTestInitializer
    {
        [Fact]
        public async Task CreateCustomerSuccessTest()
        {
            var customerInput = CustomerMotherObject.ValidCustomerInput();

            customerInput.Name = Guid.NewGuid().ToString();

            var result = await _client.PostAsync("/api/Customers", GetStringContent(customerInput));

            var content = await result.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
            
            var customerCreated = JsonConvert.DeserializeObject<CustomerOutput>(content);

            Assert.Equal(customerInput.Name, customerCreated.Name);
            Assert.Equal(customerInput.PhoneNumber, customerCreated.PhoneNumber);
        }
        [Fact]
        public async Task CreateCustomerInvalidTest()
        {
            var customerInput = CustomerMotherObject.ValidCustomerInput();

            customerInput.Name = null;
            customerInput.PhoneNumber = null;
            customerInput.Address = null;

            var result = await _client.PostAsync("/api/Customers", GetStringContent(customerInput));

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }
    }
}

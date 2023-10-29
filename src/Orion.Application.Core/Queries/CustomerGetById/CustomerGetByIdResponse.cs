using Orion.Domain.Core.Entities;

namespace Orion.Application.Core.Queries.CustomerGetById;

public class CustomerGetByIdResponse
{
    public string PublicId { get; set; }
    public string Name { get; set; }
    public DateTime LastUpdated { get; set; }
    public DateTime CreatedAt { get; set; }

    public static explicit operator CustomerGetByIdResponse(Customer customer)
    {
        if (customer is null)
            return default;

        return new CustomerGetByIdResponse
        {
            PublicId = customer.PublicId,
            Name = customer.Name,
            LastUpdated = customer.LastUpdated,
            CreatedAt = customer.CreatedAt
        };
    }
}
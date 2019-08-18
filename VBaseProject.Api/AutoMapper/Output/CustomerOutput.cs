using System;

namespace VBaseProject.Api.AutoMapper.Output
{
    public class CustomerOutput
    {
        public string PublicId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

using System;

namespace Orion.Domain.Exceptions
{
    [Serializable]
    public class NotFoundException : BusinessException
    {
        public string Id { get; private set; }

        public NotFoundException(string id) : base($"Resource Not Found with id: {id}")
        {
            Id = id;
            Title = "Not Found";
        }
    }
}

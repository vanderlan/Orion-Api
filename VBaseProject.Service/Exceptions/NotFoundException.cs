using System;

namespace VBaseProject.Service.Exceptions
{
    [Serializable]
    public class NotFoundException : BusinessException
    {
        public string PublicId { get; private set; }

        public NotFoundException(string publicId) : base($"Resource Not Found with the identifier {publicId}")
        {
            PublicId = publicId;
            Title = "Not Found";
        }
    }
}

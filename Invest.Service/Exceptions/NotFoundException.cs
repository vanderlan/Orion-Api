using System;

namespace Invest.Service.Exceptions
{
    [Serializable]
    public class NotFoundException : BusinessException
    {
        public long Id { get; private set; }

        public NotFoundException()
        {
        }

        public NotFoundException(long id)
        {
            Id = id;
        }

        public NotFoundException(string message, long id) : base(message)
        {
            Id = id;
        }

        public NotFoundException(string message) : base(message)
        {

        }
    }
}

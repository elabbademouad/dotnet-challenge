using System;
using System.Runtime.Serialization;

namespace Cds.DroidManagement.Domain.DroidAggregate.Exceptions
{
    [Serializable]
    public sealed class DroidTooManyArmsException : BusinessException
    {
        public DroidTooManyArmsException() : base("Droid has too many arms")
        {
        }

        public DroidTooManyArmsException(string message) : base(message)
        {
        }

        public DroidTooManyArmsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        private DroidTooManyArmsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

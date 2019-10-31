using System;
using System.Runtime.Serialization;

namespace Cds.DroidManagement.Domain.DroidAggregate.Exceptions
{
    [Serializable]
    public sealed class DroidNotFoundException : BusinessException
    {
        public DroidNotFoundException() : base("Droid not found")
        {
        }

        public DroidNotFoundException(string message) : base(message)
        {
        }

        public DroidNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        private DroidNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

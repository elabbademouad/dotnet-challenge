using System;
using System.Runtime.Serialization;

namespace Cds.DroidManagement.Domain.DroidAggregate.Exceptions
{
    [Serializable]
    public sealed class DroidConflictNameException : BusinessException
    {
        public DroidConflictNameException() : base("Droid Name already exists")
        {
        }

        public DroidConflictNameException(string message) : base(message)
        {
        }

        public DroidConflictNameException(string message, Exception innerException) : base(message, innerException)
        {
        }

        private DroidConflictNameException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

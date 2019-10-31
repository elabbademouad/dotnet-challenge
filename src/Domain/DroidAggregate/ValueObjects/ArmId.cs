using System;
using Cds.DroidManagement.Domain.SeedWork;

namespace Cds.DroidManagement.Domain.DroidAggregate.ValueObjects
{
    public sealed class ArmId : PrimitiveWrapper<Guid>
    {
        private ArmId(Guid value) : base(value) { }

        public static implicit operator ArmId(Guid guid) => new ArmId(guid);
    }
}

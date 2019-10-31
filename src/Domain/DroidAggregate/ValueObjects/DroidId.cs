using System;
using Cds.DroidManagement.Domain.SeedWork;

namespace Cds.DroidManagement.Domain.DroidAggregate.ValueObjects
{
    public sealed class DroidId : PrimitiveWrapper<Guid>
    {
        private DroidId(Guid value) : base(value) { }

        public static implicit operator DroidId(Guid guid) => new DroidId(guid);
    }
}

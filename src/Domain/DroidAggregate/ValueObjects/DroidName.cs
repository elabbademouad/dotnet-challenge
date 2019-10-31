using Cds.DroidManagement.Domain.SeedWork;

namespace Cds.DroidManagement.Domain.DroidAggregate.ValueObjects
{
    public sealed class DroidName : PrimitiveWrapper<string>
    {
        public DroidName(string value) : base(value) { }

        public static implicit operator DroidName(string str) => new DroidName(str);
    }
}

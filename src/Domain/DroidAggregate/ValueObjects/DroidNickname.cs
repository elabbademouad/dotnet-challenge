using Cds.DroidManagement.Domain.SeedWork;

namespace Cds.DroidManagement.Domain.DroidAggregate.ValueObjects
{
    public sealed class DroidNickname : PrimitiveWrapper<string>
    {
        public DroidNickname(string value) : base(value) { }

        public static implicit operator DroidNickname(string str) => new DroidNickname(str);
    }
}

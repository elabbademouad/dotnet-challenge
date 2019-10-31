using Cds.DroidManagement.Domain.DroidAggregate.ValueObjects;
using System;

namespace Cds.DroidManagement.Domain.DroidAggregate.Commands
{
    public class UpdateDroid
    {
        public DroidName Name { get; }

        public DroidNickname Nickname { get; }

        public DroidId SerialNumber { get; private set; }

        public UpdateDroid WithSerialNumber(Guid serialNumber)
        {
            SerialNumber = serialNumber;
            return this;
        }

        public UpdateDroid(string name, string nickname, Guid serialNumber)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Nickname = nickname ?? throw new ArgumentNullException(nameof(nickname));
            SerialNumber = serialNumber;
        }
    }
}

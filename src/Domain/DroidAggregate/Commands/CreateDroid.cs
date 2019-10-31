using Cds.DroidManagement.Domain.DroidAggregate.ValueObjects;
using System;

namespace Cds.DroidManagement.Domain.DroidAggregate.Commands
{
    public class CreateDroid
    {
        public DroidName Name { get; }

        public DroidNickname Nickname { get; }

        public CreateDroid(string name, string nickname)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Nickname = nickname ?? throw new ArgumentNullException(nameof(nickname));
        }
    }
}

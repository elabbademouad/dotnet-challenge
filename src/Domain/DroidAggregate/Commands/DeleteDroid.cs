using Cds.DroidManagement.Domain.DroidAggregate.ValueObjects;
using System;

namespace Cds.DroidManagement.Domain.DroidAggregate.Commands
{
    public class DeleteDroid
    {
        public DroidId SerialNumber {get; }

        public DeleteDroid(Guid serialNumber)
        {
            SerialNumber = serialNumber;
        }
    }
}

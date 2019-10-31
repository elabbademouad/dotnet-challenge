using Cds.DroidManagement.Domain.DroidAggregate.ValueObjects;
using System;

namespace Cds.DroidManagement.Domain.DroidAggregate.Commands
{
    public class CreateArm
    {
        public DroidId SerialNumber { get; set; }

        public CreateArm(Guid serialNumber)
        {
            SerialNumber = serialNumber;
        }
    }
}

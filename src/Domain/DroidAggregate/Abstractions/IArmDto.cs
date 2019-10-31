using System;

namespace Cds.DroidManagement.Domain.DroidAggregate.Abstractions
{
    public interface IArmDto
    {
        Guid ArmId { get; set; }

        Guid DroidId { get; set; }
    }
}

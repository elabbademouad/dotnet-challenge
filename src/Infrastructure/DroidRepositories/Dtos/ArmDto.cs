 using Cds.DroidManagement.Domain.DroidAggregate.Abstractions;
using System;

namespace Cds.DroidManagement.Infrastructure.DroidRepositories.Dtos
{
    public class ArmDto : IArmDto
    {
        public Guid ArmId { get; set; }
        public Guid DroidId { get; set; }
    }
}

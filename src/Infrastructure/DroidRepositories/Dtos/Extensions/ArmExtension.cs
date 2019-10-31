using System;
using Cds.DroidManagement.Domain.DroidAggregate;
using Cds.DroidManagement.Domain.DroidAggregate.Abstractions;
using Cds.DroidManagement.Domain.DroidAggregate.ValueObjects;

namespace Cds.DroidManagement.Infrastructure.DroidRepositories.Dtos.Extensions
{
    public static class ArmExtension
    {
        public static IArmDto ToDto(this Arm arm, DroidId droidId)
        {
            if (arm == null)
            {
                return null;
            }

            return new ArmDto
            {
                DroidId = (Guid)droidId,
                ArmId = (Guid)arm.SerialNumber
            };
        }
    }
}

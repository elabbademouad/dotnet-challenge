using System.Collections.Generic;
using Cds.DroidManagement.Domain.DroidAggregate.ValueObjects;

namespace Cds.DroidManagement.Infrastructure.DroidRepositories.Dtos.Extensions
{
    public static class ArmIdListExtension
    {
        public static ArmIdListDto ToDto(this List<ArmId> armIds)
        {
            if (armIds == null)
            {
                return null;
            }

            return new ArmIdListDto(armIds);
        }
    }
}

using Cds.DroidManagement.Domain.DroidAggregate.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cds.DroidManagement.Domain.DroidAggregate.Abstractions
{
    public interface IArmRepository
    {
        Task InsertDroidArmAsync(DroidId serialNumber, Arm arm, Action<IDroidValidationInfo> assertDroidIsValid);

        Task<bool> DeleteAsync(List<ArmId> serialNumbers, object connection);

        Task<IReadOnlyCollection<IArmDto>> GetDroidArmsAsync(DroidId serialNumber);
    }
}

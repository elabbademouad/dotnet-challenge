using Cds.DroidManagement.Domain.DroidAggregate;
using Cds.DroidManagement.Domain.DroidAggregate.Abstractions;
using Cds.DroidManagement.Domain.DroidAggregate.ValueObjects;
using Cds.DroidManagement.Infrastructure.DroidRepositories.Abstractions;
using Cds.DroidManagement.Infrastructure.DroidRepositories.Dtos;
using Cds.DroidManagement.Infrastructure.DroidRepositories.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cds.DroidManagement.Infrastructure.DroidRepositories.Dtos.Extensions;

namespace Cds.DroidManagement.Infrastructure.DroidRepositories
{
    public class InMemoryArmRepository : IArmRepository, IReadArmRepository
    {
        private readonly List<IArmDto> _arms = new List<IArmDto>
        {
            new ArmDto
            {
                DroidId = Guid.Parse("341d9e5b-ae06-4c06-ba4f-7256317792cd"),
                ArmId = Guid.Parse("8ef70fa1-5822-433d-9cb6-e39595eec42a")
            },
            new ArmDto
            {
                DroidId = Guid.Parse("7e202cf3-d50f-4d70-bab9-c0f3a35c5bb2"),
                ArmId = Guid.Parse("e863e570-4516-4966-8920-93cd5c9a6e3e")
            },
            new ArmDto
            {
                DroidId = Guid.Parse("7e202cf3-d50f-4d70-bab9-c0f3a35c5bb2"),
                ArmId = Guid.Parse("cffc1357-6b1b-49b0-8020-c59d22de9162")
            },
            new ArmDto
            {
                DroidId = Guid.Parse("9e3cf3eb-e866-47cf-8cb6-951ac9b27d8a"),
                ArmId = Guid.Parse("1f23b73c-49a1-4bc2-a6bc-eb55cef3a682")
            }
        };

        public Task<bool> DeleteAsync(List<ArmId> serialNumbers, object connection)
        {
            int deletedLines = _arms.RemoveAll(a => serialNumbers.Contains(a.ArmId));
            bool isDeleted = deletedLines == serialNumbers.Count;
            return Task.FromResult(isDeleted);
        }

        public Task<IReadOnlyCollection<IArmDto>> GetDroidArmsAsync(DroidId serialNumber)
        {
            var arms = _arms.Where(a => a.DroidId == (Guid)serialNumber).ToList();
            return Task.FromResult<IReadOnlyCollection<IArmDto>>(arms);
        }

        public Task InsertDroidArmAsync(DroidId serialNumber, Arm arm, Action<IDroidValidationInfo> assertDroidIsValid)
        {
            // INFO: In multi-instance, semaphore is not possible, here is a check before insert it
            var nbExistingArms = _arms.Count(droid => droid.DroidId == (Guid)serialNumber);
            var nbArmsAfterInsert = nbExistingArms + 1;
            var droidValidation = new DroidValidationInfo { NbArm = nbArmsAfterInsert };

            assertDroidIsValid(droidValidation);

            _arms.Add(arm.ToDto(serialNumber));
            return Task.CompletedTask;
        }
    }
}

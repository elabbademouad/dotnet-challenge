using Cds.DroidManagement.Domain.DroidAggregate.Abstractions;
using Cds.DroidManagement.Domain.DroidAggregate.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cds.DroidManagement.Infrastructure.DroidRepositories.Abstractions
{
    public interface IReadArmRepository
    {
        Task<IReadOnlyCollection<IArmDto>> GetDroidArmsAsync(DroidId serialNumber);
    }
}

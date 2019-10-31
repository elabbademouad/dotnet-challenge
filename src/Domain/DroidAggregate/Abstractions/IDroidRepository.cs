using Cds.DroidManagement.Domain.DroidAggregate.RepositoryAction;
using Cds.DroidManagement.Domain.DroidAggregate.ValueObjects;
using System;
using System.Threading.Tasks;

namespace Cds.DroidManagement.Domain.DroidAggregate.Abstractions
{
    public interface IDroidRepository
    {
        Task InsertAsync(Droid droid);

        Task UpdateAsync(Droid droid);

        Task<bool> DeleteAsync(DroidId serialNumber, DeleteArmListAction previousActions);

        Task<bool> DoesNameAlreadyExistsAsync(DroidName name);

        Task<IDroidDto> GetBySerialNumberAsync(DroidId serialNumber, Action<IDroidUnicityValidationInfo> assertDroidExists);
    }
}

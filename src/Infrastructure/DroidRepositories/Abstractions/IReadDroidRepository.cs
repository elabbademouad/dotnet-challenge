using Cds.DroidManagement.Domain.DroidAggregate.Abstractions;
using Cds.DroidManagement.Domain.DroidAggregate.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cds.DroidManagement.Infrastructure.DroidRepositories.Abstractions
{
    public interface IReadDroidRepository
    {
        Task<(IReadOnlyCollection<IDroidDto>, bool)> GetAllPagedAsync(int skip, int take);

        Task<IDroidDto> GetBySerialNumberAsync(DroidId serialNumber, Action<IDroidUnicityValidationInfo> assertDroidExists);        
    }
}

using Cds.DroidManagement.Domain.DroidAggregate.Commands;
using System;
using System.Threading.Tasks;

namespace Cds.DroidManagement.Domain.DroidAggregate.Abstractions
{
    public interface IDroidHandler
    {
        Task<Droid> HandleAsync(CreateDroid createDroid);

        Task HandleAsync(UpdateDroid updateDroid);

        Task HandleAsync(DeleteDroid deleteDroid);

        Task HandleAsync(CreateArm createArm);
    }
}

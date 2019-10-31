using Cds.DroidManagement.Domain.DroidAggregate.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cds.DroidManagement.Domain.DroidAggregate.RepositoryAction
{
    public class DeleteArmListAction
    {
        public List<ArmId> SerialNumbers { get; private set; }

        public Func<List<ArmId>, object, Task<bool>> Action { get; private set; }

        private DeleteArmListAction(List<ArmId> serialNumber, Func<List<ArmId>, object, Task<bool>> action)
        {
            SerialNumbers = serialNumber;
            Action = action;
        }

        public static DeleteArmListAction CreateNew(List<ArmId> serialNumber, Func<List<ArmId>, object, Task<bool>> action)
        {
            return new DeleteArmListAction(serialNumber, action);
        }
    }
}

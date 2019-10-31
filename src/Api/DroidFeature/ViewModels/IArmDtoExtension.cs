using Cds.DroidManagement.Domain.DroidAggregate.Abstractions;

namespace Cds.DroidManagement.Api.DroidFeature.ViewModels
{
    internal static class IArmDtoExtension
    {
        internal static ArmViewModel ToViewModel(this IArmDto arm)
        {
            return new ArmViewModel
            {
                SerialNumber = arm.ArmId
            };
        }
    }
}

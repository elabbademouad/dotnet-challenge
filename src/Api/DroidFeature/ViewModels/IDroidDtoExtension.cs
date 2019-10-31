using Cds.DroidManagement.Domain.DroidAggregate.Abstractions;

namespace Cds.DroidManagement.Api.DroidFeature.ViewModels
{
    internal static class IDroidDtoExtension
    {
        internal static DroidViewModel ToViewModel(this IDroidDto droid)
        {            
            return new DroidViewModel
            {
                SerialNumber = droid.DroidId,
                CreatedOn = droid.CreationDate,
                Name = droid.Name,
                Nickname = droid.Nickname,
                Quote = droid.Quote                
            };
        }
    }
}

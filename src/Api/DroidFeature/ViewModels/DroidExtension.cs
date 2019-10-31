using Cds.DroidManagement.Domain.DroidAggregate;
using System;

namespace Cds.DroidManagement.Api.DroidFeature.ViewModels
{
    internal static class DroidExtension
    {
        internal static DroidViewModel ToViewModel(this Droid droid)
        {
            return new DroidViewModel
            {
                SerialNumber = (Guid)droid.SerialNumber,
                CreatedOn = droid.CreatedOn,
                Name = (string)droid.Name,
                Nickname = (string)droid.Nickname,
                Quote = droid.Quote
            };
        }
    }
}

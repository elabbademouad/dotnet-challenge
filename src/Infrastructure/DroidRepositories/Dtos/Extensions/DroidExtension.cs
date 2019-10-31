using Cds.DroidManagement.Domain.DroidAggregate;
using Cds.DroidManagement.Domain.DroidAggregate.Abstractions;
using System;

// INFO: Extension class is the same namespace as the typed extended
namespace Cds.DroidManagement.Infrastructure.DroidRepositories.Dtos.Extensions
{
    // INFO: Extension method on domain entity is in infrastructure to know DroidDto implementation
    public static class DroidExtension
    {
        public static IDroidDto ToDto(this Droid droid)
        {
            if (droid == null)
            {
                return null;
            }

            return new DroidDto
            {
                DroidId = (Guid)droid.SerialNumber,
                CreationDate = droid.CreatedOn,
                Name = (string)droid.Name,
                Nickname = (string)droid.Nickname,
                Quote = droid.Quote
            };
        }
    }
}

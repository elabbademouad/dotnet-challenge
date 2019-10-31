using Cds.DroidManagement.Domain.DroidAggregate.Abstractions;

namespace Cds.DroidManagement.Infrastructure.DroidRepositories.Validation
{
    public class DroidUnicityValidationInfo : IDroidUnicityValidationInfo
    {
        public IDroidDto Droid { get; set; }
    }
}

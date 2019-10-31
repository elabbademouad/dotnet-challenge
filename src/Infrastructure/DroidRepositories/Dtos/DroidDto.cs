using Cds.DroidManagement.Domain.DroidAggregate.Abstractions;
using System;

namespace Cds.DroidManagement.Infrastructure.DroidRepositories.Dtos
{
    public class DroidDto : IDroidDto
    {
        public Guid DroidId { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public string Name { get; set; }
        public string Nickname { get; set; }
        public string Quote { get; set; }
    }
}

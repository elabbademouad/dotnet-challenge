using System;

namespace Cds.DroidManagement.Domain.DroidAggregate.Abstractions
{
    public interface IDroidDto
    {
        Guid DroidId { get; set; }
        DateTimeOffset CreationDate { get; set; }
        string Name { get; set; }
        string Nickname { get; set; }
        string Quote { get; set; }
    }
}

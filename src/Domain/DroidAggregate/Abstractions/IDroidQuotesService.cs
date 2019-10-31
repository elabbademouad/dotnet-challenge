using System.Threading.Tasks;

namespace Cds.DroidManagement.Domain.DroidAggregate.Abstractions
{
    public interface IDroidQuotesService
    {
        Task<string> GetRandomQuoteAsync();
    }
}

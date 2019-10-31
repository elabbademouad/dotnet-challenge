using System.Net.Http;
using System.Threading.Tasks;
using Cds.DroidManagement.Domain.DroidAggregate.Abstractions;
using Newtonsoft.Json;

namespace Cds.DroidManagement.Infrastructure.DroidQuotes
{
    public class ExternalDroidQuotesService : IDroidQuotesService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ExternalDroidQuotesService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetRandomQuoteAsync()
        {
            var client = _httpClientFactory.CreateClient("StarWars");
            var response = await client.GetStringAsync(client.BaseAddress);
            var jsonObject = JsonConvert.DeserializeObject<DroidQuote>(response);

            return jsonObject.StarWarsQuote;
        }
    }
}

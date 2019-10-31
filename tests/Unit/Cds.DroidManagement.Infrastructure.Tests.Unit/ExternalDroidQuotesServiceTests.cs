using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cds.DroidManagement.Infrastructure.DroidQuotes;
using FluentAssertions;
using Moq;
using Xunit;

namespace Cds.DroidManagement.Infrastructure.Tests.Unit
{
    public class ExternalDroidQuotesServiceTests
    {
        public class FakeHttpMessageHandler : DelegatingHandler
        {
            private readonly HttpResponseMessage _fakeResponse;

            public FakeHttpMessageHandler(HttpResponseMessage responseMessage)
            {
                _fakeResponse = responseMessage;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(_fakeResponse);
            }
        }

        [Fact]
        public async Task GetRandomQuoteAsync_WithValidApi_ReturnsQuote()
        {
            // Arrange
            var quote = "Just for once, let me look on you with my own eyes. — Anakin Skywalker";
            var response = "{\r\n\"id\": 39,\r\n\"starWarsQuote\": \"" + quote + "\",\r\n\"faction\": 0\r\n}";
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(response, Encoding.UTF8, "application/json")
            };
            var fakeHttpClient = new HttpClient(new FakeHttpMessageHandler(responseMessage))
            {
                BaseAddress = new Uri("http://localhost/")
            };

            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory
                .Setup(f => f.CreateClient("StarWars"))
                .Returns(fakeHttpClient);

            var droidQuoteService = new ExternalDroidQuotesService(httpClientFactory.Object);

            // Act
            var retrievedQuote = await droidQuoteService.GetRandomQuoteAsync();

            // Assert
            retrievedQuote.Should().Be(quote);
        }
    }
}

using Cds.DroidManagement.Api.Bootstrap;
using Cds.DroidManagement.Api.DroidFeature.ViewModels;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Cds.Droid.Management.Api.Tests.Integration
{
    public class DroidsControllerTests
    {
        [Fact]
        public async Task GetAllAsync()
        {
            // Arrange
            var server = new TestServer(
                Program
                    .CreateWebHostBuilder(new string[0])
                    .UseConfiguration(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build())
                    .UseEnvironment("Sandbox"));
            var client = server.CreateClient();
            var getUrl = new Uri(client.BaseAddress, "api/Droids");

            // Act
            var response = await client.GetAsync(getUrl);
            var json = await response.Content.ReadAsStringAsync();
            var pagedListDroid = JsonConvert.DeserializeObject<PagedList<DroidViewModel>>(json);

            // Assert
            pagedListDroid.Items.Should().HaveCountGreaterThan(0);
        }
    }
}
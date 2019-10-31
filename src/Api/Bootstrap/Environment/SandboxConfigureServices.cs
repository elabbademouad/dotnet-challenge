using Cds.DroidManagement.Domain.DroidAggregate.Abstractions;
using Cds.DroidManagement.Infrastructure.DroidRepositories;
using Cds.DroidManagement.Infrastructure.DroidRepositories.Abstractions;
using Cds.DroidManagement.Infrastructure.Encryption;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;

namespace Cds.DroidManagement.Api.Bootstrap.Environment
{
    internal class SandboxConfigureServices : AConfigureServices
    {
        public SandboxConfigureServices(
            IServiceCollection services) : base(services)
        {
        }

        internal override void AddRepositories()
        {
            var inMemoryArmRepository = new InMemoryArmRepository();
            var inMemoryDroidRepository = new InMemoryDroidRepository();
            
            _services
                .AddSingleton<IDroidRepository>(inMemoryDroidRepository)
                .AddSingleton<IReadDroidRepository>(inMemoryDroidRepository)
                .AddSingleton<IArmRepository>(inMemoryArmRepository)
                .AddSingleton<IReadArmRepository>(inMemoryArmRepository)
                .AddSingleton<IEncryptionService, EncryptionService>();
        }

        internal override void RegisterChecks(HealthCheckBuilder checks)
        {
            checks.AddCheck("Default", () => HealthCheckResult.Healthy("OK"));
        }
    }
}

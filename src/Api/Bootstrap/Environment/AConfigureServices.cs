using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;

namespace Cds.DroidManagement.Api.Bootstrap.Environment
{
    internal abstract class AConfigureServices
    {
        protected IServiceCollection _services;

        protected AConfigureServices(IServiceCollection services)
        {
            _services = services;
        }

        internal abstract void AddRepositories();

        internal abstract void RegisterChecks(HealthCheckBuilder checks);
    }
}

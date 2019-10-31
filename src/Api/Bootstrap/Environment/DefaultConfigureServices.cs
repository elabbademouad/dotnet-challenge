using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using Cds.DroidManagement.Domain.DroidAggregate.Abstractions;
using Cds.DroidManagement.Infrastructure.DroidRepositories;
using Cds.DroidManagement.Infrastructure.DroidRepositories.Abstractions;
using Cds.DroidManagement.Infrastructure.Encryption;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;

namespace Cds.DroidManagement.Api.Bootstrap.Environment
{
    [ExcludeFromCodeCoverage]
    internal class DefaultConfigureServices : AConfigureServices
    {
        protected string _connectionString;

        public DefaultConfigureServices(
            IServiceCollection services,
            IConfiguration configuration,
            IHostingEnvironment env) : base(services)
        {
            var templatedConnectionString = configuration.GetConnectionString(connectionStringName);
            var mdfDirectory = Directory.GetParent(env.ContentRootPath);
            _connectionString = string.Format(CultureInfo.InvariantCulture, templatedConnectionString, mdfDirectory);
        }

        internal override void AddRepositories()
        {
            IDbConnection ConnectionProvider() => new SqlConnection(_connectionString);
            var sqlServerDroidRepository = new SqlServerDroidRepository(ConnectionProvider);
            var sqlServerArmRepository = new SqlServerArmRepository(ConnectionProvider);

            _services
                .AddSingleton<IDroidRepository>(sqlServerDroidRepository)
                .AddSingleton<IReadDroidRepository>(sqlServerDroidRepository)
                .AddSingleton<IArmRepository>(sqlServerArmRepository)
                .AddSingleton<IReadArmRepository>(sqlServerArmRepository)
                .AddSingleton<IEncryptionService, EncryptionService>();
        }

        internal override void RegisterChecks(HealthCheckBuilder checks)
        {
            checks
                .AddCheck("Default", () => HealthCheckResult.Healthy("OK"))
                .AddSqlCheck(connectionStringName, _connectionString);
        }

        private const string connectionStringName = "DroidRepository";
    }
}

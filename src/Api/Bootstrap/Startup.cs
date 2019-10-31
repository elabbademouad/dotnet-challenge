using System;
using Cds.DroidManagement.Api.Bootstrap.Environment;
using Cds.DroidManagement.Domain.DroidAggregate;
using Cds.DroidManagement.Domain.DroidAggregate.Abstractions;
using Cds.DroidManagement.Infrastructure.DroidQuotes;
using Cds.DroidManagement.Infrastructure.Encryption;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Polly;

namespace Cds.DroidManagement.Api.Bootstrap
{
    /// <summary>
    /// Represents the application's bootstrap.
    /// </summary>
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _environment;

        private readonly TimeSpan[] _httpRetries =
        {
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(10)
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="configuration">The configuration.</param>
        public Startup(IHostingEnvironment environment, IConfiguration configuration)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        
        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            AConfigureServices serviceConfiguration;
            if (_environment.IsEnvironment("Sandbox"))
            {
                serviceConfiguration = new SandboxConfigureServices(services);
            }
            else
            {
                serviceConfiguration = new DefaultConfigureServices(services, _configuration, _environment);
            }

            services.Configure<EncryptionConfiguration>(_configuration.GetSection("EncryptionSettings"));
            services.ConfigureOptions<EncryptionConfiguration>();
            serviceConfiguration.AddRepositories();

            services
                .AddHttpClient("StarWars", client =>
                {
                    client.BaseAddress = new Uri(_configuration.GetValue<string>("StarWarsQuoteUrl"));
                })
                .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(_httpRetries));

            services
                .AddScoped<IDroidHandler, DroidHandler>()
                .AddSingleton<IDroidQuotesService, ExternalDroidQuotesService>()
                .AddHealthChecks(checks => serviceConfiguration.RegisterChecks(checks))
                .AddCustomSwaggerGen()
                .AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore)
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The application.</param>
        public void Configure(IApplicationBuilder app)
        {
            if (_environment.IsDevelopment()) app.AddStackTraceErrorDisplay();

            app
                .UseCustomSwagger()
                .UseMvc();
        }
    }
}

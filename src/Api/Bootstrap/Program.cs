using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;

namespace Cds.DroidManagement.Api.Bootstrap
{
    /*  
      TODO:
        - Fix PrimitiveWrapper to match string/primitives (use specific primitive type wrapper)        
        - Add Pact tests
        - IArmRepository: replace DroidId by a list of ArmId

        - Add Event sourcing example (separate state mutation into event and store it in ges)
        - Add Swashbuckle.Examples nuget to manage consistent examples
        - Integration Tests of repository with Database
        - Add HATEOAS (restful level 3)
        - Improve healthchecks: in 2.2 you'll be able to add healthchecks in different places (no more required to define connectionString in test)
    */

    /// <summary>
    /// Entry point class used at runtime.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The application arguments.</param>
        public static void Main(string[] args) =>
            CreateWebHostBuilder(args)
                .Build()
                .Run();

        /// <summary>
        /// Creates the web build host.
        /// </summary>
        /// <param name="args">The application arguments.</param>
        /// <returns></returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseHealthChecks("/health", TimeSpan.FromSeconds(3))
                .UseStartup<Startup>();
    }
}

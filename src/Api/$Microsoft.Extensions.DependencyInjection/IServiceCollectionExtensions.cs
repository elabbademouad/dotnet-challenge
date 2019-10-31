using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Microsoft.Extensions.DependencyInjection
{
    internal static class IServiceCollectionExtensions
    {
        internal static IServiceCollection AddCustomSwaggerGen(this IServiceCollection services)
        {
            services
                .AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new Info
                    {
                        Title = "Droids",
                        Description = "The droids factory",
                        Version = "v1"
                    });

                    // Set the comments path for the Swagger JSON and UI.
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    options.IncludeXmlComments(xmlPath);

                    options.CustomSchemaIds(UseDataContractAnnotation);
                });

            return services;
        }

        private const string DataContractParameterName = "Name";
        private static string UseDataContractAnnotation(Type type)
        {
            var attributeName = type.CustomAttributes
                                    .FirstOrDefault(a => a.AttributeType == typeof(DataContractAttribute))
                                    ?.NamedArguments
                                    ?.FirstOrDefault(na => na.MemberName == DataContractParameterName)
                                    .TypedValue;

            if (attributeName?.ArgumentType == typeof(string))
            {
                return (string)attributeName.Value.Value;
            }

            return type.Name;
        }
    }
}

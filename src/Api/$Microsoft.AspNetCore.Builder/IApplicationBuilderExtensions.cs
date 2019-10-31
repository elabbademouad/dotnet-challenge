using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Reflection;

namespace Microsoft.AspNetCore.Builder
{
    internal static class IApplicationBuilderExtensions
    {
        internal static IApplicationBuilder AddStackTraceErrorDisplay(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(options =>
                options.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "text/html";
                    var ex = context.Features.Get<IExceptionHandlerFeature>();
                    if (ex != null)
                    {
                        var err = $"Error: {ex.Error.Message} - {ex.Error.StackTrace}";
                        await context.Response.WriteAsync(err);
                    }
                })
            );

            return app;
        }

        internal static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger()
            .UseSwaggerUI((options) =>
            {
                var assemblyProduct = Assembly.GetExecutingAssembly().GetName().Name;
                options.SwaggerEndpoint("/swagger/v1/swagger.json", $"{assemblyProduct} v1.0");
            });

            return app;
        }
    }
}

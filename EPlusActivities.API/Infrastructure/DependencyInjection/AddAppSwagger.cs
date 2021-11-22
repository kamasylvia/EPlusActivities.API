using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace EPlusActivities.API.Infrastructure.DependencyInjection
{
    public static partial class ServiceCollectionDependencyInjection
    {
        public static void AddAppSwagger(this IServiceCollection services) => services.AddSwaggerGen(
                 c =>
                 {
                     c.SwaggerDoc(
                         "v1",
                         new OpenApiInfo { Title = "EPlusActivities.API", Version = "v1" }
                     );
                     c.IncludeXmlComments(
                         Path.Combine(
                             Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                             "EPlusActivities.API.xml"
                         )
                     );
                 }
             );

    }
}

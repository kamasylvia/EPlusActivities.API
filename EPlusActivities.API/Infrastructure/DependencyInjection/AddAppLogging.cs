using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace EPlusActivities.API.Infrastructure.DependencyInjection
{
    public static partial class ServiceCollectionDependencyInjection
    {
        public static void AddAppLogging(this IServiceCollection services) =>
            services.AddLogging(
                (builder) =>
                {
                    builder.AddSerilog(dispose: true);
                }
            );
    }
}

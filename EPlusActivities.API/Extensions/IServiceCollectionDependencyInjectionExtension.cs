using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EPlusActivities.API.Infrastructure.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace EPlusActivities.API.Extensions
{
    public static class IServiceCollectionDependencyInjectionExtension
    {
        public static void AddCustomDependencies(
            this IServiceCollection services
        )
        {
           services.RegisterLifetimesByAttribute(ServiceLifetime.Transient); 
           services.RegisterLifetimesByAttribute(ServiceLifetime.Scoped); 
           services.RegisterLifetimesByAttribute(ServiceLifetime.Singleton); 
        }

        private static void RegisterLifetimesByAttribute(
            this IServiceCollection services,
            ServiceLifetime serviceLifetime
        )
        {
            // var types =
            Assembly
                .GetEntryAssembly()
                .GetReferencedAssemblies()
                .Select(Assembly.Load)
                .SelectMany(asm => asm.DefinedTypes)
                .Where(implementer =>
                    implementer
                        .GetCustomAttributes(typeof(DependencyAttribute), true)
                        .Length >
                    0 &&
                    implementer.GetCustomAttribute<DependencyAttribute>().Lifetime ==
                    serviceLifetime &&
                    implementer.IsClass &&
                    !implementer.IsAbstract)
                    .ToList().ForEach(implementerItem =>
                        implementerItem.GetInterfaces().ToList().ForEach(
                            interfaceItem =>
                            {
                                switch (serviceLifetime)
                                {
                                    case ServiceLifetime.Transient:
                                        services.AddTransient(interfaceItem, implementerItem);
                                        break;
                                    case ServiceLifetime.Scoped:
                                        services.AddScoped(interfaceItem, implementerItem);
                                        break;
                                    case ServiceLifetime.Singleton:
                                        services.AddSingleton(interfaceItem, implementerItem);
                                        break;
                                }
                            }
                        )
                    );
        }
    }
}

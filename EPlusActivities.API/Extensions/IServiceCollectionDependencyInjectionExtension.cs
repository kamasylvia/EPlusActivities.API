using System;
using System.Linq;
using System.Reflection;
using EPlusActivities.API.Infrastructure.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace EPlusActivities.API.Extensions
{
    public static class IServiceCollectionDependencyInjectionExtension
    {
        public static void AddCustomDependencies(this IServiceCollection services)
        {
            services.RegisterLifetimesByAttribute(ServiceLifetime.Transient);
            services.RegisterLifetimesByAttribute(ServiceLifetime.Scoped);
            services.RegisterLifetimesByAttribute(ServiceLifetime.Singleton);
        }

        /*
        利用反射取所有类型时，用的是
            AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
        如果是多项目分层的话要替换成
            Assembly.GetEntryAssembly().GetReferencedAssemblies()
                .Select(Assembly.Load).SelectMany(x => x.DefinedTypes)
        */
        private static void RegisterLifetimesByAttribute(
            this IServiceCollection services,
            ServiceLifetime serviceLifetime
        ) =>
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(
                    implementer =>
                        implementer.GetCustomAttributes(
                            typeof(CustomDependencyAttribute),
                            false
                        ).Length > 0
                        && implementer.GetCustomAttribute<CustomDependencyAttribute>().Lifetime
                            == serviceLifetime
                        && implementer.IsClass
                        && !implementer.IsAbstract
                )
                .ToList()
                .ForEach(
                    implementerItem =>
                        implementerItem.GetInterfaces()
                            .ToList()
                            .ForEach(
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

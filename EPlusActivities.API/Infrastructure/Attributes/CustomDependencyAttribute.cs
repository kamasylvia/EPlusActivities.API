using System;
using Microsoft.Extensions.DependencyInjection;

namespace EPlusActivities.API.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomDependencyAttribute : Attribute
    {
        public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;
        public CustomDependencyAttribute(ServiceLifetime serviceLifetime)
        {
            Lifetime = serviceLifetime;
        }
    }
}

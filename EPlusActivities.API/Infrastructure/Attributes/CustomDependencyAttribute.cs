using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

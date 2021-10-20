using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace EPlusActivities.API.Infrastructure.Attributes
{
    public class DependencyAttribute : Attribute
    {
        public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;
        public DependencyAttribute(ServiceLifetime serviceLifetime)
        {
           Lifetime = serviceLifetime; 
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace EPlusActivities.API.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomServiceAttribute : CustomDependencyAttribute
    {
        public CustomServiceAttribute(ServiceLifetime serviceLifetime) : base(serviceLifetime)
        {
        }
    }
}

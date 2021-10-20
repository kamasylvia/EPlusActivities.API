using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace EPlusActivities.API.Infrastructure.Attributes
{
    public class RepositoryAttribute : DependencyAttribute
    {
        public RepositoryAttribute(ServiceLifetime serviceLifetime) : base(serviceLifetime)
        {
        }
    }
}

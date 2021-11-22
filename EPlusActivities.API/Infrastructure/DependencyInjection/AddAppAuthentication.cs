using Microsoft.Extensions.DependencyInjection;

namespace EPlusActivities.API.Infrastructure.DependencyInjection
{
    public static partial class ServiceCollectionDependencyInjection 
    {
        public static void AddAppAuthentication(this IServiceCollection services) =>
            //基于策略授权
            services.AddAuthorization(
                options =>
                {
                    options.AddPolicy(
                        "CustomerPolicy",
                        builder =>
                        {
                            builder.RequireRole("customer, tester");
                        }
                    );
                    options.AddPolicy(
                        "ManagerPolicy",
                        builder =>
                        {
                            builder.RequireRole("manager", "admin", "tester");
                        }
                    );
                    options.AddPolicy(
                        "AdminPolicy",
                        builder =>
                        {
                            builder.RequireRole("admin", "tester");
                        }
                    );
                    options.AddPolicy(
                        "TesterPolicy",
                        builder =>
                        {
                            builder.RequireRole("tester");
                        }
                    );
                    options.AddPolicy(
                        "AllRoles",
                        builder =>
                        {
                            builder.RequireRole("admin", "manager", "customer", "tester");
                        }
                    );
                }
            );
        
    }
}

using System.Text.Json;
using EPlusActivities.API.Application.Actors.ActivityActors;
using EPlusActivities.API.Application.Actors.ActivityUserActors;
using EPlusActivities.API.Application.Actors.AddressActors;
using EPlusActivities.API.Application.Actors.AttendanceActors;
using Microsoft.Extensions.DependencyInjection;

namespace EPlusActivities.API.Infrastructure.DependencyInjection
{
    public static partial class ServiceCollectionDependencyInjection
    {
        public static void AddAppActors(this IServiceCollection services) =>
            services.AddActors(
                options =>
                {
                    var jsonSerializerOptions = new JsonSerializerOptions()
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true
                    };

                    options.JsonSerializerOptions = jsonSerializerOptions;
                    options.Actors.RegisterActor<ActivityUserActor>();
                    options.Actors.RegisterActor<ActivityActor>();
                    options.Actors.RegisterActor<AddressActor>();
                    options.Actors.RegisterActor<AttendanceActor>();
                }
            );
    }
}

using System.Text.Json;
using EPlusActivities.API.Application.Actors.ActivityActors;
using EPlusActivities.API.Application.Actors.ActivityUserActors;
using EPlusActivities.API.Application.Actors.AddressActors;
using EPlusActivities.API.Application.Actors.AttendanceActors;
using EPlusActivities.API.Application.Actors.BrandActors;
using EPlusActivities.API.Application.Actors.CategoryActors;
using EPlusActivities.API.Application.Actors.FileActors;
using EPlusActivities.API.Application.Actors.DrawingActors;
using EPlusActivities.API.Application.Actors.PrizeItemActors;
using EPlusActivities.API.Application.Actors.PrizeTierActors;
using EPlusActivities.API.Application.Actors.UserActors;
using Microsoft.Extensions.DependencyInjection;
using EPlusActivities.API.Application.Actors.LotteryStatementActors;

namespace EPlusActivities.API.Infrastructure.DependencyInjection
{
    public static partial class ServiceCollectionDependencyInjection
    {
        public static void AddAppActors(this IServiceCollection services) =>
            services.AddActors(
                options =>
                {
                    options.JsonSerializerOptions = new JsonSerializerOptions()
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true
                    };
                    options.Actors.RegisterActor<ActivityActor>();
                    options.Actors.RegisterActor<ActivityUserActor>();
                    options.Actors.RegisterActor<AddressActor>();
                    options.Actors.RegisterActor<AttendanceActor>();
                    options.Actors.RegisterActor<BrandActor>();
                    options.Actors.RegisterActor<CategoryActor>();
                    options.Actors.RegisterActor<FileActor>();
                    options.Actors.RegisterActor<DrawingActor>();
                    options.Actors.RegisterActor<LotteryStatementActor>();
                    options.Actors.RegisterActor<PrizeItemActor>();
                    options.Actors.RegisterActor<PrizeTierActor>();
                    options.Actors.RegisterActor<UserActor>();
                }
            );
    }
}

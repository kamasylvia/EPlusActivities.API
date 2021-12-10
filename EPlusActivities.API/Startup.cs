using System;
using System.Reflection;
using System.Text.Json.Serialization;
using Dapr.Actors;
using Dapr.Actors.Client;
using EPlusActivities.API.Application.Actors.LotteryStatementActors;
using EPlusActivities.API.Infrastructure.DependencyInjection;
using EPlusActivities.API.Infrastructure.Filters;
using EPlusActivities.API.Services.IdGeneratorService;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Yitter.IdGenerator;

namespace EPlusActivities.API
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }

        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers(
                    options =>
                    {
                        options.Filters.Add<CustomExceptionFilterAttribute>();
                        options.Filters.Add<CustomActionFilterAttribute>();
                    }
                )
                .AddJsonOptions(
                    options =>
                    {
                        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                        // options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                    }
                )
                .AddDapr();

            if (Environment.IsDevelopment())
            {
                services.AddCors(
                    options =>
                    {
                        options.AddDefaultPolicy(
                            builder =>
                            {
                                builder
                                    .AllowAnyHeader()
                                    .AllowAnyMethod()
                                    .WithOrigins(Configuration["ClientUrl"]);
                            }
                        );
                    }
                );
            }

            services.AddHttpContextAccessor();

            // Actors
            services.AddAppActors();

            // Swagger
            services.AddAppSwagger();

            // Serilog
            services.AddAppLogging();

            // 数据库配置系统应用用户数据上下文
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            // var serverVersion = ServerVersion.AutoDetect(connectionString);
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 25));
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            // 数据库和 IdentityServer4
            services.AddDbAndIS4(connectionString, serverVersion, migrationsAssembly, Environment.IsDevelopment());

            // 权限验证策略
            services.AddAppAuthentication();

            // 启用自定义依赖，包括仓储和服务
            services.AddCustomDependencies();

            // 启用创建短 ID 服务
            services.AddSingleton<IIdGeneratorService>(
                new IdGeneratorService(
                    new IdGeneratorOptions(1) { WorkerIdBitLength = 1, SeqBitLength = 3 }
                )
            );

            // AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // MediatR
            services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(
                    c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EPlusActivities.API")
                );
            }

            app.UseHttpLogging();
            // app.UseHttpsRedirection();

            app.UseRouting();

            if (env.IsDevelopment())
            {
                app.UseCors();
            }

            app.UseIdentityServer();

            app.UseAuthentication();

            app.UseAuthorization();

            // app.UseCloudEvents();

            ActorProxy
                .Create<ILotteryStatementActor>(ActorId.CreateRandom(), "StatementGenerator")
                .SetReminderAsync();

            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapActorsHandlers();
                    // endpoints.MapSubscribeHandler();
                    endpoints.MapControllers();
                }
            );
        }
    }
}

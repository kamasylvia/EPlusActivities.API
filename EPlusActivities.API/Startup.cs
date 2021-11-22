using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using EPlusActivities.API.Application.Actors.ActivityActors;
using EPlusActivities.API.Application.Actors.ActivityUserActors;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Extensions;
using EPlusActivities.API.Infrastructure.Filters;
using EPlusActivities.API.Services.IdentityServer;
using EPlusActivities.API.Services.IdGeneratorService;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Serilog;
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
                    }
                )
                .AddDapr();

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
                }
            );

            services.AddLogging(
                (builder) =>
                {
                    builder.AddSerilog(dispose: true);
                }
            );

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

            services.AddSwaggerGen(
                c =>
                {
                    c.SwaggerDoc(
                        "v1",
                        new OpenApiInfo { Title = "EPlusActivities.API", Version = "v1" }
                    );
                    c.IncludeXmlComments(
                        Path.Combine(
                            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                            "EPlusActivities.API.xml"
                        )
                    );
                }
            );

            // 数据库配置系统应用用户数据上下文
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            // var serverVersion = ServerVersion.AutoDetect(connectionString);
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<ApplicationDbContext>(
                options =>
                    options.UseMySql(
                        connectionString,
                        o => o.ServerVersion(new Version(8, 0, 25), ServerType.MySql)
                    )
            );

            // 启用 Identity 服务 添加指定的用户和角色类型的默认标识系统配置
            services
                .AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // 启用自定义依赖，包括仓储和服务
            services.AddCustomDependencies();

            // 启用创建短 ID 服务
            services.AddSingleton<IIdGeneratorService>(
                new IdGeneratorService(
                    new IdGeneratorOptions(1) { WorkerIdBitLength = 1, SeqBitLength = 3 }
                )
            );

            // IdentityServer 4
            var builder = services
                .AddIdentityServer(
                    options =>
                    {
                        options.Events.RaiseErrorEvents = true;
                        options.Events.RaiseInformationEvents = true;
                        options.Events.RaiseFailureEvents = true;
                        options.Events.RaiseSuccessEvents = true;

                        // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                        options.EmitStaticAudienceClaim = true;
                    }
                ) // .AddTestUsers(TestUsers.Users)
                .AddAspNetIdentity<ApplicationUser>() // SMS Validator
                .AddExtensionGrantValidator<SmsGrantValidator>();

            // InMemory Mode
            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();
            if (Environment.IsDevelopment())
            {
                builder
                    .AddInMemoryIdentityResources(Config.IdentityResources)
                    .AddInMemoryApiScopes(Config.ApiScopes)
                    .AddInMemoryApiResources(Config.ApiResources)
                    .AddInMemoryClients(Config.Clients);
            }
            else
            {
                // Db Mode
                // this adds the config data from DB (clients, resources, CORS)
                builder
                    .AddConfigurationStore(
                        options =>
                        {
                            options.ConfigureDbContext = builder =>
                                builder.UseMySql(
                                    connectionString,
                                    sql => sql.MigrationsAssembly(migrationsAssembly)
                                );
                        }
                    ) // this adds the operational data from DB (codes, tokens, consents)
                    .AddOperationalStore(
                        options =>
                        {
                            options.ConfigureDbContext = builder =>
                                builder.UseMySql(
                                    connectionString,
                                    sql => sql.MigrationsAssembly(migrationsAssembly)
                                );

                            // this enables automatic token cleanup. this is optional.
                            options.EnableTokenCleanup = true;
                        }
                    );
            }

            // 受保护的 API 设置
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                    options =>
                    {
                        //IdentityServer地址
                        options.Authority = Environment.IsDevelopment()
                            ? "http://localhost:52537"
                            : "http://localhost:80";

                        //对应Idp中ApiResource的Name
                        options.Audience = "eplus.api";

                        //不使用https
                        options.RequireHttpsMetadata = false;

                        options.TokenValidationParameters.ValidateIssuer = false;
                    }
                );

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

            // app.UseHttpsRedirection();

            app.UseRouting();

            if (env.IsDevelopment())
            {
                app.UseCors();
            }

            app.UseIdentityServer();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseCloudEvents();

            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapActorsHandlers();
                    endpoints.MapSubscribeHandler();
                    endpoints.MapControllers();
                }
            );
        }
    }
}

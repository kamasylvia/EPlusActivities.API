using System;
using System.Reflection;
using System.Text.Json.Serialization;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Services.ActivityService;
using EPlusActivities.API.Services.DeliveryService;
using EPlusActivities.API.Services.IdentityServer;
using EPlusActivities.API.Services.IdGeneratorService;
using EPlusActivities.API.Services.MemberService;
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
            services.AddControllers()
                .AddJsonOptions(
                    x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve
                );

            services.AddCors(
                options =>
                {
                    options.AddDefaultPolicy(
                        builder =>
                        {
                            builder.AllowAnyHeader()
                                .AllowAnyMethod()
                                .WithOrigins("http://localhost:8080");
                        }
                    );
                }
            );

            services.AddHttpClient();
            services.AddHttpContextAccessor();

            services.AddSwaggerGen(
                c =>
                {
                    c.SwaggerDoc(
                        "v1",
                        new OpenApiInfo { Title = "EPlusActivities.API", Version = "v1" }
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
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // 启用数据库仓库
            services.AddTransient<IActivityRepository, ActivityRepository>()
                .AddTransient<IAttendanceRepository, AttendanceRepository>()
                .AddTransient<IRepository<Credit>, CreditRepository>()
                .AddTransient<IRepository<Coupon>, CouponRepository>()
                .AddTransient<IFindByParentIdRepository<ActivityUser>, ActivityUserRepository>()
                .AddTransient<IFindByParentIdRepository<Address>, AddressRepository>()
                .AddTransient<IFindByParentIdRepository<Lottery>, LotteryRepository>()
                .AddTransient<IPrizeItemRepository, PrizeItemRepository>()
                .AddTransient<INameExistsRepository<Brand>, BrandRepository>()
                .AddTransient<INameExistsRepository<Category>, CategoryRepository>()
                .AddTransient<IFindByParentIdRepository<PrizeTier>, PrizeTierRepository>();

            // 启用创建短 ID 服务
            services.AddSingleton<IIdGeneratorService>(
                new IdGeneratorService(
                    new IdGeneratorOptions(1) { WorkerIdBitLength = 1, SeqBitLength = 3 }
                )
            );

            // 启用短信服务
            services.AddTransient<ISmsService, SmsService>();

            // 启用会员服务
            services.AddScoped<IMemberService, MemberService>();

            // 启用发送奖品服务
            services.AddScoped<ILotteryDrawService, LotteryDrawService>();

            // 启用活动服务
            services.AddTransient<IActivityService, ActivityService>();

            // IdentityServer 4
            var builder = services.AddIdentityServer(
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
            builder.AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryApiResources(Config.ApiResources)
                .AddInMemoryClients(Config.Clients);

            // Db Mode
            // this adds the config data from DB (clients, resources, CORS)
            /*
            builder
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder
                            .UseMySql(connectionString,
                            sql =>
                                sql.MigrationsAssembly(migrationsAssembly));
                }) // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder
                            .UseMySql(connectionString,
                            sql =>
                                sql.MigrationsAssembly(migrationsAssembly));

                        // this enables automatic token cleanup. this is optional.
                        options.EnableTokenCleanup = true;
                });
            */

            // 受保护的 API 设置
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                    options =>
                    {
                        //IdentityServer地址
                        options.Authority = "http://localhost:52537";

                        //对应Idp中ApiResource的Name
                        // options.Audience = "eplus.api.test";
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
                        "Customer",
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
                        "TestPolicy",
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            #region Seed data
            DbInitializer.Initialize(app: app, environment: env);
            #endregion


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(
                    c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EPlusActivities.API")
                );
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseIdentityServer();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllers();
                }
            );
        }
    }
}

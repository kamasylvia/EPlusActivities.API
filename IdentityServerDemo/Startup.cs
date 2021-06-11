// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityServerDemo
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

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            // var serverVersion = ServerVersion.AutoDetect(connectionString);
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;


            // services.AddControllers();
            // services.AddSwaggerGen(c =>
            // {
            //     c.SwaggerDoc("v1", new OpenApiInfo { Title = "IdentityServer", Version = "v1" });
            // });

            // 数据库配置系统应用用户数据上下文
            // services.AddDbContext<ApplicationDbContext>(options =>
            //     options.UseMySql(connectionString));
            // 启用 Identity 服务 添加指定的用户和角色类型的默认标识系统配置
            // services.AddIdentity<ApplicationUser, IdentityRole>()
            //     .AddEntityFrameworkStores<ApplicationDbContext>()
            //     .AddDefaultTokenProviders();

            services.AddSingleton(new HttpClient());
            services.AddScoped<IVerificationCodeService, VerificationCodeService>()
                .AddScoped<IUserService, UserService>();

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                options.EmitStaticAudienceClaim = true;
            })
                // .AddTestUsers(TestUsers.Users)
                .AddAspNetIdentity<ApplicationUser>()
                // SMS Validator
                .AddExtensionGrantValidator<SMSGrantValidator>()
                // this adds the config data from DB (clients, resources, CORS)
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryClients(Config.Clients);
                // .AddConfigurationStore(options =>
                // {
                //     options.ConfigureDbContext = builder =>
                //         builder.UseMySql(connectionString,
                //             sql => sql.MigrationsAssembly(migrationsAssembly));
                // })
                // this adds the operational data from DB (codes, tokens, consents)
                // .AddOperationalStore(options =>
                // {
                //     options.ConfigureDbContext = builder =>
                //         builder.UseMySql(connectionString,
                //             sql => sql.MigrationsAssembly(migrationsAssembly));

                    // this enables automatic token cleanup. this is optional.
                //     options.EnableTokenCleanup = true;
                // });


            // not recommended for production - you need to store your key material somewhere secure
            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // uncomment if you want to add MVC
            //app.UseStaticFiles();
            //app.UseRouting();

            app.UseIdentityServer();

            // uncomment, if you want to add MVC
            //app.UseAuthorization();
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapDefaultControllerRoute();
            //});
        }
    }
}

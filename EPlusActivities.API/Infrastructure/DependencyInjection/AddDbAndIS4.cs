using System;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Services.IdentityServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace EPlusActivities.API.Infrastructure.DependencyInjection
{
    public static partial class ServiceCollectionDependencyInjection
    {
        public static void AddDbAndIS4(
            this IServiceCollection services,
            string connectionString,
            MySqlServerVersion serverVersion,
            string migrationsAssembly,
            bool isDevelopment
        )
        {
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseMySql(connectionString, serverVersion)
            );

            // 启用 Identity 服务 添加指定的用户和角色类型的默认标识系统配置
            services
                .AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

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
            if (isDevelopment)
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
                                    serverVersion,
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
                                    serverVersion,
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
                        options.Authority = isDevelopment
                            ? "http://localhost:52537"
                            : "http://localhost:80";

                        //对应Idp中ApiResource的Name
                        options.Audience = "eplus.api";

                        //不使用https
                        options.RequireHttpsMetadata = false;

                        options.TokenValidationParameters.ValidateIssuer = false;
                    }
                );
        }
    }
}

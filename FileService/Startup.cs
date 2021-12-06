using System;
using System.Reflection;
using FileService.Data;
using FileService.Extensions;
using FileService.Services.GrpcService;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FileService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // services.AddControllers();
            services.AddGrpc();

            #region Database
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.AddDbContext<ApplicationDbContext>(
                options =>
                    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            );
            #endregion

            #region DI
            services.AddCustomDependencies();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            #region Seed data
            DbInitializer.CreateStorageDirectory(app: app);
            DbInitializer.Initialize(app: app, environment: env);
            #endregion

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpLogging();
            // app.UseHttpsRedirection();

            app.UseStaticFiles(
                new StaticFileOptions
                {
                    ServeUnknownFileTypes = true,
                    DefaultContentType = "image/png"
                }
            );

            app.UseRouting();

            // app.UseAuthorization();

            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapGrpcService<GrpcService>();
                    // endpoints.MapControllers();
                }
            );
        }
    }
}

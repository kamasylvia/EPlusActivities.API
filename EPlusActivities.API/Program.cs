using System;
using System.IO;
using System.Threading.Tasks;
using EPlusActivities.API.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace EPlusActivities.API
{
    public class Program
    {
        public static IConfiguration Configuration { get; } =
            new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile(
                    $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
                    optional: true
                )
                .AddEnvironmentVariables()
                .Build();

        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            try
            {
                Log.Information("启动主机");
                var host = CreateHostBuilder(args).Build();
                await new ConfigurationDbContextSeeder().SeedAsync(host);
                await new ApplicationDbContextSeeder().SeedAsync(host);
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "主机意外终止");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(
                    webBuilder =>
                    {
                        webBuilder.UseStartup<Startup>();
                    }
                );
    }
}

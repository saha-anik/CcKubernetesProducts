using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CcKubernetes.Data;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace CcKubernetes
{
    public class Program
    {
        public static void Main(string[] args)
        {
           var host= CreateHostBuilder(args).Build();
           CreateDbIfNotExists(host);
            host.Run();
        }

        private static void CreateDbIfNotExists(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ProductsContext>();
                    Initialize(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
        }

        public static void Initialize(ProductsContext context)
        {
            context.Database.EnsureCreated();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((ctx, provider, loggerConfig) =>
                {
                    loggerConfig
                        .ReadFrom.Configuration(ctx.Configuration) // minimum levels defined per project in json files 
                        .Enrich.FromLogContext()
                        .WriteTo.Console()
                        .WriteTo.Seq($"http://{ctx.Configuration.GetConnectionString("Seq")}:{ctx.Configuration.GetConnectionString("SeqPort")}");
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

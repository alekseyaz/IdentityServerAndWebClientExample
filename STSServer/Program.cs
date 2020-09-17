using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Zaac.STSServer.Services.Certificates;

namespace Zaac.STSServer
{
    public class Program
    {
        public static readonly string Namespace = typeof(Program).Namespace;
        public static readonly string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);

        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .CreateLogger();

            try
            {
                Log.Information("Configuring web host ({ApplicationContext})...", AppName);
                var host = CreateHostBuilder(args).Build();

                //Log.Information("Applying migrations ({ApplicationContext})...", AppName);
                //host.MigrateDbContext<ApplicationDbContext>((context, services) =>
                //{
                //    var env = services.GetService<IWebHostEnvironment>();
                //    var logger = services.GetService<ILogger<ApplicationDbContextSeed>>();
                //    var settings = services.GetService<IOptions<AppSettings>>();

                //    new ApplicationDbContextSeed()
                //        .SeedAsync(context, env, logger, settings)
                //        .Wait();
                //});

                Log.Information("Starting web host ({ApplicationContext})...", AppName);
                host.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", AppName);
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                    .UseKestrel(
                        options =>
                        {
                            options.AddServerHeader = false;
                            options.Listen(IPAddress.Any, 5001, listenOptions =>
                            {
                                listenOptions.UseHttps(Certificate.GetForKestrel());
                            });
                        }
                    )
                    .CaptureStartupErrors(false)
                    .ConfigureAppConfiguration((context, config) =>
                    {
                        IHostEnvironment env = context.HostingEnvironment;
                        config.SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                            .AddEnvironmentVariables();
                        //.AddUserSecrets("your user secret....");

                    })
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseSerilog((hostingContext, loggerConfiguration) =>
                    {
                        var configuration = hostingContext.Configuration;
                        var seqServerUrl = configuration["Serilog:SeqServerUrl"];
                        var logstashUrl = configuration["Serilog:LogstashgUrl"];

                        loggerConfiguration.MinimumLevel.Verbose()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                            .ReadFrom.Configuration(configuration)
                            .Enrich.WithProperty("ApplicationContext", AppName)
                            .Enrich.FromLogContext()
                            .WriteTo.Console(theme: AnsiConsoleTheme.Code);
                        if (!string.IsNullOrWhiteSpace(seqServerUrl))
                        {
                            //loggerConfiguration.WriteTo.Seq(seqServerUrl);
                        }
                        if (!string.IsNullOrWhiteSpace(logstashUrl))
                        {
                            //loggerConfiguration.WriteTo.Http(logstashUrl);
                        }
                    });
                });
    }
}

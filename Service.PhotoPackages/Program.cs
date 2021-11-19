using System.Threading;
using Lib.Consul;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Prometheus.DotNetRuntime;
using Serilog;

namespace Service.PhotoPackages
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            DotNetRuntimeStatsBuilder.Default().StartCollecting();
            BuildWebHost(args).Run();
            cancellationTokenSource.Cancel();
        }

        private static IWebHost BuildWebHost(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(
                    (_, builder) =>
                    {
                        builder
                            .AddConsul()
                            .AddEnvironmentVariables();
                    })
                .UseStartup<Startup>()
#if DEBUG
                .UseSerilog((_, c) =>
                {
                    c.MinimumLevel.Debug()
                        .Enrich.FromLogContext()
                        .Enrich.WithProperty("Type", typeof(Program).Assembly.GetName().Name)
                        .WriteTo.Console();
                })
#else
                .UseSerilog((b, c) =>
                {
                    c.ReadFrom.Configuration(b.Configuration)
                     .Enrich.WithProperty("Type", typeof(Program).Assembly.GetName().Name); 
                })
#endif
                .Build();

            return host;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using Lib.Consul;
using Lib.EventBus;
using Lib.HealthChecks;
using Lib.Swagger;
using Lib.Trace;
using Mbr.Bootstraper.Contracts;
using Mbr.Lib.Middleware.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Prometheus;
using Service.PhotoPackages.Consumers;
using Service.PhotoPackages.Dal;
using Service.PhotoPackages.Filters;
using Service.PhotoPackages.ServiceLayer;
using Service.Settings.Client;
using Service.Settings.Client.Contracts.Enums;
using StackExchange.Redis;

namespace Service.PhotoPackages
{
    public class Startup
    {
        #region Private properties

        private IConfiguration Configuration { get; }

        private readonly string[] _ignoreRoutes = {"/healthcheck", "/metrics", "/swagger"};

        private static readonly List<ClientTypes> ClientTypes = new()
        {
            Service.Settings.Client.Contracts.Enums.ClientTypes.Params,
            Service.Settings.Client.Contracts.Enums.ClientTypes.Settings,
            Service.Settings.Client.Contracts.Enums.ClientTypes.Decisions
        };

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #endregion

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(o => { o.Filters.Add<ExceptionFilter>(); })
                .AddNewtonsoftJson(o => { o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore; })
                .AddFluentValidation(opt => { opt.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly()); });
            services.AddSwagger();
            services.AddApiInfoMiddleware();
            services.AddExceptionHandlingMiddleware();
            services.AddLoggingMiddleware();
            services.AddMemoryCache();
            services.AddHealthChecks()
                .AddUrlGroup(
                    new Uri(Configuration.GetSection("HttpServiceBindings:LoadBalancer:BaseUrl").Value +
                            "/healthcheck"), "load-balancer")
                .AddProcessAllocatedMemoryHealthCheck();
            services.AddJaeger(Configuration, _ignoreRoutes);
            services.AddTransient<IStartupFilter, MigrationApplyStartupFilter>();

            services.AddEventBus(Configuration, typeof(Startup).Assembly,
                new[] {typeof(CreatePhotoPackageCommandConsumer).Assembly});
            services.AddClients(Configuration, GetType().Assembly, ClientTypes);

            services.AddConsulServiceDiscovery();

            var cong = new ConfigurationOptions
            {
                EndPoints =
                {
                    new DnsEndPoint(Configuration.GetValue<string>("Redis:Hosts:0:Host"),
                        Configuration.GetValue<int>("Redis:Hosts:0:Port"))

                },
                ResolveDns = true,
                ConnectRetry = 6,
                ConnectTimeout = 10000,
                AbortOnConnectFail = false
            };
            services.AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(cong));

            services.AddScoped(ctx => ctx.GetRequiredService<IConnectionMultiplexer>().GetDatabase());
            foreach (var settingItem in Settings) settingItem.Configure(services, Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime,
            IApiVersionDescriptionProvider provider)
        {
            app.UseMetricServer();
            app.UseHttpMetrics();

            app.UseSwagger(provider);

            app.UseApiInfoMiddleware();
            app.UseCorrelationIdMiddleware();
            app.UseExceptionHandlingMiddleware();
            app.UseLoggingMiddleware(_ignoreRoutes, _ignoreRoutes);

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("", context =>
                {
                    context.Response.Redirect("/swagger/index.html", permanent: false);
                    return Task.CompletedTask;
                });
                endpoints.MapHealthChecks("/healthcheck");
                endpoints.MapControllers();
            });
            app.UseConsulServiceDiscovery(appLifetime);
        }

        private IEnumerable<ISettingsModule> Settings
        {
            get
            {
                yield return new DalModule();
                yield return new ServiceModule();
            }
        }
    }
}
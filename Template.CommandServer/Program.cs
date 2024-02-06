using HostedServiceExtension.CronosJobScheduler;
using HostedServiceExtension.KestrelTcpServer;

using Microsoft.Extensions.Diagnostics.HealthChecks;

using OpenTelemetry.Metrics;

using Serilog;

using Template.CommandServer;
using Template.CommandServer.Application.Health;
using Template.CommandServer.Application.Metrics;
using Template.CommandServer.Handlers;
using Template.CommandServer.Jobs;
using Template.CommandServer.Service;
using Template.CommandServer.Settings;

Directory.SetCurrentDirectory(AppContext.BaseDirectory);

var host = Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .UseSystemd()
    .ConfigureLogging(config =>
    {
        config.ClearProviders();
    })
    .ConfigureServices((context, services) =>
    {
        var setting = context.Configuration.GetSection("Server").Get<ServerSetting>()!;

        // Logging
        services.AddSerilog(options =>
        {
            options.ReadFrom.Configuration(context.Configuration);
        });

        // Health
        services
            .AddHealthChecks()
            .AddCheck("test", () => HealthCheckResult.Healthy());
        services.AddSingleton<IHealthCheckPublisher, HealthCheckPublisher>();
        services.AddSingleton<HealthCheckState>();
        services.Configure<HealthCheckPublisherOptions>(options =>
        {
            options.Delay = TimeSpan.FromSeconds(5);
            options.Period = TimeSpan.FromSeconds(15);
        });

        // OpenTelemetry
        services
            .AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics
                    .AddRuntimeInstrumentation()
                    .AddProcessInstrumentation()
                    .AddApplicationInstrumentation();

                // http://localhost:9464/metrics
                metrics.AddPrometheusHttpListener();
            });

        // Handler
        services.AddTcpServer(options =>
        {
            options.ListenAnyIP<CommandHandler>(setting.Port);
        });
        services.AddCommands();
        services.AddSingleton(new CommandSetting
        {
            AllowAnonymous = setting.AllowAnonymous
        });

        // Job
        services.AddJobScheduler(options =>
        {
            options.UseJob<ScheduleJob>(setting.Cron);
        });

        // Service
        services.AddSingleton<DataService>();
        services.AddSingleton(new AuthorizeServiceOption
        {
            PublicKey = setting.PublicKey
        });
        services.AddSingleton<IAuthorizeService, AuthorizeService>();
    })
    .Build();

var log = host.Services.GetRequiredService<ILogger<Program>>();
ThreadPool.GetMinThreads(out var workerThreads, out var completionPortThreads);
log.InfoServiceStart();
log.InfoServiceSettingsEnvironment(typeof(Program).Assembly.GetName().Version, Environment.Version, Environment.CurrentDirectory);
log.InfoServiceSettingsGC(GCSettings.IsServerGC, GCSettings.LatencyMode, GCSettings.LargeObjectHeapCompactionMode);
log.InfoServiceSettingsThreadPool(workerThreads, completionPortThreads);

await host.RunAsync();

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

        // Job
        services.AddJobScheduler(options =>
        {
            options.UseJob<ScheduleJob>(setting.Cron);
        });

        // Service
        services.AddSingleton<DataService>();
    })
    .Build();

var log = host.Services.GetRequiredService<ILogger<Program>>();
log.InfoServiceStart();

await host.RunAsync();

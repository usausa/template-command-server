using HostedServiceExtension.CronosJobScheduler;
using HostedServiceExtension.KestrelTcpServer;

using OpenTelemetry.Metrics;

using Serilog;

using Template.CommandServer;
using Template.CommandServer.Handlers;
using Template.CommandServer.Jobs;
using Template.CommandServer.Service;
using Template.CommandServer.Settings;

Directory.SetCurrentDirectory(AppContext.BaseDirectory);

var builder = Host.CreateDefaultBuilder(args)
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

        // OpenTelemetry
        services
            .AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics
                    .AddRuntimeInstrumentation();

                metrics.AddPrometheusHttpListener();
            });

        // TODO health?

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
    });

var host = builder.Build();

var log = host.Services.GetRequiredService<ILogger<Program>>();
log.InfoServiceStart();

await host.RunAsync();

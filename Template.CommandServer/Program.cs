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

var builder = Host.CreateApplicationBuilder(args);

// Service
builder.Services
    .AddWindowsService()
    .AddSystemd();

// Logging
builder.Logging.ClearProviders();
builder.Services.AddSerilog(options =>
{
    options.ReadFrom.Configuration(builder.Configuration);
});

var setting = builder.Configuration.GetSection("Server").Get<ServerSetting>()!;

// Logging
builder.Services.AddSerilog(options =>
{
    options.ReadFrom.Configuration(builder.Configuration);
});

// Health
builder.Services
    .AddHealthChecks()
    .AddCheck("test", () => HealthCheckResult.Healthy());
builder.Services.AddSingleton<IHealthCheckPublisher, HealthCheckPublisher>();
builder.Services.AddSingleton<HealthCheckState>();
builder.Services.Configure<HealthCheckPublisherOptions>(options =>
{
    options.Delay = TimeSpan.FromSeconds(5);
    options.Period = TimeSpan.FromSeconds(15);
});

// OpenTelemetry
builder.Services
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
builder.Services.AddTcpServer(options =>
{
    options.ListenAnyIP<CommandHandler>(setting.Port);
});
builder.Services.AddCommands();
builder.Services.AddSingleton(new CommandSetting
{
    AllowAnonymous = setting.AllowAnonymous
});

// Job
builder.Services.AddJobScheduler(options =>
{
    options.UseJob<ScheduleJob>(setting.Cron);
});

// Service
builder.Services.AddSingleton<DataService>();
builder.Services.AddSingleton(new AuthorizeServiceOption
{
    PublicKey = setting.PublicKey
});
builder.Services.AddSingleton<IAuthorizeService, AuthorizeService>();

// Build
var host = builder.Build();

var log = host.Services.GetRequiredService<ILogger<Program>>();

// Startup information
ThreadPool.GetMinThreads(out var workerThreads, out var completionPortThreads);
log.InfoServiceStart();
log.InfoServiceSettingsEnvironment(typeof(Program).Assembly.GetName().Version, Environment.Version, Environment.CurrentDirectory);
log.InfoServiceSettingsGC(GCSettings.IsServerGC, GCSettings.LatencyMode, GCSettings.LargeObjectHeapCompactionMode);
log.InfoServiceSettingsThreadPool(workerThreads, completionPortThreads);

// Run
await host.RunAsync();

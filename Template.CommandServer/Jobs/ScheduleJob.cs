namespace Template.CommandServer.Jobs;

using HostedServiceExtension.CronosJobScheduler;

#pragma warning disable CA1848
public sealed class ScheduleJob : ISchedulerJob
{
    private readonly ILogger<ScheduleJob> log;

    public ScheduleJob(ILogger<ScheduleJob> log)
    {
        this.log = log;
    }

    public ValueTask ExecuteAsync(DateTimeOffset time, CancellationToken cancellationToken)
    {
        log.LogInformation("Run at {Time:HH:mm:ss}.", time);

        return ValueTask.CompletedTask;
    }
}
#pragma warning restore CA1848

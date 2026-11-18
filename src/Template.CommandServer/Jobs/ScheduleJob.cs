namespace Template.CommandServer.Jobs;

using Mofucat.JobScheduler;

public sealed class ScheduleJob : ISchedulerJob
{
    private readonly ILogger<ScheduleJob> log;

    public ScheduleJob(ILogger<ScheduleJob> log)
    {
        this.log = log;
    }

    public ValueTask ExecuteAsync(DateTimeOffset time, CancellationToken cancellationToken)
    {
        log.InfoScheduleRun(time);

        return ValueTask.CompletedTask;
    }
}

namespace Template.CommandServer.Service;

public sealed class StatsService
{
    private readonly TimeProvider timeProvider;

    private long commandCount;

    private long StartTimestamp { get; }

    public long CommandCount => Interlocked.Read(ref commandCount);

    public long UptimeSeconds => (long)timeProvider.GetElapsedTime(StartTimestamp).TotalSeconds;

    public StatsService(TimeProvider timeProvider)
    {
        this.timeProvider = timeProvider;
        StartTimestamp = timeProvider.GetTimestamp();
    }

    public void IncrementCommand() => Interlocked.Increment(ref commandCount);
}

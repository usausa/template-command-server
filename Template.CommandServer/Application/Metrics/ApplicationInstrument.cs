namespace Template.CommandServer.Application.Metrics;

using System.Diagnostics;
using System.Diagnostics.Metrics;

public sealed class ApplicationInstrument : IDisposable
{
    internal const string MeterName = "Application";

    private readonly Meter meter;

    private readonly Counter<long> commandExecution;

    private readonly Histogram<double> commandDuration;

    public ApplicationInstrument(IMeterFactory meterFactory)
    {
        meter = meterFactory.Create(MeterName, typeof(ApplicationInstrument).Assembly.GetName().Version!.ToString());

        meter.CreateObservableCounter("application.uptime", ObserveValue);

        commandExecution = meter.CreateCounter<long>("command.execution", description: "Command execution count.");
        commandDuration = meter.CreateHistogram<double>("command.duration", unit: "ms", description: "Command execution duration.");
    }

    public void Dispose()
    {
        meter.Dispose();
    }

    public void RecordCommand(string command, double elapsed)
    {
        var tag = new KeyValuePair<string, object?>("command", command);
        commandExecution.Add(1, tag);
        commandDuration.Record(elapsed, tag);
    }

    private static long ObserveValue() =>
        (long)(DateTime.Now - Process.GetCurrentProcess().StartTime).TotalSeconds;
}

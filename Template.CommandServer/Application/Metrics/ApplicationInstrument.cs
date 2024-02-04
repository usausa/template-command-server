namespace Template.CommandServer.Application.Metrics;

using System.Diagnostics;
using System.Diagnostics.Metrics;

public sealed class ApplicationInstrument : IDisposable
{
    internal const string MeterName = "Application";

    private readonly Meter meter;

    public ApplicationInstrument(IMeterFactory meterFactory)
    {
        meter = meterFactory.Create(MeterName, typeof(ApplicationInstrument).Assembly.GetName().Version!.ToString());

        meter.CreateObservableCounter("application.uptime", ObserveValue);
    }

    public void Dispose()
    {
        meter.Dispose();
    }

    private static long ObserveValue() =>
        (long)(DateTime.Now - Process.GetCurrentProcess().StartTime).TotalSeconds;
}

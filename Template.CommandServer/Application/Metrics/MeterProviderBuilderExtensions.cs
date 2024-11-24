namespace Template.CommandServer.Application.Metrics;

using System.Diagnostics.Metrics;

using OpenTelemetry.Metrics;

public static class MeterProviderBuilderExtensions
{
    public static MeterProviderBuilder AddApplicationInstrumentation(this MeterProviderBuilder builder)
    {
        builder.AddInstrumentation(static p => new ApplicationInstrument(p.GetRequiredService<IMeterFactory>()));
        builder.AddMeter(ApplicationInstrument.MeterName);
        return builder;
    }
}

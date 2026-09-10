namespace Template.CommandServer.Application.Metrics;

using OpenTelemetry.Metrics;

public static class MeterProviderBuilderExtensions
{
    public static IServiceCollection AddApplicationInstrument(this IServiceCollection services)
    {
        services.AddSingleton<ApplicationInstrument>();
        return services;
    }

    public static MeterProviderBuilder AddApplicationInstrumentation(this MeterProviderBuilder builder)
    {
        builder.AddInstrumentation(static p => p.GetRequiredService<ApplicationInstrument>());
        builder.AddMeter(ApplicationInstrument.MeterName);
        return builder;
    }
}

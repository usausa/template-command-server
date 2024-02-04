namespace Template.CommandServer.Application.Health;

using Microsoft.Extensions.Diagnostics.HealthChecks;

public sealed class HealthCheckPublisher : IHealthCheckPublisher
{
    private readonly HealthCheckState healthState;

    public HealthCheckPublisher(HealthCheckState healthState)
    {
        this.healthState = healthState;
    }

    public Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
    {
        healthState.HealthStatus = report.Status;
        return Task.CompletedTask;
    }
}

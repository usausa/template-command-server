namespace Template.CommandServer.Application.Health;

using Microsoft.Extensions.Diagnostics.HealthChecks;

public sealed class HealthCheckState
{
    private readonly object sync = new();

    private HealthStatus status;

    public HealthStatus HealthStatus
    {
        get
        {
            lock (sync)
            {
                return status;
            }
        }
        set
        {
            lock (sync)
            {
                status = value;
            }
        }
    }
}

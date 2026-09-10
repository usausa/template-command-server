namespace Template.CommandServer.Application.Health;

using Microsoft.Extensions.Diagnostics.HealthChecks;

public sealed class HealthCheckState
{
    private readonly Lock sync = new();

    public HealthStatus HealthStatus
    {
        get
        {
            lock (sync)
            {
                return field;
            }
        }
        set
        {
            lock (sync)
            {
                field = value;
            }
        }
    }
}

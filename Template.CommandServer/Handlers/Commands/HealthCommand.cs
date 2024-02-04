namespace Template.CommandServer.Handlers.Commands;

using System.Buffers;

using Microsoft.Extensions.Diagnostics.HealthChecks;

using Template.CommandServer.Application.Health;

public sealed class HealthCommand : ICommand
{
    private readonly HealthCheckState healthCheckState;

    public HealthCommand(HealthCheckState healthCheckState)
    {
        this.healthCheckState = healthCheckState;
    }

    public bool Match(ReadOnlySequence<byte> command) => command.SequentialEqual("health"u8);

    public ValueTask<bool> ExecuteAsync(CommandContext context, ReadOnlySequence<byte> options, IBufferWriter<byte> writer)
    {
        var status = healthCheckState.HealthStatus;
        switch (status)
        {
            case HealthStatus.Healthy:
                writer.WriteAndAdvanceOk("healthy"u8);
                break;
            case HealthStatus.Unhealthy:
                writer.WriteAndAdvanceOk("unhealthy"u8);
                break;
            case HealthStatus.Degraded:
                writer.WriteAndAdvanceOk("degraded"u8);
                break;
        }

        return ValueTask.FromResult(true);
    }
}

namespace Template.CommandServer.Handlers.Commands;

using System.Buffers;

using Template.CommandServer.Service;

public sealed class StatsCommand : ICommand
{
    private readonly StatsService statsService;

    public string Name => "stats";

    public StatsCommand(StatsService statsService)
    {
        this.statsService = statsService;
    }

    public bool Match(ReadOnlySequence<byte> command) => command.SequentialEqual("stats"u8);

    public ValueTask<bool> ExecuteAsync(CommandContext context, ReadOnlySequence<byte> options, IBufferWriter<byte> writer)
    {
        if (!context.IsAllowed)
        {
            writer.WriteAndAdvanceNg();
            return ValueTask.FromResult(true);
        }

        writer.WriteAndAdvanceOk(Encoding.ASCII.GetBytes($"uptime={statsService.UptimeSeconds} commands={statsService.CommandCount}"));

        return ValueTask.FromResult(true);
    }
}

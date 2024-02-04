namespace Template.CommandServer.Handlers.Commands;

using System.Buffers;

using Template.CommandServer.Service;

public sealed class GetCommand : ICommand
{
    private readonly DataService dataService;

    public GetCommand(DataService dataService)
    {
        this.dataService = dataService;
    }

    public bool Match(ReadOnlySequence<byte> command) => command.SequentialEqual("get"u8);

    public ValueTask<bool> ExecuteAsync(CommandContext context, ReadOnlySequence<byte> options, IBufferWriter<byte> writer)
    {
        if (!context.IsAllowed)
        {
            writer.WriteAndAdvanceNg();
            return ValueTask.FromResult(true);
        }

        var value = dataService.QueryValue();
        writer.WriteAndAdvanceOk(value);

        return ValueTask.FromResult(true);
    }
}

namespace Template.CommandServer.Handlers.Commands;

using System.Buffers;

using Template.CommandServer.Service;

public sealed class SetCommand : ICommand
{
    private readonly DataService dataService;

    public SetCommand(DataService dataService)
    {
        this.dataService = dataService;
    }

    public bool Match(ReadOnlySequence<byte> command) => command.SequentialEqual("set"u8);

    public ValueTask<bool> ExecuteAsync(CommandContext context, ReadOnlySequence<byte> options, IBufferWriter<byte> writer)
    {
        if (!context.IsAllowed)
        {
            writer.WriteAndAdvanceNg();
            return ValueTask.FromResult(true);
        }

        if (options.TryParse(out var value))
        {
            dataService.UpdateValue(value);
            writer.WriteAndAdvanceOk();
        }
        else
        {
            writer.WriteAndAdvanceNg();
        }

        return ValueTask.FromResult(true);
    }
}

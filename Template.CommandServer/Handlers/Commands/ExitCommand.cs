namespace Template.CommandServer.Handlers.Commands;

using System.Buffers;

public sealed class ExitCommand : ICommand
{
    public bool Match(ReadOnlySequence<byte> command) => command.SequentialEqual("exit"u8);

    public ValueTask<bool> ExecuteAsync(CommandContext context, ReadOnlySequence<byte> options, IBufferWriter<byte> writer) => ValueTask.FromResult(false);
}

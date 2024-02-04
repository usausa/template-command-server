namespace Template.CommandServer.Handlers.Commands;

using System.Buffers;

public interface ICommand
{
    bool Match(ReadOnlySequence<byte> command);

    ValueTask<bool> ExecuteAsync(CommandContext context, ReadOnlySequence<byte> options, IBufferWriter<byte> writer);
}

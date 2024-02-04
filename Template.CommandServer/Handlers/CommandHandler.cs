namespace Template.CommandServer.Handlers;

using System;
using System.Buffers;

using Microsoft.AspNetCore.Connections;

using Template.CommandServer.Handlers.Commands;

public sealed class CommandHandler : ConnectionHandler
{
    private enum CommandResult
    {
        Success,
        Unknown,
        Quit
    }

    private readonly ILogger<CommandHandler> log;

    private readonly CommandSetting setting;

    private readonly ICommand[] commands;

    public CommandHandler(ILogger<CommandHandler> log, CommandSetting setting, IEnumerable<ICommand> commands)
    {
        this.log = log;
        this.setting = setting;
        this.commands = commands.ToArray();
    }

    public override async Task OnConnectedAsync(ConnectionContext connection)
    {
        log.DebugHandlerConnected(connection.ConnectionId);

        var timeout = new CancellationTokenSource();
        try
        {
            var context = new CommandContext
            {
                AllowAnonymous = setting.AllowAnonymous
            };

            var running = true;
            while (running)
            {
                timeout.CancelAfter(30_000);
                var result = await connection.Transport.Input.ReadAsync(timeout.Token);
                var resetTimeout = timeout.TryReset();

                var buffer = result.Buffer;

                while (!buffer.IsEmpty && ReadLine(ref buffer, out var line))
                {
                    var commandResult = await ProcessLineAsync(context, line, connection.Transport.Output);
                    if (commandResult == CommandResult.Unknown)
                    {
                        connection.Transport.Output.WriteAndAdvanceNg();
                    }
                    else if (commandResult == CommandResult.Quit)
                    {
                        running = false;
                        break;
                    }

                    await connection.Transport.Output.FlushAsync(CancellationToken.None);
                }

                if (!running || result.IsCompleted)
                {
                    break;
                }

                connection.Transport.Input.AdvanceTo(buffer.Start, buffer.End);

                if (!resetTimeout)
                {
                    timeout.Dispose();
                    timeout = new CancellationTokenSource();
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Ignore
        }
        finally
        {
            timeout.Dispose();

            log.DebugHandlerDisconnected(connection.ConnectionId);
        }
    }

    private static bool ReadLine(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> line)
    {
        var reader = new SequenceReader<byte>(buffer);
        if (reader.TryReadTo(out ReadOnlySequence<byte> l, "\r\n"u8))
        {
            buffer = buffer.Slice(reader.Position);
            line = l;
            return true;
        }

        line = default;
        return false;
    }

    private async ValueTask<CommandResult> ProcessLineAsync(CommandContext context, ReadOnlySequence<byte> buffer, IBufferWriter<byte> writer)
    {
        CommandHelper.Split(ref buffer, out var first, (byte)' ');
        foreach (var command in commands)
        {
            if (command.Match(first))
            {
                return await command.ExecuteAsync(context, buffer, writer) ? CommandResult.Success : CommandResult.Quit;
            }
        }

        return CommandResult.Unknown;
    }
}

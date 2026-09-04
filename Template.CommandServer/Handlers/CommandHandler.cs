namespace Template.CommandServer.Handlers;

using System;
using System.Buffers;

using Microsoft.AspNetCore.Connections;

using Smart.Threading;

using Template.CommandServer.Application.Metrics;
using Template.CommandServer.Handlers.Commands;
using Template.CommandServer.Service;

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

    private readonly ApplicationInstrument instrument;

    private readonly StatsService statsService;

    private readonly TimeProvider timeProvider;

    public CommandHandler(
        ILogger<CommandHandler> log,
        CommandSetting setting,
        IEnumerable<ICommand> commands,
        ApplicationInstrument instrument,
        StatsService statsService,
        TimeProvider timeProvider)
    {
        this.log = log;
        this.setting = setting;
        this.commands = [.. commands];
        this.instrument = instrument;
        this.statsService = statsService;
        this.timeProvider = timeProvider;
    }

    public override async Task OnConnectedAsync(ConnectionContext connection)
    {
        log.DebugHandlerConnected(connection.ConnectionId);

        try
        {
            var context = new CommandContext
            {
                AllowAnonymous = setting.AllowAnonymous
            };

            using var timeout = new ReusableCancellationTokenSource();
            while (true)
            {
                timeout.CancelAfter(30_000);
                var result = await connection.Transport.Input.ReadAsync(timeout.Token);
                var buffer = result.Buffer;

                var running = true;
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

                timeout.Reset();
            }
        }
        catch (OperationCanceledException)
        {
            // Ignore
        }
        finally
        {
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
                var timestamp = timeProvider.GetTimestamp();
                var result = await command.ExecuteAsync(context, buffer, writer);

                instrument.RecordCommand(command.Name, timeProvider.GetElapsedTime(timestamp).TotalMilliseconds);
                statsService.IncrementCommand();

                return result ? CommandResult.Success : CommandResult.Quit;
            }
        }

        return CommandResult.Unknown;
    }
}

namespace Template.CommandServer.Handlers.Commands;

using System.Buffers;

public sealed class ConfigCommand : ICommand
{
    private readonly CommandSetting setting;

    public string Name => "config";

    public ConfigCommand(CommandSetting setting)
    {
        this.setting = setting;
    }

    public bool Match(ReadOnlySequence<byte> command) => command.SequentialEqual("config"u8);

    public ValueTask<bool> ExecuteAsync(CommandContext context, ReadOnlySequence<byte> options, IBufferWriter<byte> writer)
    {
        if (!context.IsAllowed)
        {
            writer.WriteAndAdvanceNg();
            return ValueTask.FromResult(true);
        }

        writer.WriteAndAdvanceOk(Encoding.ASCII.GetBytes($"allowAnonymous={setting.AllowAnonymous}"));

        return ValueTask.FromResult(true);
    }
}

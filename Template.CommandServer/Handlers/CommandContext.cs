namespace Template.CommandServer.Handlers;

#pragma warning disable CA1819
public sealed class CommandContext
{
    public byte[] Challenge { get; set; } = [];

    public bool IsAuthorized { get; set; }
}
#pragma warning restore CA1819

namespace Template.CommandServer.Handlers;

#pragma warning disable CA1819
public sealed class CommandContext
{
    public bool AllowAnonymous { get; set; }

    public byte[] Token { get; set; } = [];

    public bool IsAuthorized { get; set; }

    public bool IsAllowed => AllowAnonymous || IsAuthorized;
}
#pragma warning restore CA1819

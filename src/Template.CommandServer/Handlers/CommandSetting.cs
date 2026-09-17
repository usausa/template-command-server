namespace Template.CommandServer.Handlers;

public sealed class CommandSetting
{
    public bool AllowAnonymous { get; set; }

    public int ReadTimeout { get; set; }

    public int MaxLineLength { get; set; }
}

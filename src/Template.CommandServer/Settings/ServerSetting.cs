namespace Template.CommandServer.Settings;

public sealed class ServerSetting
{
    public int Port { get; set; }

    public bool AllowAnonymous { get; set; }

    public int ReadTimeout { get; set; }

    public int MaxLineLength { get; set; }

    public required string PublicKey { get; set; }

    public required string Cron { get; set; }
}

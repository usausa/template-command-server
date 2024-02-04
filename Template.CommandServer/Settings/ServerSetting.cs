namespace Template.CommandServer.Settings;

public sealed class ServerSetting
{
    public int Port { get; set; }

    public required string PublicKey { get; set; }

    public required string Cron { get; set; }
}

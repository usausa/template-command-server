namespace Template.CommandServer;

using System.Runtime;

public static partial class Log
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Service start.")]
    public static partial void InfoServiceStart(this ILogger logger);
}

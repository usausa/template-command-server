namespace Template.CommandServer.Jobs;

internal static partial class Log
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Run at {time:HH:mm:ss}.")]
    public static partial void InfoScheduleRun(this ILogger logger, DateTimeOffset time);
}

namespace Template.CommandServer.Handlers;

internal static partial class Log
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Handler connected. connectionId=[{connectionId}]")]
    public static partial void DebugHandlerConnected(this ILogger logger, string connectionId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Handler disconnected. connectionId=[{connectionId}]")]
    public static partial void DebugHandlerDisconnected(this ILogger logger, string connectionId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Handler read timeout. connectionId=[{connectionId}]")]
    public static partial void WarnHandlerReadTimeout(this ILogger logger, string connectionId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Handler line too long. connectionId=[{connectionId}], length=[{length}]")]
    public static partial void WarnHandlerLineTooLong(this ILogger logger, string connectionId, long length);
}

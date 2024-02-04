namespace Template.CommandServer.Handlers.Commands;

using Template.CommandServer.Service;

public sealed class AuthorizeCommand : ICommand
{
    private readonly IAuthorizeService authorizeService;

    public AuthorizeCommand(IAuthorizeService authorizeService)
    {
        this.authorizeService = authorizeService;
    }

    public bool Match(ReadOnlySequence<byte> command) => command.SequentialEqual("authorize"u8);

    public ValueTask<bool> ExecuteAsync(CommandContext context, ReadOnlySequence<byte> options, IBufferWriter<byte> writer)
    {
        var signature = Convert.FromHexString(Encoding.ASCII.GetString(options.IsSingleSegment ? options.FirstSpan : options.ToArray()));
        if (authorizeService.VerifySignature(context.Token, signature))
        {
            context.IsAuthorized = true;
            writer.WriteAndAdvanceOk();
        }
        else
        {
            writer.WriteAndAdvanceNg();
        }

        return ValueTask.FromResult(true);
    }
}

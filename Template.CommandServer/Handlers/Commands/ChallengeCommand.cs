namespace Template.CommandServer.Handlers.Commands;

using Template.CommandServer.Service;

public sealed class ChallengeCommand : ICommand
{
    private readonly IAuthorizeService authorizeService;

    public ChallengeCommand(IAuthorizeService authorizeService)
    {
        this.authorizeService = authorizeService;
    }

    public bool Match(ReadOnlySequence<byte> command) => command.SequentialEqual("challenge"u8);

    public ValueTask<bool> ExecuteAsync(CommandContext context, ReadOnlySequence<byte> options, IBufferWriter<byte> writer)
    {
        context.Token = authorizeService.CreateToken();
        writer.WriteAndAdvanceOk(Encoding.ASCII.GetBytes(Convert.ToHexString(context.Token)));
        return ValueTask.FromResult(true);
    }
}

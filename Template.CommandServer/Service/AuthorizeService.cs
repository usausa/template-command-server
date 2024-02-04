namespace Template.CommandServer.Service;

using System.Security.Cryptography;

public interface IAuthorizeService
{
    byte[] CreateToken();

    bool VerifySignature(ReadOnlySpan<byte> token, ReadOnlySpan<byte> signature);
}

public sealed class AuthorizeServiceOption
{
    public string PublicKey { get; set; } = default!;
}

public sealed class AuthorizeService : IAuthorizeService
{
    private readonly AuthorizeServiceOption option;

    public AuthorizeService(AuthorizeServiceOption option)
    {
        this.option = option;
    }

    public byte[] CreateToken() => RandomNumberGenerator.GetBytes(32);

    public bool VerifySignature(ReadOnlySpan<byte> token, ReadOnlySpan<byte> signature)
    {
        using var rsa = RSA.Create();
        rsa.ImportFromPem(File.ReadAllText(option.PublicKey));
        return rsa.VerifyData(token, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }
}

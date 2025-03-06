namespace MiniFlexCrmApi.Auth;

public interface IEncryptionSecretProvider
{
    string GetSecret();
}
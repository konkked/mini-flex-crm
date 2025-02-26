namespace MiniFlexCrmApi.Api.Auth;

public interface IEncryptionSecretProvider
{
    string GetSecret();
}
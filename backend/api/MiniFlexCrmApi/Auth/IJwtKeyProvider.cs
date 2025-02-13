namespace MiniFlexCrmApi.Api.Auth;

public interface IJwtKeyProvider
{
    string GetKey();
}
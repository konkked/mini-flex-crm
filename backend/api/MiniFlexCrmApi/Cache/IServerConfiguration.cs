namespace MiniFlexCrmApi.Cache;

public interface IServerConfiguration
{
    string Host { get; }
    string? Password { get; }
    int Port { get; }
    bool UseTls { get; }
}
namespace MiniFlexCrmApi.Cache;

public record ServerConfiguration(string Host,
    string? Password = null, int Port = 6379, bool UseTls = false)
    : IServerConfiguration;
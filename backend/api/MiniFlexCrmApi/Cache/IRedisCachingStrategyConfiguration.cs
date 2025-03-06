namespace MiniFlexCrmApi.Cache;

public interface IRedisCachingStrategyConfiguration : ICachingStrategyConfiguration
{
    IServerConfiguration ServerConfiguration { get; }
}
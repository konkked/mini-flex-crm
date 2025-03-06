namespace MiniFlexCrmApi.Cache;

public class RedisCachingStrategyConfiguration(
    RateLimitingConfiguration rateLimitingConfiguration, 
    ServerConfiguration serverConfiguration)
    : IRedisCachingStrategyConfiguration
{
    public CacheType CacheType => CacheType.DistributedCache;
    public IRateLimitingConfiguration RateLimitingConfiguration => rateLimitingConfiguration;
    public IServerConfiguration ServerConfiguration => serverConfiguration;
}
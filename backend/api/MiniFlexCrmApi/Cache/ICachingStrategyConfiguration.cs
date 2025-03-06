namespace MiniFlexCrmApi.Cache;

public interface ICachingStrategyConfiguration
{
    CacheType CacheType { get; }
    IRateLimitingConfiguration RateLimitingConfiguration { get; }
}
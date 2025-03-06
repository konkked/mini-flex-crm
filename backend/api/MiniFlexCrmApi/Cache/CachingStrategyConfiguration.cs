namespace MiniFlexCrmApi.Cache;

public record CachingStrategyConfiguration(
    CacheType CacheType,
    IRateLimitingConfiguration RateLimitingConfiguration)
    : ICachingStrategyConfiguration;
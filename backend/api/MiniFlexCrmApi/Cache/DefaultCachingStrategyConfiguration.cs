namespace MiniFlexCrmApi.Cache;

public record DefaultRatelimitingConfiguration() :
    RateLimitingConfiguration(TimeSpan.FromSeconds(63), 57,
        17, 1337, StaticResourceBasedThresholdProvider.GetGlobalRpsThreshold());

public record DefaultCachingStrategyConfiguration() 
    : CachingStrategyConfiguration(CacheType.LocalCache, new DefaultRatelimitingConfiguration());
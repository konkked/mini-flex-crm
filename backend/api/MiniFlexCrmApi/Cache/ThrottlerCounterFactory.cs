namespace MiniFlexCrmApi.Cache;

public class ThrottlerCounterFactory(ICachingStrategyConfiguration config, ISlidingWindowCounter counter) : IThrottlerCounterFactory
{
    public IThrottlerCounter Create()
    {
        var windowSeconds = (int) config.RateLimitingConfiguration.WindowDuration.TotalSeconds;
        return new ThrottlerCounter(
            new BaseFixedSlidingWindowCounter(config.RateLimitingConfiguration.UserRpsThreshold * windowSeconds, windowSeconds, counter),
            new BaseFixedSlidingWindowCounter(config.RateLimitingConfiguration.TenantRpsThreshold * windowSeconds, windowSeconds, counter),
            new BaseFixedSlidingWindowCounter(config.RateLimitingConfiguration.UserRouteRpsThreshold * windowSeconds, windowSeconds, counter),
            new BaseFixedSlidingWindowCounter(config.RateLimitingConfiguration.UserRouteRpsThreshold * windowSeconds, windowSeconds, counter)
        );
    }
}
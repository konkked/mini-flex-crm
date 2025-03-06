namespace MiniFlexCrmApi.Cache;

public record RateLimitingConfiguration(
    TimeSpan WindowDuration,
    int UserRpsThreshold,
    int UserRouteRpsThreshold,
    int TenantRpsThreshold,
    int GlobalRpsThreshold)
    : IRateLimitingConfiguration;
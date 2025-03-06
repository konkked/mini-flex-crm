namespace MiniFlexCrmApi.Cache;

public interface IRateLimitingConfiguration
{
    public TimeSpan WindowDuration { get; }
    public int UserRpsThreshold { get; }
    public int UserRouteRpsThreshold { get; }
    public int TenantRpsThreshold { get; }
    public int GlobalRpsThreshold { get; }
}
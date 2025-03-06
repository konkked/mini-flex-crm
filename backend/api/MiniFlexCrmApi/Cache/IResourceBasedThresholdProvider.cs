namespace MiniFlexCrmApi.Cache;

public interface IResourceBasedThresholdProvider
{
    int GetGlobalRpsThreshold();
}
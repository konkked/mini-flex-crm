namespace MiniFlexCrmApi.Cache;

public static class StaticResourceBasedThresholdProvider
{
    private static IResourceBasedThresholdProvider _instance = new ResourceBasedThresholdProvider();
    public static int GetGlobalRpsThreshold()=> _instance.GetGlobalRpsThreshold();
}
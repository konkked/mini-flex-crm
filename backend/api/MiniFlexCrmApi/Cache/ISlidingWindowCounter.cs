namespace MiniFlexCrmApi.Cache;

public interface ISlidingWindowCounter
{
    Task<int> IncrWindowCountAsync(string keyPart, int windowSeconds, double threshold);
}
namespace MiniFlexCrmApi.Cache;

public class BaseFixedSlidingWindowCounter(int threshold, int windowSeconds, ISlidingWindowCounter counter) : IFixedSlidingWindowCounter
{
    public Task<int> IncrWindowCountAsync(string keyPart)
        => counter.IncrWindowCountAsync(keyPart, windowSeconds, threshold);
}
namespace MiniFlexCrmApi.Cache;

public interface IFixedSlidingWindowCounter
{
    Task<int> IncrWindowCountAsync(string keyPart);
}
namespace MiniFlexCrmApi.Cache;

public class RedisFixedSlidingWindowCounter(int threshold, int windowSeconds, RedisSlidingWindowCounter counter)
    : BaseFixedSlidingWindowCounter(threshold, windowSeconds, counter);
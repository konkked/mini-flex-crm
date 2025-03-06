namespace MiniFlexCrmApi.Cache;

public class LocalFixedSlidingWindowCounter(int threshold, int windowSeconds, LocalSlidingWindowCounter counter) 
    : BaseFixedSlidingWindowCounter(threshold, windowSeconds, counter);
using System.Collections.Concurrent;

namespace MiniFlexCrmApi.Cache;

public class LocalSlidingWindowCounter : ISlidingWindowCounter
{
    // In-memory storage: key -> (count, creation timestamp)
    private readonly ConcurrentDictionary<string, (int count, long timestamp)> _storage = new();

    public Task<int> IncrWindowCountAsync(string keyPart, int windowSeconds, double threshold)
    {
        var currTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var currBucket = currTime / windowSeconds;
        var prevBucket = currBucket - 1;
        var expiredBucket = currBucket - 2;

        // Construct keys
        var currKey = $"{keyPart}:{currBucket}";
        var prevKey = $"{keyPart}:{prevBucket}";

        // Evict key from two windows ago if it exists (no lock)
        _storage.TryRemove($"{keyPart}:{expiredBucket}", out _);

        // Get current and previous counts
        var currentEntry = _storage.GetOrAdd(currKey, _ => (0, currentTime: currTime));
        var currentCount = currentEntry.count;
        var prevCount = _storage.TryGetValue(prevKey, out var leftEntry) ? leftEntry.count : 0;

        // Calculate time passed since start of previous window
        var prevBucketStart = prevBucket * windowSeconds;
        double timePassed = currTime - prevBucketStart;
        var weight = 1.0 - (timePassed / windowSeconds);

        // Compute potential composite count with increment
        var potentialComposite = (currentCount + 1) + (weight * prevCount);

        // Check threshold before incrementing
        if (potentialComposite > threshold)
        {
            return Task.FromResult(-1);
        }

        // Safe to increment
        int newCount;
        do
        {
            currentCount = currentEntry.count;
            newCount = currentCount + 1;
        } while (!_storage.TryUpdate(currKey, (newCount, currentEntry.timestamp), currentEntry));

        var compositeCount = newCount + (weight * prevCount);
        return Task.FromResult(compositeCount > threshold ? -1: (int)Math.Ceiling(compositeCount));
    }
}
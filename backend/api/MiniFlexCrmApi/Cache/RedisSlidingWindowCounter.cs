using System.Globalization;
using StackExchange.Redis;

namespace MiniFlexCrmApi.Cache;

public class RedisSlidingWindowCounter(IConnectionMultiplexer connMultiplexer) : ISlidingWindowCounter
{
    private readonly IDatabase _db = connMultiplexer.GetDatabase();
    private const string _script = @"
-- Sliding Window Composite No Increment
-- ARGV[1]: key-part (e.g., ""user:123:rate"")
-- ARGV[2]: window_seconds (e.g., 60)
-- ARGV[3]: threshold (e.g., 100)

local key_part = ARGV[1]
local window_seconds = tonumber(ARGV[2])
local threshold = tonumber(ARGV[3])
local current_time = redis.call('TIME')[1] -- Unix time in seconds
local current_bucket = math.floor(current_time / window_seconds)
local prev_bucket = current_bucket - 1

-- Construct keys
local current_key = key_part .. ':' .. current_bucket
local left_key = key_part .. ':' .. prev_bucket

-- Get current and previous counts (before increment)
local current_count = tonumber(redis.call('GET', current_key) or 0)
local prev_count = tonumber(redis.call('GET', left_key) or 0)

-- Calculate time passed since start of previous window
local prev_bucket_start = prev_bucket * window_seconds
local time_passed = current_time - prev_bucket_start

-- Compute potential composite count with increment
local weight = 1.0 - (time_passed / window_seconds)
local potential_composite = (current_count + 1) + (weight * prev_count)

-- Check threshold before incrementing
if potential_composite > threshold then
    -- Return current composite without increment
    return -1
else
    -- Safe to increment
    current_count = redis.call('INCR', current_key)
    if current_count == 1 then
        redis.call('EXPIRE', current_key, (2 * window_seconds) + 1)
    end
    -- Return new composite count
    local composite_count = current_count + (weight * prev_count)
    return composite_count > threshold and -1 or composite_count
end";

    public async Task<int> IncrWindowCountAsync(string keyPart, int windowSeconds, double threshold)
    {
        var result = await _db.ScriptEvaluateAsync(
            _script,
            [],
            [keyPart, windowSeconds.ToString(), threshold.ToString(CultureInfo.InvariantCulture)]
        ).ConfigureAwait(false);

        return (int)Math.Ceiling((double)result); // Returns -1 or composite count
    }
}
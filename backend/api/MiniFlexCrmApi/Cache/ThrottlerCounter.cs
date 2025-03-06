using MiniFlexCrmApi.Models;
using MiniFlexCrmApi.Util;

namespace MiniFlexCrmApi.Cache;

public class ThrottlerCounter(
    IFixedSlidingWindowCounter userCounter,
    IFixedSlidingWindowCounter tenantCounter,
    IFixedSlidingWindowCounter userRouteCounter,
    IFixedSlidingWindowCounter globalRouteCounter) : IThrottlerCounter
{
    /// <summary>
    /// Determines if request exceeds configured threshold.
    /// </summary>
    public async Task<bool> ThunkAsync(RequestContext context)
    {
        var result = await globalRouteCounter.IncrWindowCountAsync("global");
        
        if (result == -1) 
            return false;
        
        result = await tenantCounter.IncrWindowCountAsync($"tenant:{context.TenantId}");
       
        if(result == -1)
            return false;

        if (string.IsNullOrEmpty(context.UserId)) 
            return true;
        
        result = await userCounter.IncrWindowCountAsync($"user:{context.UserId}");

        if (result == -1)
            return false;

        if (!string.IsNullOrEmpty(context.Path))
            result = await userRouteCounter.IncrWindowCountAsync(
                $"user:{context.UserId}:route:{Base62.EncodeString(context.Path)}");

        return result != -1;
    }
}
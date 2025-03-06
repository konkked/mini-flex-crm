using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Cache;

public interface IThrottlerCounter
{
    /// <summary>
    /// Determines if request exceeds configured threshold.
    /// </summary>
    Task<bool> ThunkAsync(RequestContext context);
}
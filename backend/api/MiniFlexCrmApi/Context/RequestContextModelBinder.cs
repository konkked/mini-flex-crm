using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MiniFlexCrmApi.Auth;
using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Context;

public class RequestContextModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext.ModelType != typeof(RequestContext))
            return Task.CompletedTask;

        var httpContext = bindingContext.HttpContext;
        var requestContext = RequestContextExtractor.Extract(httpContext);
        bindingContext.Result = ModelBindingResult.Success(requestContext);
        return Task.CompletedTask;
    }
}

public static class RequestContextExtractor 
{
    public static RequestContext Extract(HttpContext httpContext)
    {
        var user = httpContext.User;
        return new RequestContext
        {
            Method = httpContext.Request.Method,
            Path = httpContext.Request.Path,
            QueryString = httpContext.Request.QueryString.ToString(),
            ContentType = httpContext.Request.ContentType,
            Headers = httpContext.Request.Headers
                .ToDictionary(h => h.Key, h => string.Join(";", h.Value)),
            Cookies = httpContext.Request.Cookies
                .ToDictionary(c => c.Key, c => c.Value),
            UserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            Username = user.FindFirst(ClaimTypes.Name)?.Value,
            TenantId = int.TryParse(user.FindFirst(ServerCustomConstants.ClaimTypes.TenantId)?.Value, out var tenant) 
                ? tenant : null,
            Role = user.FindFirst(ClaimTypes.Role)?.Value,
            Claims = user.Claims.ToDictionary(c => c.Type, c => c.Value)
        };
    }
}
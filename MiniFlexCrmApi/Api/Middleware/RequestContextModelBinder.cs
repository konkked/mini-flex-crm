using Microsoft.AspNetCore.Mvc.ModelBinding;
using MiniFlexCrmApi.Api.Models;

namespace MiniFlexCrmApi.Api.Middleware;

public class RequestContextModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext.ModelType != typeof(RequestContext))
        {
            return Task.CompletedTask;
        }
        
        var httpContext = bindingContext.HttpContext;
        var clonedContext = new RequestContext
        {
            Method = httpContext.Request.Method,
            Path = httpContext.Request.Path,
            QueryString = httpContext.Request.QueryString.ToString(),
            ContentType = httpContext.Request.ContentType,
            Headers = httpContext.Request.Headers.ToDictionary(h => h.Key, h => string.Join(";", h.Value)),
            Cookies = httpContext.Request.Cookies.ToDictionary(c => c.Key, c => c.Value)
        };

        bindingContext.Result = ModelBindingResult.Success(clonedContext);
        return Task.CompletedTask;
    }
}
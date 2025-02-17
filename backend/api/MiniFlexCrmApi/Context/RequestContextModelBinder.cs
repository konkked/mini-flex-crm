using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MiniFlexCrmApi.Api.Auth;
using MiniFlexCrmApi.Api.Models;

namespace MiniFlexCrmApi.Api.Context;

public class RequestContextModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext.ModelType != typeof(RequestContext))
            return Task.CompletedTask;

        var httpContext = bindingContext.HttpContext;
        var user = httpContext.User;

        var requestContext = new RequestContext
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
            TenantId = int.TryParse(user.FindFirst(JwtCustomConstants.ClaimTypes.TenantId)?.Value, out var tenant) 
                ? tenant : null,
            Role = user.FindFirst(ClaimTypes.Role)?.Value,
            Claims = user.Claims.ToDictionary(c => c.Type, c => c.Value)
        };

        bindingContext.Result = ModelBindingResult.Success(requestContext);
        return Task.CompletedTask;
    }
}
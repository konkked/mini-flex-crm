using Microsoft.AspNetCore.Mvc.ModelBinding;
using MiniFlexCrmApi.Api.Models;

namespace MiniFlexCrmApi.Api.Middleware;

public class RequestContextModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {

        if (context.Metadata.ModelType == typeof(RequestContext) 
            && context.Metadata.ValidatorMetadata.OfType<FromRequestContextAttribute>().Any())
        {
            return new RequestContextModelBinder();
        }

        return default;
    }
}

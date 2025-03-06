using Microsoft.AspNetCore.Mvc.ModelBinding;
using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Context;

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

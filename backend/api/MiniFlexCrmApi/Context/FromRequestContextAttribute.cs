using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MiniFlexCrmApi.Context;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
public class FromRequestContextAttribute : Attribute, IBindingSourceMetadata
{
    public BindingSource BindingSource => BindingSource.Special;
}
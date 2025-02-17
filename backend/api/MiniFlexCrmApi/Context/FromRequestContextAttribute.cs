using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MiniFlexCrmApi.Api.Context;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
public class FromRequestContextAttribute : Attribute, IBindingSourceMetadata
{
    public BindingSource BindingSource => BindingSource.Special;
}
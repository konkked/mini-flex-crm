using System;
using System.Linq;
using System.Reflection;

namespace MiniFlexCrmApi.Util;

public static class Transpose
{
    public static void Push(object from, object to)
    {
        ArgumentNullException.ThrowIfNull(from);
        ArgumentNullException.ThrowIfNull(to);

        var fromProperties = from.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        var toProperties = to.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

        var matchingProperties = fromProperties
            .Join(toProperties,
                prop => prop.Name,
                prop => prop.Name,
                (fromProp, toProp) => new { fromProp , toProp }
                ,StringComparer.OrdinalIgnoreCase)
            .Where(x => x.fromProp is { CanWrite: true, CanRead: true }
                        && x.toProp is { CanWrite: true, CanRead: true }
                        && x.toProp.PropertyType.IsAssignableFrom(x.fromProp.PropertyType));

        foreach (var match in matchingProperties)
        {
            var currentValue = match.toProp.GetValue(from);
            var newValue = match.fromProp.GetValue(from);
            if(currentValue == null && newValue != null) 
                match.toProp.SetValue(to, newValue);
        }
    }

    public static void Pull(object to, object from) => Push(from, to);
}

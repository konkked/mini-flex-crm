using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniFlexCrmApi.Tests.Api.Services;

public static class Extensions
{
    public static async IAsyncEnumerable<T> AsAsyncEnumerable<T>(this IEnumerable<T> source)
    {
        await Task.CompletedTask;
        foreach(var element in source) yield return element;
    }
}
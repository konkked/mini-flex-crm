using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MiniFlexCrmApi.Context;
using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Security;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class AuthorizeRolesAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _roles;

    public AuthorizeRolesAttribute(params string[] roles)
    {
        _roles = roles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var requestContext = RequestContextExtractor.Extract(context.HttpContext);

        if (string.IsNullOrEmpty(requestContext.Role) || !_roles.Contains(requestContext.Role))
        {
            context.Result = new ForbidResult();
        }
    }
}
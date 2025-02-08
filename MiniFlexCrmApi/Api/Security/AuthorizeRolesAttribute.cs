using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MiniFlexCrmApi.Api.Models;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class AuthorizeRolesAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _roles;

    public AuthorizeRolesAttribute(params string[] roles)
    {
        _roles = roles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var requestContext = context.HttpContext.Items["RequestContext"] as RequestContext;

        if (requestContext == null || string.IsNullOrEmpty(requestContext.Role) || !_roles.Contains(requestContext.Role))
        {
            context.Result = new ForbidResult();
        }
    }
}
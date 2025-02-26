using System;

namespace MiniFlexCrmApi.Api.Auth;

public static class ServerCustomConstants
{
    public static class ClaimTypes
    {
        public const string Role = "role";
        public const string TenantId = "tenant_id";
    }

    public static readonly TimeSpan TokenLifetime = TimeSpan.FromMinutes(60);
    public const int RefreshWindowInMinutes = 5;
    public const int EmailVerificationWindowInMinutes = 15;
}
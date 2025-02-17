using System;

namespace MiniFlexCrmApi.Api.Auth;

public class AuthResponse
{
    public required string Token { get; set; }
    public DateTime Expiration { get; set; }
}
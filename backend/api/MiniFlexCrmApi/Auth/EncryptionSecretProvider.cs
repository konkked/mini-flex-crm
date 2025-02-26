using System;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MiniFlexCrmApi.Api.Auth;

public class EncryptionSecretProvider(ILogger<EncryptionSecretProvider> logger, IConfiguration configuration) : IEncryptionSecretProvider
{
    private const string EnvironmentVariableName = "MINIFLEXCRMAPI_JWT_KEY";
    private const string ConfigurationName = "JWT_SECRET";

    private readonly Lazy<string> _key = new(() =>
    {
        var env = configuration["ASPNETCORE_ENVIRONMENT"] ?? "Production"; // Default to Production
        var key = configuration[ConfigurationName] ?? Environment.GetEnvironmentVariable(EnvironmentVariableName);

        if (!string.IsNullOrEmpty(key)) 
            return key;

        if (!env.Equals("Development", StringComparison.OrdinalIgnoreCase) &&
            !env.Equals("Local", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(
                "❌ JWT Secret Key is missing in Production. Set environment variable MINIFLEXCRMAPI_JWT_KEY.");
        
        logger.LogWarning("⚠️ JWT Secret Key is missing. Generating a new key for local development.");
        return GenerateKey(logger);

    });

    public string GetSecret() => _key.Value;

    private static string GenerateKey(ILogger logger)
    {
        var keyBytes = RandomNumberGenerator.GetBytes(32);
        var base64Key = Convert.ToBase64String(keyBytes);

        logger.LogInformation("🔑 Generated a new JWT secret key.");
        Environment.SetEnvironmentVariable(EnvironmentVariableName, base64Key, EnvironmentVariableTarget.User);
        
        return base64Key;
    }
}
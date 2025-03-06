using StackExchange.Redis;

namespace MiniFlexCrmApi.Cache;

public static class RegisterCachingServices
{
    public static void AddThrottler(this IServiceCollection services, ICachingStrategyConfiguration? config = null)
    {
        config ??= new DefaultCachingStrategyConfiguration();
        services.AddSingleton(config);
        services.AddSingleton<IThrottlerCounterFactory, ThrottlerCounterFactory>();
        services.AddSingleton<IThrottlerCounter>(svc =>
            svc.GetService<IThrottlerCounterFactory>().Create());
        if (config.CacheType == CacheType.DistributedCache)
        {
            var redisConfiguration = config as IRedisCachingStrategyConfiguration;
            if (redisConfiguration == null)
                throw new InvalidOperationException("Unable to determine distributed cache configuration");
            services.AddSingleton<IConnectionMultiplexer>(svc =>
            {
                var options = new ConfigurationOptions
                {
                    EndPoints = { $"{redisConfiguration.ServerConfiguration.Host}:{redisConfiguration.ServerConfiguration.Port}" },
                    Password = string.IsNullOrEmpty(redisConfiguration.ServerConfiguration.Password) ? redisConfiguration.ServerConfiguration.Password : null,
                    Ssl = redisConfiguration.ServerConfiguration.UseTls, // True for AWS ElastiCache with TLS
                    AbortOnConnectFail = false,           // Retry if connection fails
                    ConnectTimeout = 5000,                // 5 seconds timeout
                    SyncTimeout = 5000
                };
                return ConnectionMultiplexer.Connect(options);
            });
            services.AddSingleton<ISlidingWindowCounter, RedisSlidingWindowCounter>();
        }
        else
        {
            services.AddSingleton<ISlidingWindowCounter, LocalSlidingWindowCounter>();
        }

    }
}
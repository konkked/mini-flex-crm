using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MiniFlexCrmApi.Db.Repos;

public static class RepositoryBootstrapper
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IRelationRepo, RelationRepo>();
        services.AddSingleton<IUserRepo,UserRepo>();
        services.AddSingleton<ICustomerRepo,CustomerRepo>();
        services.AddSingleton<ICompanyRepo, CompanyRepo>();
        services.AddSingleton<ITenantRepo, TenantRepo>();
        return services;
    }
}
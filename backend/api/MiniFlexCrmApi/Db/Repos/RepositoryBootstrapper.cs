namespace MiniFlexCrmApi.Db.Repos;

public static class RepositoryBootstrapper
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IRelationshipRepo, RelationshipRepo>();
        services.AddSingleton<IUserRepo,UserRepo>();
        services.AddSingleton<ICustomerRepo,CustomerRepo>();
        services.AddSingleton<ICompanyRepo, CompanyRepo>();
        services.AddSingleton<ITenantRepo, TenantRepo>();
        return services;
    }
}
using Microsoft.Extensions.DependencyInjection;

namespace MiniFlexCrmApi.Api.Services;

public static class ServicesBootstrapper
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddSingleton<IRelationService, RelationService>();
        services.AddSingleton<IUserService,UserService>();
        services.AddSingleton<ICompanyService,CompanyService>();
        services.AddSingleton<ICustomerService, CustomerService>();
        services.AddSingleton<ITenantService,TenantService>();
        return services;
    }
}
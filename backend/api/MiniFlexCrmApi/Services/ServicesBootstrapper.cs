namespace MiniFlexCrmApi.Services;

public static class ServicesBootstrapper
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddSingleton<IRelationshipService, RelationshipService>();
        services.AddSingleton<IUserService,UserService>();
        services.AddSingleton<ICompanyService,CompanyService>();
        services.AddSingleton<ICustomerService, CustomerService>();
        services.AddSingleton<ITenantService,TenantService>();
        services.AddSingleton<INotesService, NotesService>();
        services.AddSingleton<IAttachmentService, AttachmentService>();
        return services;
    }
}
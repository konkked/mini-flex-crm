namespace MiniFlexCrmApi.Db.Repos;

public static class RepositoryBootstrapper
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IRelationshipRepo, RelationshipRepo>();
        services.AddSingleton<IUserRepo, UserRepo>();
        services.AddSingleton<ICustomerRepo, CustomerRepo>();
        services.AddSingleton<ICompanyRepo, CompanyRepo>();
        services.AddSingleton<ITenantRepo, TenantRepo>();
        services.AddSingleton<IAttachmentRepo, AttachmentRepo>();
        services.AddSingleton<IContactRepo, ContactRepo>();
        services.AddSingleton<IEntityContactRepo, EntityContactRepo>();
        services.AddSingleton<IAddressRepo, AddressRepo>();
        services.AddSingleton<IEntityAddressRepo, EntityAddressRepo>();
        services.AddSingleton<ILeadRepo, LeadRepo>();
        services.AddSingleton<IProductRepo, ProductRepo>();
        services.AddSingleton<ISalesOpportunityRepo, SalesOpportunityRepo>();
        services.AddSingleton<ISaleRepo, SaleRepo>();
        services.AddSingleton<IPaymentRepo, PaymentRepo>();
        services.AddSingleton<IInteractionRepo, InteractionRepo>();
        services.AddSingleton<ISupportTicketRepo, SupportTicketRepo>();
        return services;
    }
}
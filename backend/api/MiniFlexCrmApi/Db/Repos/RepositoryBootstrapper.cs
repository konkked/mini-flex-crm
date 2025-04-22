namespace MiniFlexCrmApi.Db.Repos;

public static class RepositoryBootstrapper
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IRelationshipRepo, RelationshipRepo>();
        services.AddScoped<IUserRepo, UserRepo>();
        services.AddScoped<IAccountRepo, AccountRepo>();
        services.AddScoped<ICompanyRepo, CompanyRepo>();
        services.AddScoped<ITenantRepo, TenantRepo>();
        services.AddScoped<IAttachmentRepo, AttachmentRepo>();
        services.AddScoped<IContactRepo, ContactRepo>();
        services.AddScoped<IEntityContactRepo, EntityContactRepo>();
        services.AddScoped<IAddressRepo, AddressRepo>();
        services.AddScoped<IEntityAddressRepo, EntityAddressRepo>();
        services.AddScoped<ILeadRepo, LeadRepo>();
        services.AddScoped<IProductRepo, ProductRepo>();
        services.AddScoped<ISalesOpportunityRepo, SalesOpportunityRepo>();
        services.AddScoped<ISaleRepo, SaleRepo>();
        services.AddScoped<IPaymentRepo, PaymentRepo>();
        services.AddScoped<IInteractionRepo, InteractionRepo>();
        services.AddScoped<ISupportTicketRepo, SupportTicketRepo>();
        services.AddScoped<ITeamRepo, TeamRepo>();
        return services;
    }
}
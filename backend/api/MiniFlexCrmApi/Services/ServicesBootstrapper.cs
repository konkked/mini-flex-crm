namespace MiniFlexCrmApi.Services;

public static class ServicesBootstrapper
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddSingleton<IRelationshipService, RelationshipService>();
        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<ICompanyService, CompanyService>();
        services.AddSingleton<ICustomerService, CustomerService>();
        services.AddSingleton<ITenantService, TenantService>();
        services.AddSingleton<INotesService, NotesService>();
        services.AddSingleton<IAttachmentService, AttachmentService>();
        services.AddSingleton<IContactService, ContactService>();
        services.AddSingleton<IEntityContactService, EntityContactService>();
        services.AddSingleton<IAddressService, AddressService>();
        services.AddSingleton<IEntityAddressService, EntityAddressService>();
        services.AddSingleton<ILeadService, LeadService>();
        services.AddSingleton<IProductService, ProductService>();
        services.AddSingleton<ISalesOpportunityService, SalesOpportunityService>();
        services.AddSingleton<ISaleService, SaleService>();
        services.AddSingleton<IPaymentService, PaymentService>();
        services.AddSingleton<IInteractionService, InteractionService>();
        services.AddSingleton<ISupportTicketService, SupportTicketService>();
        return services;
    }
}
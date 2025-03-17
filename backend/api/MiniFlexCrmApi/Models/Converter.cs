using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Models;

public static class Converter
{
    public static Attachment From(AttachmentDbModel attachment) => new()
    {
        Id = attachment.Id,
        TenantId = attachment.TenantId,
        Ext = attachment.Ext,
        Notes = attachment.Notes,
        Path = attachment.Path,
        CreatedTs = attachment.CreatedTs,
        UpdatedTs = attachment.UpdatedTs,
        FileContent = attachment.FileContent,
    };
    
    public static AttachmentDbModel To(Attachment attachment) => new()
    {
        Id = attachment.Id,
        TenantId = attachment.TenantId,
        Ext = attachment.Ext,
        Notes = attachment.Notes,
        Path = attachment.Path,
        CreatedTs = attachment.CreatedTs,
        UpdatedTs = attachment.UpdatedTs,
        FileContent = attachment.FileContent,
    };
    
    public static UserModel From(UserDbModel userDbModel) => new()
    {
        Id = userDbModel.Id,
        Username = userDbModel.Username,
        Role = userDbModel.Role,
        Tenant = userDbModel.TenantName ?? string.Empty,
        TenantId = userDbModel.TenantId,
        Enabled = userDbModel.Enabled,
        Email = userDbModel.Email,
        Name = userDbModel.Name,
        Attributes = userDbModel.Attributes
    };

    public static UserDbModel To(UserModel model) => new()
    {
        Id = model.Id,
        Username = model.Username,
        Role = model.Role,
        TenantId = model.TenantId,
        Enabled = model.Enabled,
        Email = model.Email,
        Name = model.Name,
        Attributes = model.Attributes
    };

    public static CompanyModel From(CompanyDbModel companyDbModel) => new()
    {
        Id = companyDbModel.Id,
        Name = companyDbModel.Name,
        Tenant = companyDbModel.TenantName ?? string.Empty,
        TenantId = companyDbModel.TenantId,
        Attributes = companyDbModel.Attributes
    };

    public static CompanyDbModel To(CompanyModel model) => new()
    {
        Id = model.Id,
        Name = model.Name,
        TenantId = model.TenantId,
        Attributes =  model.Attributes
    };

    public static CustomerModel From(CustomerDbModel customerDbModel, Dictionary<string, dynamic?[]>? relationships) => new()
    {
        Id = customerDbModel.Id,
        Name = customerDbModel.Name,
        Tenant = customerDbModel.TenantName ?? string.Empty,
        TenantId = customerDbModel.TenantId,
        Attributes = customerDbModel.Attributes,
        Relationships = relationships
    };

    public static CustomerDbModel To(CustomerModel model) => new()
    {
        Id = model.Id,
        Name = model.Name,
        TenantId = model.TenantId,
        Attributes = model.Attributes
    };
    
    public static RelationshipModel From(RelationshipDbModel relationshipDbModel) => new()
    {
        Id = relationshipDbModel.Id,
        EntityId = relationshipDbModel.EntityId,
        EntityName = Enum.TryParse<EntityNameType>(relationshipDbModel.Entity, true, out var entityType) 
            ? entityType 
            : throw new ArgumentException($"Invalid entity type: {relationshipDbModel.Entity}"),
        CustomerId = relationshipDbModel.CustomerId,
        CustomerName = relationshipDbModel.CustomerName,
        Attributes = relationshipDbModel.Attributes
    };

    public static RelationshipDbModel To(RelationshipModel model) => new()
    {
        Id = model.Id,
        EntityId = model.EntityId,
        Entity = model.EntityName.ToString().ToLower(), // Convert Enum to lowercase string
        CustomerId = model.CustomerId,
        CustomerName = model.CustomerName,
        Attributes = model.Attributes
    };
    
    public static TenantModel From(TenantDbModel dbModel) => new()
    {
        Id = dbModel.Id, 
        Name = dbModel.Name,
        ShortId = dbModel.ShortId,
        Theme = dbModel.Theme,
        Attributes = dbModel.Attributes
    };
    
    public static TenantDbModel To(TenantModel model) => new()
    {
        Id = model.Id, 
        Name = model.Name,
        ShortId = model.ShortId,
        Theme = model.Theme,
        Attributes = model.Attributes
    };
    
    public static ContactModel From(ContactDbModel contactDbModel) => new()
    {
        Tenant = contactDbModel.TenantName,
        TenantId = contactDbModel.TenantId, 
        Id = contactDbModel.Id,
        Name = contactDbModel.Name,
        Title = contactDbModel.Title,
        Email = contactDbModel.Email,
        EmailVerified = contactDbModel.EmailVerified,
        Phone = contactDbModel.Phone,
        PhoneVerified = contactDbModel.PhoneVerified,
        CanText = contactDbModel.CanText,
        CanCall = contactDbModel.CanCall,
        CanEmail = contactDbModel.CanEmail,
        Attributes = contactDbModel.Attributes
    };

    public static ContactDbModel To(ContactModel model) => new()
    {
        Id = model.Id,
        Name = model.Name,
        Title = model.Title,
        Email = model.Email,
        EmailVerified = model.EmailVerified,
        Phone = model.Phone,
        PhoneVerified = model.PhoneVerified,
        CanText = model.CanText,
        CanCall = model.CanCall,
        CanEmail = model.CanEmail,
        Attributes = model.Attributes
    };

    public static EntityContactModel From(EntityContactDbModel entityContactDbModel) => new()
    {
        Id = entityContactDbModel.Id,
        SignificanceOrdinal = entityContactDbModel.SignificanceOrdinal,
        EntityName = entityContactDbModel.EntityName,
        EntityId = entityContactDbModel.EntityId,
        ContactId = entityContactDbModel.ContactId
    };

    public static EntityContactDbModel To(EntityContactModel model) => new()
    {
        Id = model.Id,
        SignificanceOrdinal = model.SignificanceOrdinal,
        EntityName = model.EntityName,
        EntityId = model.EntityId,
        ContactId = model.ContactId
    };

    public static AddressModel From(AddressDbModel addressDbModel) => new()
    {
        Id = addressDbModel.Id,
        Content = addressDbModel.Content,
        Lat = addressDbModel.Lat,
        Lng = addressDbModel.Lng,
        Attributes = addressDbModel.Attributes
    };

    public static AddressDbModel To(AddressModel model) => new()
    {
        Id = model.Id,
        Content = model.Content,
        Lat = model.Lat,
        Lng = model.Lng,
        Attributes = model.Attributes
    };

    public static EntityAddressModel From(EntityAddressDbModel entityAddressDbModel) => new()
    {
        Id = entityAddressDbModel.Id,
        SignificanceOrdinal = entityAddressDbModel.SignificanceOrdinal,
        EntityName = entityAddressDbModel.EntityName,
        EntityId = entityAddressDbModel.EntityId,
        AddressId = entityAddressDbModel.AddressId
    };

    public static EntityAddressDbModel To(EntityAddressModel model) => new()
    {
        Id = model.Id,
        SignificanceOrdinal = model.SignificanceOrdinal,
        EntityName = model.EntityName,
        EntityId = model.EntityId,
        AddressId = model.AddressId
    };

    public static LeadModel From(LeadDbModel leadDbModel) => new()
    {
        Id = leadDbModel.Id,
        Status = Enum.Parse<LeadStatusType>(leadDbModel.Status, true),
        LeadDataOrigin = Enum.Parse<LeadDataOriginType>(leadDbModel.LeadDataOrigin.Replace(" ", ""), true),
        Name = leadDbModel.Name,
        CompanyName = leadDbModel.CompanyName,
        Industry = leadDbModel.Industry,
        ApproximateCompanySize = leadDbModel.ApproximateCompanySize,
        ApproximateRevenue = leadDbModel.ApproximateRevenue,
        Email = leadDbModel.Email,
        Phone = leadDbModel.Phone,
        Attributes = leadDbModel.Attributes,
        Tenant = leadDbModel.TenantName ?? string.Empty,
        TenantId = leadDbModel.TenantId
    };

    public static LeadDbModel To(LeadModel model) => new()
    {
        Id = model.Id,
        Status = model.Status.ToString(),
        LeadDataOrigin = model.LeadDataOrigin.ToString(),
        Name = model.Name,
        CompanyName = model.CompanyName,
        Industry = model.Industry,
        ApproximateCompanySize = model.ApproximateCompanySize,
        ApproximateRevenue = model.ApproximateRevenue,
        Email = model.Email,
        Phone = model.Phone,
        Attributes = model.Attributes,
        TenantId = model.TenantId
    };

    public static ProductModel From(ProductDbModel productDbModel) => new()
    {
        Id = productDbModel.Id,
        Name = productDbModel.Name,
        SuggestedPrice = productDbModel.SuggestedPrice,
        TermMonths = productDbModel.TermMonths,
        Attributes = productDbModel.Attributes,
        Tenant = productDbModel.TenantName ?? string.Empty,
        TenantId = productDbModel.TenantId
    };

    public static ProductDbModel To(ProductModel model) => new()
    {
        Id = model.Id,
        Name = model.Name,
        SuggestedPrice = model.SuggestedPrice,
        TermMonths = model.TermMonths,
        Attributes = model.Attributes,
        TenantId = model.TenantId
    };

    public static SalesOpportunityModel From(SalesOpportunityDbModel model) => new()
    {
        Id = model.Id,
        Title = model.Title,
        Status = model.Status,
        Value = model.Value,
        Attributes = model.Attributes,
        CustomerId = model.CustomerId,
        LeadId = model.LeadId,
        Tenant = model.TenantName ?? string.Empty,
        TenantId = model.TenantId
    };

    public static SalesOpportunityDbModel To(SalesOpportunityModel model) => new()
    {
        Id = model.Id,
        Title = model.Title,
        Status = model.Status,
        Value = model.Value,
        Attributes = model.Attributes,
        CustomerId = model.CustomerId,
        LeadId = model.LeadId,
        TenantId = model.TenantId
    };

    public static SaleModel From(SaleDbModel model) => new()
    {
        Tenant = model.TenantName,
        TenantId = model.TenantId,
        Id = model.Id,
        Title = model.Title,
        Description = model.Description,
        Value = model.Value,
        TermMonths = model.TermMonths,
        Attributes = model.Attributes,
        SalesOpportunityId = model.SalesOpportunityId
    };

    public static SaleDbModel To(SaleModel model) => new()
    {
        TenantId = model.TenantId,
        TenantName = model.Tenant,
        Id = model.Id,
        Title = model.Title,
        Description = model.Description,
        Value = model.Value,
        TermMonths = model.TermMonths,
        Attributes = model.Attributes,
        SalesOpportunityId = model.SalesOpportunityId
    };

    public static PaymentModel From(PaymentDbModel model) => new()
    {
        Tenant = model.TenantName,
        TenantId = model.TenantId,
        Id = model.Id,
        Type = Enum.Parse<PaymentType>(model.Type, true),
        Title = model.Title,
        Value = model.Value,
        SaleId = model.SaleId,
        Attributes = model.Attributes
    };

    public static PaymentDbModel To(PaymentModel model) => new()
    {
        TenantId = model.TenantId,
        TenantName = model.Tenant,
        Id = model.Id,
        Type = model.Type.ToString(),
        Title = model.Title,
        Value = model.Value,
        SaleId = model.SaleId,
        Attributes = model.Attributes
    };

    public static InteractionModel From(InteractionDbModel interactionDbModel) => new()
    {
        Id = interactionDbModel.Id,
        Type = interactionDbModel.Type,
        InteractionDate = interactionDbModel.InteractionDate,
        Notes = interactionDbModel.Notes,
        Attributes = interactionDbModel.Attributes,
        CustomerId = interactionDbModel.CustomerId,
        ContactId = interactionDbModel.ContactId,
        LeadId = interactionDbModel.LeadId,
        Tenant = interactionDbModel.TenantName ?? string.Empty,
        TenantId = interactionDbModel.TenantId
    };

    public static InteractionDbModel To(InteractionModel model) => new()
    {
        Id = model.Id,
        Type = model.Type,
        InteractionDate = model.InteractionDate,
        Notes = model.Notes,
        Attributes = model.Attributes,
        CustomerId = model.CustomerId,
        ContactId = model.ContactId,
        LeadId = model.LeadId,
        TenantId = model.TenantId
    };

    public static SupportTicketModel From(SupportTicketDbModel supportTicketDbModel) => new()
    {
        Id = supportTicketDbModel.Id,
        Issue = supportTicketDbModel.Issue,
        Status = Enum.Parse<SupportTicketStatusType>(supportTicketDbModel.Status, true),
        Attributes = supportTicketDbModel.Attributes,
        UserId = supportTicketDbModel.UserId,
        Tenant = supportTicketDbModel.TenantName ?? string.Empty,
        TenantId = supportTicketDbModel.TenantId
    };

    public static SupportTicketDbModel To(SupportTicketModel model) => new()
    {
        Id = model.Id,
        Issue = model.Issue,
        Status = model.Status.ToString(),
        Attributes = model.Attributes,
        UserId = model.UserId,
        TenantId = model.TenantId
    };

    public static EntityContactDbModel ToLink(ContactModel contact) => new()
    {
        EntityId = contact.EntityId ?? 0,
        EntityName = contact.EntityName,
        ContactId = contact.Id,
        SignificanceOrdinal = contact.SignificanceOrdinal ?? 1
    };
    
    public static EntityAddressDbModel ToLink(AddressModel address) => new()
    {
        EntityId = address.EntityId ?? 0,
        EntityName = address.EntityName,
        AddressId = address.Id,
        SignificanceOrdinal = address.SignificanceOrdinal ?? 1
    };
}

using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Models;

public static class Converter
{
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
}

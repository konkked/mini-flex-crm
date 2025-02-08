using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Api.Models;

public static class Converter
{
    public static UserModel From(UserDbModel userDbModel) => new()
    {
        Id = userDbModel.Id,
        Username = userDbModel.Username,
        Role = userDbModel.Role,
        Tenant = userDbModel.TenantName,
        TenantId = userDbModel.TenantId,
        Enabled = userDbModel.Enabled
    };

    public static UserDbModel To(UserModel model) => new()
    {
        Id = model.Id,
        Username = model.Username,
        Role = model.Role,
        TenantId = model.TenantId,
        Enabled = model.Enabled
    };

    public static CompanyModel From(CompanyDbModel companyDbModel) => new()
    {
        Id = companyDbModel.Id,
        Name = companyDbModel.Name,
        Tenant = companyDbModel.TenantName,
        TenantId = companyDbModel.TenantId
    };

    public static CompanyDbModel To(CompanyModel model) => new()
    {
        Id = model.Id,
        Name = model.Name,
        TenantId = model.TenantId
    };

    public static CustomerModel From(CustomerDbModel customerDbModel, Dictionary<string, dynamic[]>? relations) => new()
    {
        Id = customerDbModel.Id,
        Name = customerDbModel.Name,
        Tenant = customerDbModel.TenantName,
        TenantId = customerDbModel.TenantId,
        Attributes = customerDbModel.Attributes,
        Relationships = relations
    };

    public static CustomerDbModel To(CustomerModel model) => new()
    {
        Id = model.Id,
        Name = model.Name,
        TenantId = model.TenantId,
        Attributes = model.Attributes
    };

    public static RelationModel From(RelationDbModel relationDbModel) => new()
    {
        Id = relationDbModel.Id,
        EntityId = relationDbModel.EntityId,
        EntityName = relationDbModel.Entity,
        CustomerId = relationDbModel.CustomerId,
        CustomerName = relationDbModel.CustomerName
    };

    public static RelationDbModel To(RelationModel model) => new()
    {
        Id = model.Id,
        EntityId = model.EntityId,
        Entity = model.EntityName,
        CustomerId = model.CustomerId,
        CustomerName = model.CustomerName
    };
    
    public static TenantModel From(TenantDbModel tenantDbModel) => new(){ Id = tenantDbModel.Id, Name = tenantDbModel.Name };
    public static TenantDbModel To(TenantModel model) => new()  { Id = model.Id, Name = model.Name };
}

using System.Collections.Generic;
using System.Text.Json;
using MiniFlexCrmApi.Models;
using MiniFlexCrmApi.Serialization;
using NUnit.Framework;

namespace MiniFlexCrmApi.Tests.Api.Models;

[TestFixture]
public class ConverterSerializationTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.General)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new AttributesJsonConverter(), new RelationshipsJsonConverter() },
        WriteIndented = false
    };

    //  Test: UserModel JSON Serialization
    [Test]
    public void Serialize_UserModel_ToJson()
    {
        var model = new UserModel
        {
            Id = 1,
            Username = "testuser",
            Role = "admin",
            Tenant = "TestTenant",
            TenantId = 100,
            Enabled = true
        };
        
        var deserialized = JsonSerializer.Deserialize<UserModel>(
            JsonSerializer.Serialize(model, JsonOptions), JsonOptions);

        Assert.That(model, Is.EqualTo(deserialized));
    }

    //  Test: UserModel JSON Deserialization
    [Test]
    public void Deserialize_UserModel_FromJson()
    {
        var json = @"{""id"":1,""username"":""testuser"",""role"":""admin"",""tenant"":""TestTenant"",""tenantId"":100,""enabled"":true}";
        var model = JsonSerializer.Deserialize<UserModel>(json, JsonOptions);

        Assert.That(model, Is.Not.Null);
        Assert.That(model!.Id, Is.EqualTo(1));
        Assert.That(model.Username, Is.EqualTo("testuser"));
        Assert.That(model.Role, Is.EqualTo("admin"));
        Assert.That(model.Tenant, Is.EqualTo("TestTenant"));
        Assert.That(model.TenantId, Is.EqualTo(100));
        Assert.That(model.Enabled, Is.EqualTo(true));
    }

    //  Test: CompanyModel JSON Serialization
    [Test]
    public void Serialize_CompanyModel_ToJson()
    {
        var model = new CompanyModel
        {
            Id = 2,
            Name = "TestCompany",
            Tenant = "CompanyTenant",
            TenantId = 200
        };

        var deserialized = JsonSerializer.Deserialize<CompanyModel>(
            JsonSerializer.Serialize(model, JsonOptions), JsonOptions);

        Assert.That(model, Is.EqualTo(deserialized));
    }

    //  Test: CompanyModel JSON Deserialization
    [Test]
    public void Deserialize_CompanyModel_FromJson()
    {
        var json = @"{""id"":2,""name"":""TestCompany"",""tenant"":""CompanyTenant"",""tenantId"":200}";
        var model = JsonSerializer.Deserialize<CompanyModel>(json, JsonOptions);

        Assert.That(model, Is.Not.Null);
        Assert.That(model!.Id, Is.EqualTo(2));
        Assert.That(model.Name, Is.EqualTo("TestCompany"));
        Assert.That(model.Tenant, Is.EqualTo("CompanyTenant"));
        Assert.That(model.TenantId, Is.EqualTo(200));
    }

    //  Test: AccountModel JSON Serialization
    [Test]
    public void Serialize_AccountModel_ToJson()
    {
        var model = new AccountModel
        {
            Id = 3,
            Name = "TestAccount",
            Tenant = "AccountTenant",
            TenantId = 300,
            Attributes = new { key = "value" },
            Relationships = new Dictionary<string, dynamic[]>
            {
                { "company", [new { id = 101, name = "Company A" }] },
                { "lead", [new { id = 101, name = "Company A" }] },
                { "contact", [new { id = 101, name = "Company A", title="Director of Sales" }] }
            }
        };
        var json = JsonSerializer.Serialize(model, JsonOptions);

        var deserialized = JsonSerializer.Deserialize<AccountModel>(
            json, JsonOptions);

        Assert.That(new{model.Name,model.TenantId,model.Id}, 
            Is.EqualTo(new{deserialized.Name,deserialized.TenantId,deserialized.Id}));
        Assert.That(deserialized.Attributes.key, Is.EqualTo(model.Attributes.key));
        Assert.That(deserialized.Relationships.Count, Is.EqualTo(model.Relationships.Count));
        Assert.That(deserialized.Relationships["company"][0].id, Is.EqualTo(model.Relationships["company"][0].id));
        Assert.That(deserialized.Relationships["company"][0].name, Is.EqualTo(model.Relationships["company"][0].name));
    }

    //  Test: AccountModel JSON Deserialization
    [Test]
    public void Deserialize_AccountModel_FromJson()
    {
        var json = @"{""id"":3,""name"":""TestAccount"",""tenant"":""AccountTenant"",""tenantId"":300,""attributes"":{""key"":""value""},""relationships"":{""company"":[{""id"":101,""name"":""Company A""}]}}";
        var model = JsonSerializer.Deserialize<AccountModel>(json, JsonOptions);

        Assert.That(model, Is.Not.Null);
        Assert.That(model!.Id, Is.EqualTo(3));
        Assert.That(model.Name, Is.EqualTo("TestAccount"));
        Assert.That(model.Tenant, Is.EqualTo("AccountTenant"));
        Assert.That(model.TenantId, Is.EqualTo(300));
        Assert.That(model.Attributes, Is.Not.Null);
        Assert.That(model.Attributes?.key, Is.EqualTo("value"));
        Assert.That(model.Relationships?.ContainsKey("company"), Is.True);
        Assert.That(model.Relationships?["company"].Length, Is.EqualTo(1));
        Assert.That(model.Relationships?["company"][0].id, Is.EqualTo(101));
        Assert.That(model.Relationships?["company"][0].name, Is.EqualTo("Company A"));
        
    }

    //  Test: RelationModel JSON Serialization
    [Test]
    public void Serialize_RelationModel_ToJson()
    {
        var model = new RelationshipModel
        {
            Id = 4,
            EntityId = 500,
            EntityName = EntityNameType.company,
            AccountId = 600,
            AccountName = "Account X"
        };
        
        var deserialized = JsonSerializer.Deserialize<RelationshipModel>(
            JsonSerializer.Serialize(model, JsonOptions), JsonOptions);

        Assert.That(model, Is.EqualTo(deserialized));
    }

    //  Test: RelationModel JSON Deserialization
    [Test]
    public void Deserialize_RelationModel_FromJson()
    {
        var json = @"{""id"":4,""entityId"":500,""entityName"":""company"",""accountId"":600,""accountName"":""Account X""}";
        var model = JsonSerializer.Deserialize<RelationshipModel>(json, JsonOptions);

        Assert.That(model, Is.Not.Null);
        Assert.That(model!.Id, Is.EqualTo(4));
        Assert.That(model.EntityId, Is.EqualTo(500));
        Assert.That(model.EntityName, Is.EqualTo(EntityNameType.company));
        Assert.That(model.AccountId, Is.EqualTo(600));
        Assert.That(model.AccountName, Is.EqualTo("Account X"));
    }
}
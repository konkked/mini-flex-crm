using System.Collections.Generic;
using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Models;
using NUnit.Framework;

namespace MiniFlexCrmApi.Tests.Api.Models;

[TestFixture]
public class ConverterTests
{
    //  Test: Convert UserDbModel to UserModel
    [Test]
    public void Convert_UserDbModel_To_UserModel()
    {
        var dbModel = new UserDbModel
        {
            Id = 1,
            Username = "testuser",
            Role = "Admin",
            TenantName = "TestTenant",
            TenantId = 100,
            Enabled = true
        };

        var result = Converter.From(dbModel);

        Assert.That(result.Id, Is.EqualTo(dbModel.Id));
        Assert.That(result.Username, Is.EqualTo(dbModel.Username));
        Assert.That(result.Role, Is.EqualTo(dbModel.Role));
        Assert.That(result.Tenant, Is.EqualTo(dbModel.TenantName));
        Assert.That(result.TenantId, Is.EqualTo(dbModel.TenantId));
        Assert.That(result.Enabled, Is.EqualTo(dbModel.Enabled));
    }

    //  Test: Convert UserModel to UserDbModel
    [Test]
    public void Convert_UserModel_To_UserDbModel()
    {
        var model = new UserModel
        {
            Id = 1,
            Username = "testuser",
            Role = "Admin",
            Tenant = "TestTenant",
            TenantId = 100,
            Enabled = true
        };

        var result = Converter.To(model);

        Assert.That(result.Id, Is.EqualTo(model.Id));
        Assert.That(result.Username, Is.EqualTo(model.Username));
        Assert.That(result.Role, Is.EqualTo(model.Role));
        Assert.That(result.TenantId, Is.EqualTo(model.TenantId));
        Assert.That(result.Enabled, Is.EqualTo(model.Enabled));
    }

    //  Test: Convert CompanyDbModel to CompanyModel
    [Test]
    public void Convert_CompanyDbModel_To_CompanyModel()
    {
        var dbModel = new CompanyDbModel
        {
            Id = 1,
            Name = "TestCompany",
            TenantName = "TestTenant",
            TenantId = 100
        };

        var result = Converter.From(dbModel);

        Assert.That(result.Id, Is.EqualTo(dbModel.Id));
        Assert.That(result.Name, Is.EqualTo(dbModel.Name));
        Assert.That(result.Tenant, Is.EqualTo(dbModel.TenantName));
        Assert.That(result.TenantId, Is.EqualTo(dbModel.TenantId));
    }

    //  Test: Convert CompanyModel to CompanyDbModel
    [Test]
    public void Convert_CompanyModel_To_CompanyDbModel()
    {
        var model = new CompanyModel
        {
            Id = 1,
            Name = "TestCompany",
            Tenant = "TestTenant",
            TenantId = 100
        };

        var result = Converter.To(model);

        Assert.That(result.Id, Is.EqualTo(model.Id));
        Assert.That(result.Name, Is.EqualTo(model.Name));
        Assert.That(result.TenantId, Is.EqualTo(model.TenantId));
    }

    //  Test: Convert AccountDbModel to AccountModel
    [Test]
    public void Convert_AccountDbModel_To_AccountModel()
    {
        var dbModel = new AccountDbModel
        {
            Id = 1,
            Name = "TestAccount",
            TenantName = "TestTenant",
            TenantId = 100,
            Attributes = new { Age = 30, Location = "NY" }
        };

        var relations = new Dictionary<string, dynamic[]>
        {
            { "company", [new { Id = 10, Name = "CompanyA" }] }
        };

        var result = Converter.From(dbModel, relations);

        Assert.That(result.Id, Is.EqualTo(dbModel.Id));
        Assert.That(result.Name, Is.EqualTo(dbModel.Name));
        Assert.That(result.Tenant, Is.EqualTo(dbModel.TenantName));
        Assert.That(result.TenantId, Is.EqualTo(dbModel.TenantId));
        Assert.That(result.Attributes, Is.EqualTo(dbModel.Attributes));
        Assert.That(result.Relationships, Is.EqualTo(relations));
    }

    //  Test: Convert AccountModel to AccountDbModel
    [Test]
    public void Convert_AccountModel_To_AccountDbModel()
    {
        var model = new AccountModel
        {
            Id = 1,
            Name = "TestAccount",
            Tenant = "TestTenant",
            TenantId = 100,
            Attributes = new { Age = 30, Location = "NY" }
        };

        var result = Converter.To(model);

        Assert.That(result.Id, Is.EqualTo(model.Id));
        Assert.That(result.Name, Is.EqualTo(model.Name));
        Assert.That(result.TenantId, Is.EqualTo(model.TenantId));
        Assert.That(result.Attributes, Is.EqualTo(model.Attributes));
    }

    //  Test: Convert RelationDbModel to RelationModel
    [Test]
    public void Convert_RelationDbModel_To_RelationModel()
    {
        var dbModel = new RelationshipDbModel
        {
            Id = 1,
            EntityId = 42,
            Entity = "company",
            AccountId = 2,
            AccountName = "John Doe"
        };

        var result = Converter.From(dbModel);

        Assert.That(result.Id, Is.EqualTo(dbModel.Id));
        Assert.That(result.EntityId, Is.EqualTo(dbModel.EntityId));
        Assert.That(result.EntityName.ToString().ToLower(), Is.EqualTo(dbModel.Entity));
        Assert.That(result.AccountId, Is.EqualTo(dbModel.AccountId));
        Assert.That(result.AccountName, Is.EqualTo(dbModel.AccountName));
    }

    //  Test: Convert RelationModel to RelationDbModel
    [Test]
    public void Convert_RelationModel_To_RelationDbModel()
    {
        var model = new RelationshipModel
        {
            Id = 1,
            EntityId = 42,
            EntityName = EntityNameType.company,
            AccountId = 2,
            AccountName = "John Doe"
        };

        var result = Converter.To(model);

        Assert.That(result.Id, Is.EqualTo(model.Id));
        Assert.That(result.EntityId, Is.EqualTo(model.EntityId));
        Assert.That(result.Entity, Is.EqualTo(model.EntityName.ToString().ToLower()));
        Assert.That(result.AccountId, Is.EqualTo(model.AccountId));
        Assert.That(result.AccountName, Is.EqualTo(model.AccountName));
    }

    //  Test: Convert TenantDbModel to TenantModel
    [Test]
    public void Convert_TenantDbModel_To_TenantModel()
    {
        var dbModel = new TenantDbModel { Id = 1, Name = "TestTenant" };

        var result = Converter.From(dbModel);

        Assert.That(result.Id, Is.EqualTo(dbModel.Id));
        Assert.That(result.Name, Is.EqualTo(dbModel.Name));
    }

    //  Test: Convert TenantModel to TenantDbModel
    [Test]
    public void Convert_TenantModel_To_TenantDbModel()
    {
        var model = new TenantModel { Id = 1, Name = "TestTenant" };

        var result = Converter.To(model);

        Assert.That(result.Id, Is.EqualTo(model.Id));
        Assert.That(result.Name, Is.EqualTo(model.Name));
    }
}
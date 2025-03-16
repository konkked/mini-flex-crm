using MiniFlexCrmApi.Db.Repos;
using MiniFlexCrmApi.Services;

namespace MiniFlexCrmApi.Tests.Api.Services;

public class TestService(IRepo<TestDbModel> repo) : BaseService<TestDbModel, TestApiModel>(repo)
{
    protected override TestDbModel ApiModelToDbModel(TestApiModel model) => model != null
        ? new()
        {
            Id = model.Id,
            Name = model.Name
        }
        : null;

    protected override TestApiModel DbModelToApiModel(TestDbModel model) => model != null
        ? new()
        {
            Id = model.Id,
            Name = model.Name
        }
        : null;
}
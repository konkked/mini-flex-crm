using MiniFlexCrmApi.Db.Repos;
using MiniFlexCrmApi.Services;

namespace MiniFlexCrmApi.Tests.Api.Services;

public class TestService(IRepo<TestDbModel> repo) : BaseService<TestDbModel, TestApiModel>(repo)
{
    protected override TestDbModel ConvertToDbModel(TestApiModel model) => model != null
        ? new()
        {
            Id = model.Id,
            Name = model.Name
        }
        : null;

    protected override TestApiModel ConvertToApiModel(TestDbModel model) => model != null
        ? new()
        {
            Id = model.Id,
            Name = model.Name
        }
        : null;
}
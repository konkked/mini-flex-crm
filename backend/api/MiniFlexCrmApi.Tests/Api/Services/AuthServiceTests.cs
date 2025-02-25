using System.Threading.Tasks;
using MiniFlexCrmApi.Api.Auth;
using MiniFlexCrmApi.Db.Repos;
using Moq;
using NUnit.Framework;

namespace MiniFlexCrmApi.Tests.Api.Services;

[TestFixture]
public class AuthServiceTests
{
    public AuthServiceTests()
    {
    }

    [Test]
    public async Task SignUp_User()
    {
        var userRepo = Mock.Of<IUserRepo>();
        var jwtService = Mock.Of<IJwtService>();
        var authService = new AuthService(userRepo, jwtService);
        Mock.Get(userRepo)
            .Setup(a=>a.ExistsByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync(false);
        await authService.SignUpAsync(new ()
            { Password = "Aly&ndWurfy#7", TenantId = 0, Username = "super_admin" });
    }
}
using System.Threading.Tasks;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using MiniFlexCrmApi.Auth;
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
        var tenantRepo = Mock.Of<ITenantRepo>();
        var jwtService = Mock.Of<IJwtService>();
        var emailSender = Mock.Of<IEmailSender>();
        var endecryptor = Mock.Of<IEndecryptor>();
        var consoleLogger = Mock.Of<ILogger<AuthService>>();
        var authService = new AuthService(consoleLogger, userRepo, tenantRepo, jwtService, emailSender, endecryptor);
        Mock.Get(userRepo)
            .Setup(a=>a.ExistsByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync(false);
        await authService.SignUpAsync(new ()
            { Password = "Aly&ndWurfy#7", TenantId = 0, Username = "super_admin" });
    }
}
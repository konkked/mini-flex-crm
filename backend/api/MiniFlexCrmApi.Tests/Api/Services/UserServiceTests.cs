using System.Threading.Tasks;
using MiniFlexCrmApi.Api.Models;
using MiniFlexCrmApi.Api.Services;
using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;
using NUnit.Framework;
using Moq;

namespace MiniFlexCrmApi.Tests.Api.Services;

[TestFixture]
public class UserServiceTests
{
    private Mock<IUserRepo> _mockUserRepo;
    private UserService _userService;

    [SetUp]
    public void SetUp()
    {
        _mockUserRepo = new Mock<IUserRepo>();
        _userService = new UserService(_mockUserRepo.Object);
    }

    //  Test: Create User
    [Test]
    public async Task CreateItem_ShouldReturnTrue_WhenUserIsCreated()
    {
        // Arrange
        var userModel = new UserModel { Id = 1, Username = "testuser", TenantId = 1, Role = "User" };

        _mockUserRepo.Setup(repo => repo.CreateAsync(It.IsAny<UserDbModel>()))
            .ReturnsAsync(1);

        // Act
        var result = await _userService.CreateItem(userModel);

        // Assert
        Assert.That(result, Is.True);
        _mockUserRepo.Verify(repo => repo.CreateAsync(It.Is<UserDbModel>(u => u.Username == userModel.Username)), Times.Once);
    }

    //  Test: Delete User
    [Test]
    public async Task DeleteItem_ShouldReturnTrue_WhenUserIsDeleted()
    {
        // Arrange
        _mockUserRepo.Setup(repo => repo.DeleteAsync(1)).ReturnsAsync(1);

        // Act
        var result = await _userService.DeleteItem(1);

        // Assert
        Assert.That(result,Is.True);
        _mockUserRepo.Verify(repo => repo.DeleteAsync(1), Times.Once);
    }

    //  Test: Update User
    [Test]
    public async Task UpdateItem_ShouldReturnTrue_WhenUserIsUpdated()
    {
        // Arrange
        var userModel = new UserModel { Id = 1, Username = "updateduser", TenantId = 1, Role = "Admin" };
        var userDbModel = Converter.To(userModel);

        _mockUserRepo.Setup(repo => repo.FindAsync(1)).ReturnsAsync(userDbModel);
        _mockUserRepo.Setup(repo => repo.UpdateAsync(It.IsAny<UserDbModel>())).ReturnsAsync(1);

        // Act
        var result = await _userService.UpdateItem(userModel);

        // Assert
        Assert.That(result, Is.True);
        _mockUserRepo.Verify(repo => repo.UpdateAsync(It.IsAny<UserDbModel>()), Times.Once);
    }

    //  Test: Enable User
    [Test]
    public async Task TryEnableUserAsync_ShouldReturnTrue_WhenUserIsDisabled()
    {
        // Arrange
        var user = new UserDbModel { Id = 1, TenantId = 1, Enabled = false };
        _mockUserRepo.Setup(repo => repo.FindAsync(1)).ReturnsAsync(user);
        _mockUserRepo.Setup(repo => repo.UpdateAsync(It.IsAny<UserDbModel>())).ReturnsAsync(1);

        // Act
        var result = await _userService.TryEnableUserAsync(1, 1);

        // Assert
        Assert.That(result,Is.True);
        _mockUserRepo.Verify(
            repo  => repo.UpdateAsync(It.Is<UserDbModel>(u => u.Enabled == true)), 
            Times.Once);
    }

    //  Test: Disable User
    [Test]
    public async Task TryDisableUserAsync_ShouldReturnTrue_WhenUserIsEnabled()
    {
        // Arrange
        var user = new UserDbModel { Id = 1, TenantId = 1, Enabled = true };
        _mockUserRepo.Setup(repo => repo.FindAsync(1)).ReturnsAsync(user);
        _mockUserRepo.Setup(repo => repo.UpdateAsync(It.IsAny<UserDbModel>())).ReturnsAsync(1);

        // Act
        var result = await _userService.TryDisableUserAsync(1, 1);

        // Assert
        Assert.That(result,Is.True);
        _mockUserRepo.Verify(repo => 
            repo.UpdateAsync(It.Is<UserDbModel>(u => u.Enabled == false)), 
            Times.Once);
    }

    //  Test: Tenant Restriction on Enabling
    [Test]
    public async Task TryEnableUserAsync_ShouldReturnFalse_WhenCallerTenantDoesNotMatch()
    {
        // Arrange
        var user = new UserDbModel { Id = 1, TenantId = 2, Enabled = false };
        _mockUserRepo.Setup(repo => repo.FindAsync(1)).ReturnsAsync(user);

        // Act
        var result = await _userService.TryEnableUserAsync(1, 1); // Caller is tenant 1, but user belongs to tenant 2

        // Assert
        Assert.That(result, Is.False);
        _mockUserRepo.Verify(repo => repo.UpdateAsync(It.IsAny<UserDbModel>()), Times.Never);
    }

    //  Test: Tenant Restriction on Disabling
    [Test]
    public async Task TryDisableUserAsync_ShouldReturnFalse_WhenCallerTenantDoesNotMatch()
    {
        // Arrange
        var user = new UserDbModel { Id = 1, TenantId = 2, Enabled = true };
        _mockUserRepo.Setup(repo => repo.FindAsync(1)).ReturnsAsync(user);

        // Act
        var result = await _userService.TryDisableUserAsync(1, 1); // Caller is tenant 1, but user belongs to tenant 2

        // Assert
        Assert.That(result, Is.False);
        _mockUserRepo.Verify(repo => repo.UpdateAsync(It.IsAny<UserDbModel>()), Times.Never);
    }

    // Test: Return False If User Not Found
    [Test]
    public async Task TryEnableUserAsync_ShouldReturnFalse_WhenUserNotFound()
    {
        // Arrange
        _mockUserRepo.Setup(repo => repo.FindAsync(1)).ReturnsAsync((UserDbModel)null);

        // Act
        var result = await _userService.TryEnableUserAsync(1, 1);

        // Assert
        Assert.That(result, Is.False);
    }
}
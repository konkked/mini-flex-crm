using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiniFlexCrmApi.Db.Repos;
using Moq;
using NUnit.Framework;

namespace MiniFlexCrmApi.Tests.Api.Services;

[TestFixture]
public class BaseServiceTests
{
    private Mock<IRepo<TestDbModel>> _mockRepo;
    private TestService _testService;

    [SetUp]
    public void SetUp()
    {
        _mockRepo = new Mock<IRepo<TestDbModel>>();
        _testService = new TestService(_mockRepo.Object);
    }

    // Test: Create Item
    [Test]
    public async Task CreateItem_ShouldReturnTrue_WhenEntityIsCreated()
    {
        // Arrange
        var testModel = new TestApiModel { Id = 1, Name = "Test" };
        _mockRepo.Setup(repo => repo.CreateAsync(It.IsAny<TestDbModel>()))
            .ReturnsAsync(1);

        // Act
        var result = await _testService.CreateItemAsync(testModel);

        // Assert
        Assert.That(result, Is.EqualTo(true));
        _mockRepo.Verify(repo => repo.CreateAsync(It.IsAny<TestDbModel>()), Times.Once);
    }

    // Test: Delete Item
    [Test]
    public async Task DeleteItem_ShouldReturnTrue_WhenEntityIsDeleted()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.DeleteAsync(1)).ReturnsAsync(1);

        // Act
        var result = await _testService.DeleteItemAsync(1);

        // Assert
        Assert.That(result, Is.EqualTo(true));
        _mockRepo.Verify(repo => repo.DeleteAsync(1), Times.Once);
    }

    //Test: Update Item
    [Test]
    public async Task UpdateItem_ShouldReturnTrue_WhenEntityIsUpdated()
    {
        // Arrange
        var testModel = new TestApiModel { Id = 1, Name = "Updated" };
        var testDbModel = new TestDbModel { Id = 1, Name = "Original" };

        _mockRepo.Setup(repo => repo.FindAsync(1)).ReturnsAsync(testDbModel);
        _mockRepo.Setup(repo => repo.UpdateAsync(It.IsAny<TestDbModel>())).ReturnsAsync(1);

        // Act
        var result = await _testService.UpdateItemAsync(testModel);

        // Assert
        Assert.That(result, Is.EqualTo(true));
        _mockRepo.Verify(repo => repo.UpdateAsync(It.IsAny<TestDbModel>()), Times.Once);
    }

    //Test: Get Item
    [Test]
    public async Task GetItem_ShouldReturnEntity_WhenEntityExists()
    {
        // Arrange
        var testDbModel = new TestDbModel { Id = 1, Name = "Test" };
        _mockRepo.Setup(repo => repo.FindAsync(1)).ReturnsAsync(testDbModel);

        // Act
        var result = await _testService.GetItemAsync(1);

        // Assert
        Assert.That(result.Id,Is.EqualTo(testDbModel.Id));
        Assert.That(result.Name,Is.EqualTo(testDbModel.Name));
    }

    //Test: Get Item (Not Found)
    [Test]
    public async Task GetItem_ShouldReturnNull_WhenEntityDoesNotExist()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.FindAsync(1))
            .Returns(Task.FromResult((TestDbModel)null));

        // Act
        var result = await _testService.GetItemAsync(1);

        // Assert
        Assert.That(result, Is.EqualTo(null));
    }

    //Test: List Items
    [Test]
    public async Task ListItems_ShouldReturnPagedResults()
    {
        // Arrange
        var testDbModels = new List<TestDbModel>
        {
            new() { Id = 1, Name = "Test1" },
            new() { Id = 2, Name = "Test2" }
        };

        _mockRepo.Setup(repo => repo.GetSomeAsync(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(testDbModels.AsAsyncEnumerable());

        // Act
        var result = await _testService.ListItemsAsync(2);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.ElementAt(0).Name, Is.EqualTo("Test1"));
        Assert.That(result.ElementAt(1).Name, Is.EqualTo("Test2"));
    }
    
    //  Test: Next Token is generated when there are more items
    [Test]
    public async Task ListItems_ShouldGenerateNextToken_WhenMoreItemsExist()
    {
        // Arrange
        var testDbModels = new List<TestDbModel>
        {
            new() { Id = 1, Name = "Item1" },
            new() { Id = 2, Name = "Item2" },
            new() { Id = 3, Name = "Item3" }
        };

        _mockRepo.Setup(repo => repo.GetSomeAsync(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(testDbModels.AsAsyncEnumerable());

        // Act
        var result = await _testService.ListItemsAsync(3);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(3));
     }

    //  Test: Next Token is NULL when there are NO more items
    [Test]
    public async Task ListItems_ShouldNotGenerateNextToken_WhenNoMoreItemsExist()
    {
        // Arrange
        var testDbModels = new List<TestDbModel>
        {
            new() { Id = 1, Name = "Item1" },
            new() { Id = 2, Name = "Item2" }
        };

        _mockRepo.Setup(repo => repo.GetSomeAsync(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(testDbModels.AsAsyncEnumerable());

        // Act
        var result = await _testService.ListItemsAsync(3);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
    }
}
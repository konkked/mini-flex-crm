using System;
using System.Text.Json;
using MiniFlexCrmApi.Api.Services;
using NUnit.Framework;

namespace MiniFlexCrmApi.Tests.Api.Services;

[TestFixture]
public class NextTokenTests
{
    [Test]
    public void Serialize_ShouldEncodeNextTokenCorrectly()
    {
        // Arrange
        var nextToken = new CursorTokenModel { Id = 100, PageSize = 50 };

        // Act
        var encoded = Base62JsonConverter.Serialize(nextToken);

        // Assert
        Assert.That(string.IsNullOrEmpty(encoded), Is.False);
    }

    [Test]
    public void Deserialize_ShouldDecodeNextTokenCorrectly()
    {
        // Arrange
        var originalToken = new CursorTokenModel { Id = 100, PageSize = 50 };
        var encoded = Base62JsonConverter.Serialize(originalToken);

        // Act
        var decoded = Base62JsonConverter.Deserialize<CursorTokenModel>(encoded);

        // Assert
        Assert.That(decoded.Id, Is.EqualTo(originalToken.Id));
        Assert.That(decoded.PageSize, Is.EqualTo(originalToken.PageSize));
    }

    [Test]
    public void Deserialize_ShouldThrowExceptionOnInvalidBase62String()
    {
        // Arrange
        var invalidBase62 = "InvalidString@@!!";

        // Act & Assert
        Assert.Throws<JsonException>(() =>
            Base62JsonConverter.Deserialize<CursorTokenModel>(invalidBase62));
    }

    [Test]
    public void SerializeAndDeserialize_ShouldMaintainDataIntegrity()
    {
        // Arrange
        var originalToken = new CursorTokenModel { Id = 250, PageSize = 100 };

        // Act
        var encoded = Base62JsonConverter.Serialize(originalToken);
        var decoded = Base62JsonConverter.Deserialize<CursorTokenModel>(encoded);

        // Assert
        Assert.That(decoded.Id, Is.EqualTo(originalToken.Id));
        Assert.That(decoded.PageSize, Is.EqualTo(originalToken.PageSize));
    }

    [Test]
    public void DeserializeAnonymous_ShouldWorkCorrectly()
    {
        // Arrange
        var originalToken = new CursorTokenModel { Id = 300, PageSize = 75 };
        var encoded = Base62JsonConverter.Serialize(originalToken);

        // Act
        var anonymousDecoded = Base62JsonConverter.DeserializeAnonymous(encoded, new CursorTokenModel());

        // Assert
        Assert.That(anonymousDecoded.Id, Is.EqualTo(originalToken.Id));
        Assert.That(anonymousDecoded.PageSize, Is.EqualTo(originalToken.PageSize));
    }
}
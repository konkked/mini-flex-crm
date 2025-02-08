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
        var nextToken = new NextTokenModel { LastId = 100, PageSize = 50 };

        // Act
        var encoded = Base62JsonConverter.Serialize(nextToken);

        // Assert
        Assert.That(string.IsNullOrEmpty(encoded), Is.False);
    }

    [Test]
    public void Deserialize_ShouldDecodeNextTokenCorrectly()
    {
        // Arrange
        var originalToken = new NextTokenModel { LastId = 100, PageSize = 50 };
        var encoded = Base62JsonConverter.Serialize(originalToken);

        // Act
        var decoded = Base62JsonConverter.Deserialize<NextTokenModel>(encoded);

        // Assert
        Assert.That(decoded.LastId, Is.EqualTo(originalToken.LastId));
        Assert.That(decoded.PageSize, Is.EqualTo(originalToken.PageSize));
    }

    [Test]
    public void Deserialize_ShouldThrowExceptionOnInvalidBase62String()
    {
        // Arrange
        var invalidBase62 = "InvalidString@@!!";

        // Act & Assert
        Assert.Throws<JsonException>(() =>
            Base62JsonConverter.Deserialize<NextTokenModel>(invalidBase62));
    }

    [Test]
    public void SerializeAndDeserialize_ShouldMaintainDataIntegrity()
    {
        // Arrange
        var originalToken = new NextTokenModel { LastId = 250, PageSize = 100 };

        // Act
        var encoded = Base62JsonConverter.Serialize(originalToken);
        var decoded = Base62JsonConverter.Deserialize<NextTokenModel>(encoded);

        // Assert
        Assert.That(decoded.LastId, Is.EqualTo(originalToken.LastId));
        Assert.That(decoded.PageSize, Is.EqualTo(originalToken.PageSize));
    }

    [Test]
    public void DeserializeAnonymous_ShouldWorkCorrectly()
    {
        // Arrange
        var originalToken = new NextTokenModel { LastId = 300, PageSize = 75 };
        var encoded = Base62JsonConverter.Serialize(originalToken);

        // Act
        var anonymousDecoded = Base62JsonConverter.DeserializeAnonymous(encoded, new NextTokenModel());

        // Assert
        Assert.That(anonymousDecoded.LastId, Is.EqualTo(originalToken.LastId));
        Assert.That(anonymousDecoded.PageSize, Is.EqualTo(originalToken.PageSize));
    }
}
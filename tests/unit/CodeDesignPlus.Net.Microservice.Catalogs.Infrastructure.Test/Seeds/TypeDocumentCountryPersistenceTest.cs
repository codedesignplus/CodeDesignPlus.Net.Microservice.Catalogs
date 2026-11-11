using CodeDesignPlus.Net.Mongo.Extensions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Xunit;

namespace CodeDesignPlus.Net.Microservice.Catalogs.Infrastructure.Test.Seeds;

/// <summary>
/// El pais es un campo nuevo de un agregado que ya esta en Mongo (regla 31): los tipos guardados antes no lo traen y
/// tienen que seguir leyendose (pendings/663).
/// </summary>
public class TypeDocumentCountryPersistenceTest
{
    static TypeDocumentCountryPersistenceTest() => MongoSerializerRegistration.RegisterSerializers();

    [Fact]
    public void Deserialize_DocumentSavedWithoutCountry_ReadsWithNullCountry()
    {
        // Arrange
        var document = TypeDocumentAggregate.Create(Guid.NewGuid(), "Cédula de Ciudadanía", null, "CC", "CO", true, Guid.NewGuid()).ToBsonDocument();
        document.Remove("Country");

        // Act
        var read = BsonSerializer.Deserialize<TypeDocumentAggregate>(document);

        // Assert
        Assert.Equal(("CC", (string?)null), (read.Code, read.Country));
    }

    [Fact]
    public void Deserialize_DocumentWithCountry_KeepsIt()
    {
        // Arrange
        var document = TypeDocumentAggregate.Create(Guid.NewGuid(), "Cédula de Ciudadanía", null, "13", "co", true, Guid.NewGuid()).ToBsonDocument();

        // Act
        var read = BsonSerializer.Deserialize<TypeDocumentAggregate>(document);

        // Assert
        Assert.Equal("CO", read.Country);
    }
}

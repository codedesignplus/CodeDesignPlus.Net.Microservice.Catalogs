using System.Threading;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Microservice.Catalogs.Domain.Repositories;
using CodeDesignPlus.Net.Microservice.Catalogs.Infrastructure.Seeds;
using Microsoft.Extensions.Logging.Abstractions;
using CodeDesignPlus.Net.Mongo.Extensions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Xunit;

namespace CodeDesignPlus.Net.Microservice.Catalogs.Infrastructure.Test.Seeds;

/// <summary>
/// La siembra de tipos de documento: pais del despliegue, por id y sin tocar lo existente (plan 049 de pendings).
/// </summary>
public class CatalogSeedServiceTest
{
    /// <summary>
    /// Colombia siembra la tabla de tipos de documento de la DIAN, con su codigo (pendings/663).
    /// </summary>
    [Fact]
    public void LoadTypeDocuments_Colombia_HasTheDianCodes()
    {
        // Act
        var types = CatalogSeedService.LoadTypeDocuments("co");

        // Assert
        Assert.Equal(["11", "12", "13", "21", "22", "31", "41", "42", "47", "48", "50", "91"], types.Select(t => t.Code));
    }

    /// <summary>
    /// El id de cada tipo es fijo y sale de su codigo DIAN, el mismo en todos los entornos y en ms-microsoftgraph.
    /// </summary>
    [Fact]
    public void LoadTypeDocuments_Colombia_IdsAreFixedByDianCode()
    {
        // Act
        var types = CatalogSeedService.LoadTypeDocuments("co");

        // Assert
        Assert.All(types, t => Assert.Equal(Guid.Parse($"c0d0c000-0000-0000-0000-0000000000{t.Code}"), t.Id));
    }

    /// <summary>
    /// Cada tipo del fichero de un pais es de ese pais, y se guarda con el (pendings/663).
    /// </summary>
    [Fact]
    public async Task SeedTypeDocumentsAsync_Colombia_StoresTheCountry()
    {
        // Arrange
        var types = CatalogSeedService.LoadTypeDocuments("co");
        var repository = new Mock<ITypeDocumentRepository>();
        var created = new List<TypeDocumentAggregate>();
        repository.Setup(x => x.CreateAsync(It.IsAny<TypeDocumentAggregate>(), It.IsAny<CancellationToken>()))
            .Callback<TypeDocumentAggregate, CancellationToken>((t, _) => created.Add(t));
        var service = new CatalogSeedService(repository.Object, new ConfigurationBuilder().Build(), NullLogger<CatalogSeedService>.Instance);

        // Act
        await service.SeedTypeDocumentsAsync(types, CancellationToken.None);

        // Assert
        Assert.Equal(12, created.Count);
        Assert.All(created, t => Assert.Equal("CO", t.Country));
    }

    /// <summary>Sin pais, o con un pais sin fichero, el micro no arranca.</summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("xx")]
    public void CountryWithoutFile_FailsToStart(string? country)
    {
        Assert.Throws<InvalidOperationException>(() => CatalogSeedService.LoadTypeDocuments(country));
    }

    /// <summary>Solo se insertan los que faltan, con el usuario de sistema, y nunca se actualiza lo existente.</summary>
    [Fact]
    public async Task InsertsOnlyMissingTypes_AndNeverUpdatesExistingOnes()
    {
        var types = CatalogSeedService.LoadTypeDocuments("co");
        var existingId = types[0].Id;
        var repository = new Mock<ITypeDocumentRepository>();
        repository.Setup(x => x.ExistsAsync<TypeDocumentAggregate>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => id == existingId);
        var created = new List<TypeDocumentAggregate>();
        repository.Setup(x => x.CreateAsync(It.IsAny<TypeDocumentAggregate>(), It.IsAny<CancellationToken>()))
            .Callback<TypeDocumentAggregate, CancellationToken>((t, _) => created.Add(t));

        var service = new CatalogSeedService(repository.Object, new ConfigurationBuilder().Build(), NullLogger<CatalogSeedService>.Instance);
        await service.SeedTypeDocumentsAsync(types, CancellationToken.None);

        Assert.Equal(types.Count - 1, created.Count);
        Assert.DoesNotContain(created, t => t.Id == existingId);
        Assert.All(created, t => Assert.Equal(CatalogSeedService.SystemUserId, t.CreatedBy));
        repository.Verify(x => x.UpdateAsync(It.IsAny<TypeDocumentAggregate>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>No se siembra un tipo cuyo codigo ya usa otro tipo.</summary>
    [Fact]
    public async Task SkipsATypeWhoseCodeIsAlreadyInUse()
    {
        var types = CatalogSeedService.LoadTypeDocuments("co");
        var repository = new Mock<ITypeDocumentRepository>();
        repository.Setup(x => x.ExistsCodeAsync("13", It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var created = new List<TypeDocumentAggregate>();
        repository.Setup(x => x.CreateAsync(It.IsAny<TypeDocumentAggregate>(), It.IsAny<CancellationToken>()))
            .Callback<TypeDocumentAggregate, CancellationToken>((t, _) => created.Add(t));

        var service = new CatalogSeedService(repository.Object, new ConfigurationBuilder().Build(), NullLogger<CatalogSeedService>.Instance);
        await service.SeedTypeDocumentsAsync(types, CancellationToken.None);

        Assert.DoesNotContain(created, t => t.Code == "13");
        Assert.Equal(types.Count - 1, created.Count);
    }

    /// <summary>
    /// Cada tipo de Colombia trae la abreviatura con la que lo muestran las pantallas (pendings/663).
    /// </summary>
    [Fact]
    public void LoadTypeDocuments_Colombia_HasTheAbbreviations()
    {
        // Act
        var types = CatalogSeedService.LoadTypeDocuments("co");

        // Assert
        Assert.Equal(
            ["RC", "TI", "CC", "TE", "CE", "NIT", "PP", "DIE", "PEP", "PPT", "NITE", "NUIP"],
            types.Select(t => t.Abbreviation));
    }

    /// <summary>
    /// Un tipo nuevo se siembra con su abreviatura (pendings/663).
    /// </summary>
    [Fact]
    public async Task SeedTypeDocumentsAsync_Colombia_StoresTheAbbreviation()
    {
        // Arrange
        var types = CatalogSeedService.LoadTypeDocuments("co");
        var repository = new Mock<ITypeDocumentRepository>();
        var created = new List<TypeDocumentAggregate>();
        repository.Setup(x => x.CreateAsync(It.IsAny<TypeDocumentAggregate>(), It.IsAny<CancellationToken>()))
            .Callback<TypeDocumentAggregate, CancellationToken>((t, _) => created.Add(t));
        var service = new CatalogSeedService(repository.Object, new ConfigurationBuilder().Build(), NullLogger<CatalogSeedService>.Instance);

        // Act
        await service.SeedTypeDocumentsAsync(types, CancellationToken.None);

        // Assert
        Assert.Equal(types.Select(t => t.Abbreviation), created.Select(t => t.Abbreviation));
    }

    /// <summary>
    /// Un tipo sembrado antes de que existiera la abreviatura la recibe del fichero, sin crear otro (regla 25,
    /// pendings/663).
    /// </summary>
    [Fact]
    public async Task SeedTypeDocumentsAsync_SeededTypeWithoutAbbreviation_GetsItFromTheFile()
    {
        // Arrange
        var types = CatalogSeedService.LoadTypeDocuments("co");
        var nit = types.Single(t => t.Code == "31");
        var stored = StoredWithoutAbbreviation(nit);
        var repository = RepositoryWith(stored);
        var service = new CatalogSeedService(repository.Object, new ConfigurationBuilder().Build(), NullLogger<CatalogSeedService>.Instance);

        // Act
        await service.SeedTypeDocumentsAsync(types, CancellationToken.None);

        // Assert
        Assert.Equal("NIT", stored.Abbreviation);
        Assert.Equal(CatalogSeedService.SystemUserId, stored.UpdatedBy);
        repository.Verify(x => x.UpdateAsync(stored, It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(x => x.CreateAsync(It.Is<TypeDocumentAggregate>(t => t.Id == nit.Id), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// Completar solo anade: la abreviatura que el usuario puso a un tipo sembrado se queda (regla 25, pendings/663).
    /// </summary>
    [Fact]
    public async Task SeedTypeDocumentsAsync_SeededTypeWithItsOwnAbbreviation_IsNotTouched()
    {
        // Arrange
        var types = CatalogSeedService.LoadTypeDocuments("co");
        var nit = types.Single(t => t.Code == "31");
        var stored = TypeDocumentAggregate.Create(nit.Id, nit.Name, nit.Description, nit.Code, "N.I.T.", "CO", true, Guid.NewGuid());
        var repository = RepositoryWith(stored);
        var service = new CatalogSeedService(repository.Object, new ConfigurationBuilder().Build(), NullLogger<CatalogSeedService>.Instance);

        // Act
        await service.SeedTypeDocumentsAsync(types, CancellationToken.None);

        // Assert
        Assert.Equal("N.I.T.", stored.Abbreviation);
        repository.Verify(x => x.UpdateAsync(It.IsAny<TypeDocumentAggregate>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// Un tipo guardado sin abreviatura: se serializa uno de verdad y se le quita el campo, como lo deja Mongo (regla 31).
    /// </summary>
    private static TypeDocumentAggregate StoredWithoutAbbreviation(TypeDocumentSeed seed)
    {
        MongoSerializerRegistration.RegisterSerializers();

        var document = TypeDocumentAggregate.Create(seed.Id, seed.Name, seed.Description, seed.Code, "X", "CO", true, Guid.NewGuid()).ToBsonDocument();
        document.Remove("Abbreviation");

        return BsonSerializer.Deserialize<TypeDocumentAggregate>(document);
    }

    private static Mock<ITypeDocumentRepository> RepositoryWith(TypeDocumentAggregate stored)
    {
        var repository = new Mock<ITypeDocumentRepository>();
        repository.Setup(x => x.ExistsAsync<TypeDocumentAggregate>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => id == stored.Id);
        repository.Setup(x => x.FindAsync<TypeDocumentAggregate>(stored.Id, It.IsAny<CancellationToken>())).ReturnsAsync(stored);

        return repository;
    }
}

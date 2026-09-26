using System.Threading;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Microservice.Catalogs.Domain.Repositories;
using CodeDesignPlus.Net.Microservice.Catalogs.Infrastructure.Seeds;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace CodeDesignPlus.Net.Microservice.Catalogs.Infrastructure.Test.Seeds;

/// <summary>
/// La siembra de tipos de documento: pais del despliegue, por id y sin tocar lo existente (plan 049 de pendings).
/// </summary>
public class CatalogSeedServiceTest
{
    /// <summary>Colombia siembra seis tipos, sin «Celular», y sin ids repetidos.</summary>
    [Fact]
    public void Colombia_HasSixTypes_WithoutCellPhone()
    {
        var types = CatalogSeedService.LoadTypeDocuments("co");

        Assert.Equal(["CC", "CE", "NIT", "PP", "TI", "RC"], types.Select(t => t.Code));
        Assert.Equal(types.Count, types.Select(t => t.Id).Distinct().Count());
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
        repository.Setup(x => x.ExistsCodeAsync("CC", It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var created = new List<TypeDocumentAggregate>();
        repository.Setup(x => x.CreateAsync(It.IsAny<TypeDocumentAggregate>(), It.IsAny<CancellationToken>()))
            .Callback<TypeDocumentAggregate, CancellationToken>((t, _) => created.Add(t));

        var service = new CatalogSeedService(repository.Object, new ConfigurationBuilder().Build(), NullLogger<CatalogSeedService>.Instance);
        await service.SeedTypeDocumentsAsync(types, CancellationToken.None);

        Assert.DoesNotContain(created, t => t.Code == "CC");
        Assert.Equal(types.Count - 1, created.Count);
    }
}

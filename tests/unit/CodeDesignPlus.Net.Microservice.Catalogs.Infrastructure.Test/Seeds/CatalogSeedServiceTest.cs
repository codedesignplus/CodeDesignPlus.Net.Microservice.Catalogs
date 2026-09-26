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
    [Fact]
    public void Colombia_TieneSeisTipos_SinCelular()
    {
        var tipos = CatalogSeedService.LoadTypeDocuments("co");

        Assert.Equal(["CC", "CE", "NIT", "PP", "TI", "RC"], tipos.Select(t => t.Code));
        Assert.Equal(tipos.Count, tipos.Select(t => t.Id).Distinct().Count());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("xx")]
    public void UnPaisSinFichero_NoArranca(string? pais)
    {
        Assert.Throws<InvalidOperationException>(() => CatalogSeedService.LoadTypeDocuments(pais));
    }

    [Fact]
    public async Task InsertaSoloLosQueFaltan_YNoTocaLosQueExisten()
    {
        var tipos = CatalogSeedService.LoadTypeDocuments("co");
        var existente = tipos[0].Id;
        var repository = new Mock<ITypeDocumentRepository>();
        repository.Setup(x => x.ExistsAsync<TypeDocumentAggregate>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => id == existente);
        var creados = new List<TypeDocumentAggregate>();
        repository.Setup(x => x.CreateAsync(It.IsAny<TypeDocumentAggregate>(), It.IsAny<CancellationToken>()))
            .Callback<TypeDocumentAggregate, CancellationToken>((t, _) => creados.Add(t));

        var servicio = new CatalogSeedService(repository.Object, new ConfigurationBuilder().Build(), NullLogger<CatalogSeedService>.Instance);
        await servicio.SeedTypeDocumentsAsync(tipos, CancellationToken.None);

        Assert.Equal(tipos.Count - 1, creados.Count);
        Assert.DoesNotContain(creados, t => t.Id == existente);
        Assert.All(creados, t => Assert.Equal(CatalogSeedService.SystemUserId, t.CreatedBy));
        repository.Verify(x => x.UpdateAsync(It.IsAny<TypeDocumentAggregate>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task NoSiembraUnTipoCuyoCodigoYaUsaOtro()
    {
        var tipos = CatalogSeedService.LoadTypeDocuments("co");
        var repository = new Mock<ITypeDocumentRepository>();
        repository.Setup(x => x.ExistsCodeAsync("CC", It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var creados = new List<TypeDocumentAggregate>();
        repository.Setup(x => x.CreateAsync(It.IsAny<TypeDocumentAggregate>(), It.IsAny<CancellationToken>()))
            .Callback<TypeDocumentAggregate, CancellationToken>((t, _) => creados.Add(t));

        var servicio = new CatalogSeedService(repository.Object, new ConfigurationBuilder().Build(), NullLogger<CatalogSeedService>.Instance);
        await servicio.SeedTypeDocumentsAsync(tipos, CancellationToken.None);

        Assert.DoesNotContain(creados, t => t.Code == "CC");
        Assert.Equal(tipos.Count - 1, creados.Count);
    }
}

using System.Threading;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Cache.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Models.Pager;
using CodeDesignPlus.Net.Microservice.Catalogs.Application.TypeDocument.Commands.CreateTypeDocument;
using CodeDesignPlus.Net.Microservice.Catalogs.Application.TypeDocument.Commands.DeleteTypeDocument;
using CodeDesignPlus.Net.Microservice.Catalogs.Application.TypeDocument.Commands.UpdateTypeDocument;
using CodeDesignPlus.Net.Microservice.Catalogs.Application.TypeDocument.Queries.GetAllTypeDocument;
using CodeDesignPlus.Net.Microservice.Catalogs.Application.TypeDocument.Queries.GetTypeDocumentById;
using AppErrors = CodeDesignPlus.Net.Microservice.Catalogs.Application.Errors;

namespace CodeDesignPlus.Net.Microservice.Catalogs.Application.Test.TypeDocument;

/// <summary>
/// Planes 047 (listado y cache), 049 (codigo unico), 050 (mensajes) y 051 (auditoria) de pendings.
/// </summary>
public class ListadoYCacheTest
{
    private readonly Mock<ITypeDocumentRepository> repository = new();
    private readonly Mock<IPubSub> pubsub = new();
    private readonly Mock<ICacheManager> cache = new();
    private readonly Mock<IUserContext> user = new();
    private readonly Guid idUser = Guid.NewGuid();

    public ListadoYCacheTest()
    {
        user.SetupGet(x => x.IdUser).Returns(idUser);
    }

    private static TypeDocumentAggregate Tipo(Guid id) =>
        TypeDocumentAggregate.Create(id, "Pasaporte", "Documento de viaje", "PP", true, Guid.NewGuid());

    private static TypeDocumentDto Dto(Guid id) =>
        new() { Id = id, Name = "Pasaporte", Description = null, Code = "PP", IsActive = true };

    // ---------------------------------------------------------------- 047

    [Fact]
    public async Task Listado_PasaLosCriteriosAlRepositorio()
    {
        var criteria = new C.Criteria { Filters = "isActive=true", OrderBy = "name", Limit = 2, Skip = 0 };
        var pagina = new Pagination<TypeDocumentAggregate>([Tipo(Guid.NewGuid())], 7, 2, 0);
        repository.Setup(x => x.MatchingAsync<TypeDocumentAggregate>(criteria, It.IsAny<CancellationToken>())).ReturnsAsync(pagina);
        var mapper = new Mock<IMapper>();
        var esperado = new Pagination<TypeDocumentDto>([Dto(Guid.NewGuid())], 7, 2, 0);
        mapper.Setup(x => x.Map<Pagination<TypeDocumentDto>>(pagina)).Returns(esperado);

        var handler = new GetAllTypeDocumentQueryHandler(repository.Object, mapper.Object);
        var resultado = await handler.Handle(new GetAllTypeDocumentQuery(criteria), CancellationToken.None);

        Assert.Same(esperado, resultado);
        repository.Verify(x => x.MatchingAsync<TypeDocumentAggregate>(criteria, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Detalle_DeUnTipoQueNoExiste_DaTypeDocumentNotFound_YNoCacheaNada()
    {
        var id = Guid.NewGuid();
        cache.Setup(x => x.ExistsAsync(id.ToString())).ReturnsAsync(false);
        repository.Setup(x => x.FindAsync<TypeDocumentAggregate>(id, It.IsAny<CancellationToken>())).ReturnsAsync((TypeDocumentAggregate)null!);

        var handler = new GetTypeDocumentByIdQueryHandler(repository.Object, new Mock<IMapper>().Object, cache.Object);
        var error = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(new GetTypeDocumentByIdQuery(id), CancellationToken.None));

        Assert.Equal(AppErrors.TypeDocumentNotFound.GetCode(), error.Code);
        cache.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<TypeDocumentDto>(), It.IsAny<TimeSpan?>()), Times.Never);
    }

    [Fact]
    public async Task Editar_LimpiaElDetalleEnCache_YGuardaQuienEdita()
    {
        var id = Guid.NewGuid();
        var tipo = Tipo(id);
        repository.Setup(x => x.FindAsync<TypeDocumentAggregate>(id, It.IsAny<CancellationToken>())).ReturnsAsync(tipo);

        var handler = new UpdateTypeDocumentCommandHandler(repository.Object, pubsub.Object, cache.Object, user.Object);
        await handler.Handle(new UpdateTypeDocumentCommand(id, "Pasaporte", null, "PP", false), CancellationToken.None);

        cache.Verify(x => x.RemoveAsync(id.ToString()), Times.Once);
        Assert.Equal(idUser, tipo.UpdatedBy);
    }

    [Fact]
    public async Task Borrar_LimpiaElDetalleEnCache_YGuardaQuienBorra()
    {
        var id = Guid.NewGuid();
        var tipo = Tipo(id);
        repository.Setup(x => x.FindAsync<TypeDocumentAggregate>(id, It.IsAny<CancellationToken>())).ReturnsAsync(tipo);

        var handler = new DeleteTypeDocumentCommandHandler(repository.Object, pubsub.Object, cache.Object, user.Object);
        await handler.Handle(new DeleteTypeDocumentCommand(id), CancellationToken.None);

        cache.Verify(x => x.RemoveAsync(id.ToString()), Times.Once);
        Assert.Equal(idUser, tipo.DeletedBy);
    }

    // ---------------------------------------------------------------- 049 y 051

    [Fact]
    public async Task Crear_ConUnCodigoQueYaUsaOtroTipo_DaTypeDocumentCodeAlreadyExists()
    {
        var id = Guid.NewGuid();
        repository.Setup(x => x.ExistsCodeAsync("CC", id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new CreateTypeDocumentCommandHandler(repository.Object, pubsub.Object, user.Object);
        var error = await Assert.ThrowsAsync<CodeDesignPlusException>(() =>
            handler.Handle(new CreateTypeDocumentCommand(id, "Otra cédula", null, "CC", true), CancellationToken.None));

        Assert.Equal(AppErrors.TypeDocumentCodeAlreadyExists.GetCode(), error.Code);
        repository.Verify(x => x.CreateAsync(It.IsAny<TypeDocumentAggregate>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Editar_ConUnCodigoQueYaUsaOtroTipo_DaTypeDocumentCodeAlreadyExists()
    {
        var id = Guid.NewGuid();
        repository.Setup(x => x.FindAsync<TypeDocumentAggregate>(id, It.IsAny<CancellationToken>())).ReturnsAsync(Tipo(id));
        repository.Setup(x => x.ExistsCodeAsync("CC", id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new UpdateTypeDocumentCommandHandler(repository.Object, pubsub.Object, cache.Object, user.Object);
        var error = await Assert.ThrowsAsync<CodeDesignPlusException>(() =>
            handler.Handle(new UpdateTypeDocumentCommand(id, "Pasaporte", null, "CC", true), CancellationToken.None));

        Assert.Equal(AppErrors.TypeDocumentCodeAlreadyExists.GetCode(), error.Code);
    }

    [Fact]
    public async Task Crear_GuardaQuienCrea()
    {
        var id = Guid.NewGuid();
        TypeDocumentAggregate? creado = null;
        repository.Setup(x => x.CreateAsync(It.IsAny<TypeDocumentAggregate>(), It.IsAny<CancellationToken>()))
            .Callback<TypeDocumentAggregate, CancellationToken>((t, _) => creado = t);

        var handler = new CreateTypeDocumentCommandHandler(repository.Object, pubsub.Object, user.Object);
        await handler.Handle(new CreateTypeDocumentCommand(id, "Pasaporte", null, " pp ", true), CancellationToken.None);

        Assert.NotNull(creado);
        Assert.Equal(idUser, creado!.CreatedBy);
        Assert.Equal("PP", creado.Code);
    }

    [Fact]
    public void Agregado_SinUsuario_NoSeCrea()
    {
        var error = Assert.Throws<CodeDesignPlusException>(() =>
            TypeDocumentAggregate.Create(Guid.NewGuid(), "Pasaporte", null, "PP", true, Guid.Empty));

        Assert.Equal(Domain.Errors.UserRequired.GetCode(), error.Code);
    }

    // ---------------------------------------------------------------- 050

    [Fact]
    public void Validador_NombraLosCamposEnEspanol_NoConElNombreDeLaPropiedad()
    {
        var resultado = new CodeDesignPlus.Net.Microservice.Catalogs.Application.TypeDocument.Commands.CreateTypeDocument.Validator()
            .Validate(new CreateTypeDocumentCommand(Guid.NewGuid(), "", new string('d', 600), "ABCDEFG", true));

        var mensajes = resultado.Errors.Select(e => e.ErrorMessage).ToList();

        Assert.Contains(AppErrors.NameIsRequired.GetMessage(), mensajes);
        Assert.Contains(AppErrors.CodeMaxLengthExceeded.GetMessage(), mensajes);
        Assert.Contains(AppErrors.DescriptionMaxLengthExceeded.GetMessage(), mensajes);
        Assert.DoesNotContain(mensajes, m => m.StartsWith("Name ") || m.StartsWith("Code ") || m.StartsWith("Description "));
    }
}

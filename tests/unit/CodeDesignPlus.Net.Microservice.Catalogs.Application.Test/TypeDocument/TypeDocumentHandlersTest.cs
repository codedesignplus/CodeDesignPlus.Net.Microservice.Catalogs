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
using CreateValidator = CodeDesignPlus.Net.Microservice.Catalogs.Application.TypeDocument.Commands.CreateTypeDocument.Validator;

namespace CodeDesignPlus.Net.Microservice.Catalogs.Application.Test.TypeDocument;

/// <summary>
/// Planes 047 (listado y cache), 049 (codigo unico), 050 (mensajes) y 051 (auditoria) de pendings.
/// </summary>
public class TypeDocumentHandlersTest
{
    private readonly Mock<ITypeDocumentRepository> repository = new();
    private readonly Mock<IPubSub> pubsub = new();
    private readonly Mock<ICacheManager> cache = new();
    private readonly Mock<IUserContext> user = new();
    private readonly Guid userId = Guid.NewGuid();

    public TypeDocumentHandlersTest()
    {
        user.SetupGet(x => x.IdUser).Returns(userId);
    }

    private static TypeDocumentAggregate NewTypeDocument(Guid id) =>
        TypeDocumentAggregate.Create(id, "Pasaporte", "Documento de viaje", "PP", true, Guid.NewGuid());

    private static TypeDocumentDto NewDto(Guid id) =>
        new() { Id = id, Name = "Pasaporte", Description = null, Code = "PP", IsActive = true };

    /// <summary>El listado pasa filtro, orden y paginacion al repositorio (plan 047).</summary>
    [Fact]
    public async Task GetAll_PassesTheCriteriaToTheRepository()
    {
        var criteria = new C.Criteria { Filters = "isActive=true", OrderBy = "name", Limit = 2, Skip = 0 };
        var page = new Pagination<TypeDocumentAggregate>([NewTypeDocument(Guid.NewGuid())], 7, 2, 0);
        repository.Setup(x => x.MatchingAsync<TypeDocumentAggregate>(criteria, It.IsAny<CancellationToken>())).ReturnsAsync(page);
        var mapper = new Mock<IMapper>();
        var expected = new Pagination<TypeDocumentDto>([NewDto(Guid.NewGuid())], 7, 2, 0);
        mapper.Setup(x => x.Map<Pagination<TypeDocumentDto>>(page)).Returns(expected);

        var handler = new GetAllTypeDocumentQueryHandler(repository.Object, mapper.Object);
        var result = await handler.Handle(new GetAllTypeDocumentQuery(criteria), CancellationToken.None);

        Assert.Same(expected, result);
        repository.Verify(x => x.MatchingAsync<TypeDocumentAggregate>(criteria, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>Un tipo que no existe da el error 203 y no se guarda nada en cache; antes era un 500 (plan 047).</summary>
    [Fact]
    public async Task GetById_UnknownId_ThrowsNotFound_AndCachesNothing()
    {
        var id = Guid.NewGuid();
        cache.Setup(x => x.ExistsAsync(id.ToString())).ReturnsAsync(false);
        repository.Setup(x => x.FindAsync<TypeDocumentAggregate>(id, It.IsAny<CancellationToken>())).ReturnsAsync((TypeDocumentAggregate)null!);

        var handler = new GetTypeDocumentByIdQueryHandler(repository.Object, new Mock<IMapper>().Object, cache.Object);
        var error = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(new GetTypeDocumentByIdQuery(id), CancellationToken.None));

        Assert.Equal(AppErrors.TypeDocumentNotFound.GetCode(), error.Code);
        cache.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<TypeDocumentDto>(), It.IsAny<TimeSpan?>()), Times.Never);
    }

    /// <summary>Editar limpia el detalle en cache y guarda quien edita (planes 047 y 051).</summary>
    [Fact]
    public async Task Update_ClearsTheCachedDetail_AndRecordsWhoUpdated()
    {
        var id = Guid.NewGuid();
        var typeDocument = NewTypeDocument(id);
        repository.Setup(x => x.FindAsync<TypeDocumentAggregate>(id, It.IsAny<CancellationToken>())).ReturnsAsync(typeDocument);

        var handler = new UpdateTypeDocumentCommandHandler(repository.Object, pubsub.Object, cache.Object, user.Object);
        await handler.Handle(new UpdateTypeDocumentCommand(id, "Pasaporte", null, "PP", false), CancellationToken.None);

        cache.Verify(x => x.RemoveAsync(id.ToString()), Times.Once);
        Assert.Equal(userId, typeDocument.UpdatedBy);
    }

    /// <summary>Borrar limpia el detalle en cache y guarda quien borra; antes seguia respondiendo 200 (planes 047 y 051).</summary>
    [Fact]
    public async Task Delete_ClearsTheCachedDetail_AndRecordsWhoDeleted()
    {
        var id = Guid.NewGuid();
        var typeDocument = NewTypeDocument(id);
        repository.Setup(x => x.FindAsync<TypeDocumentAggregate>(id, It.IsAny<CancellationToken>())).ReturnsAsync(typeDocument);

        var handler = new DeleteTypeDocumentCommandHandler(repository.Object, pubsub.Object, cache.Object, user.Object);
        await handler.Handle(new DeleteTypeDocumentCommand(id), CancellationToken.None);

        cache.Verify(x => x.RemoveAsync(id.ToString()), Times.Once);
        Assert.Equal(userId, typeDocument.DeletedBy);
    }

    /// <summary>No se crea un tipo con un codigo que ya usa otro (plan 049).</summary>
    [Fact]
    public async Task Create_WithACodeAlreadyInUse_ThrowsCodeAlreadyExists()
    {
        var id = Guid.NewGuid();
        repository.Setup(x => x.ExistsCodeAsync("CC", id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new CreateTypeDocumentCommandHandler(repository.Object, pubsub.Object, user.Object);
        var error = await Assert.ThrowsAsync<CodeDesignPlusException>(() =>
            handler.Handle(new CreateTypeDocumentCommand(id, "Otra cédula", null, "CC", true), CancellationToken.None));

        Assert.Equal(AppErrors.TypeDocumentCodeAlreadyExists.GetCode(), error.Code);
        repository.Verify(x => x.CreateAsync(It.IsAny<TypeDocumentAggregate>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>Tampoco se edita un tipo para darle el codigo de otro (plan 049).</summary>
    [Fact]
    public async Task Update_WithACodeAlreadyInUse_ThrowsCodeAlreadyExists()
    {
        var id = Guid.NewGuid();
        repository.Setup(x => x.FindAsync<TypeDocumentAggregate>(id, It.IsAny<CancellationToken>())).ReturnsAsync(NewTypeDocument(id));
        repository.Setup(x => x.ExistsCodeAsync("CC", id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new UpdateTypeDocumentCommandHandler(repository.Object, pubsub.Object, cache.Object, user.Object);
        var error = await Assert.ThrowsAsync<CodeDesignPlusException>(() =>
            handler.Handle(new UpdateTypeDocumentCommand(id, "Pasaporte", null, "CC", true), CancellationToken.None));

        Assert.Equal(AppErrors.TypeDocumentCodeAlreadyExists.GetCode(), error.Code);
    }

    /// <summary>Crear guarda quien crea, y el codigo en mayusculas y sin espacios (planes 049 y 051).</summary>
    [Fact]
    public async Task Create_RecordsWhoCreated_AndNormalizesTheCode()
    {
        var id = Guid.NewGuid();
        TypeDocumentAggregate? created = null;
        repository.Setup(x => x.CreateAsync(It.IsAny<TypeDocumentAggregate>(), It.IsAny<CancellationToken>()))
            .Callback<TypeDocumentAggregate, CancellationToken>((t, _) => created = t);

        var handler = new CreateTypeDocumentCommandHandler(repository.Object, pubsub.Object, user.Object);
        await handler.Handle(new CreateTypeDocumentCommand(id, "Pasaporte", null, " pp ", true), CancellationToken.None);

        Assert.NotNull(created);
        Assert.Equal(userId, created!.CreatedBy);
        Assert.Equal("PP", created.Code);
    }

    /// <summary>El agregado no se crea sin saber quien lo crea (plan 051).</summary>
    [Fact]
    public void Aggregate_WithoutUser_IsRejected()
    {
        var error = Assert.Throws<CodeDesignPlusException>(() =>
            TypeDocumentAggregate.Create(Guid.NewGuid(), "Pasaporte", null, "PP", true, Guid.Empty));

        Assert.Equal(Domain.Errors.UserRequired.GetCode(), error.Code);
    }

    /// <summary>
    /// Los validadores corrientes van sin texto ni codigo propio, para que el SDK los traduzca por su codigo de
    /// FluentValidation (regla 35). Un WithMessage cambiaria el codigo y el texto quedaria en un solo idioma.
    /// </summary>
    [Fact]
    public void Validator_UsesTheStandardCodes_ThatTheSdkTranslates()
    {
        var result = new CreateValidator()
            .Validate(new CreateTypeDocumentCommand(Guid.NewGuid(), "", new string('d', 600), "ABCDEFG", true));

        Assert.Equal(
            ["NotEmptyValidator", "MaximumLengthValidator", "MaximumLengthValidator"],
            result.Errors.Select(e => e.ErrorCode));
    }
}

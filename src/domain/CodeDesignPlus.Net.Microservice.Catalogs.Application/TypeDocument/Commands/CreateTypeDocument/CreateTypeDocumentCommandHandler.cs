using CodeDesignPlus.Net.Microservice.Catalogs.Application.TypeDocument.Queries.GetAllTypeDocument;

namespace CodeDesignPlus.Net.Microservice.Catalogs.Application.TypeDocument.Commands.CreateTypeDocument;

public class CreateTypeDocumentCommandHandler(ITypeDocumentRepository repository, IPubSub pubsub, ICacheManager cacheManager) : IRequestHandler<CreateTypeDocumentCommand>
{
    public async Task Handle(CreateTypeDocumentCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var exist = await repository.ExistsAsync<TypeDocumentAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsTrue(exist, Errors.TypeDocumentAlreadyExists);

        var typeDocument = TypeDocumentAggregate.Create(request.Id, request.Name, request.Description, request.Code, request.IsActive);

        await repository.CreateAsync(typeDocument, cancellationToken);

        await pubsub.PublishAsync(typeDocument.GetAndClearEvents(), cancellationToken);

        await cacheManager.RemoveAsync(GetAllTypeDocumentQueryHandler.CACHE_KEY);
    }
}
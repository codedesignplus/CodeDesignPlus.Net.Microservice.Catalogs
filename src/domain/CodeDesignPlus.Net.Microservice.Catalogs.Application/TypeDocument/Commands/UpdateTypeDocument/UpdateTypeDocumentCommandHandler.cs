namespace CodeDesignPlus.Net.Microservice.Catalogs.Application.TypeDocument.Commands.UpdateTypeDocument;

public class UpdateTypeDocumentCommandHandler(ITypeDocumentRepository repository, IPubSub pubsub, ICacheManager cacheManager) : IRequestHandler<UpdateTypeDocumentCommand>
{
    public async Task Handle(UpdateTypeDocumentCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var typeDocument = await repository.FindAsync<TypeDocumentAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(typeDocument, Errors.TypeDocumentNotFound);

        typeDocument.Update(request.Name, request.Description, request.Code, request.IsActive);

        await repository.UpdateAsync(typeDocument, cancellationToken);

        await pubsub.PublishAsync(typeDocument.GetAndClearEvents(), cancellationToken);

        await cacheManager.RemoveAsync(request.Id.ToString());
    }
}
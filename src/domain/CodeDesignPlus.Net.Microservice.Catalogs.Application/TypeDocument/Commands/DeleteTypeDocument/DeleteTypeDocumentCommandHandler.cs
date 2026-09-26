namespace CodeDesignPlus.Net.Microservice.Catalogs.Application.TypeDocument.Commands.DeleteTypeDocument;

public class DeleteTypeDocumentCommandHandler(ITypeDocumentRepository repository, IPubSub pubsub, ICacheManager cacheManager, IUserContext user) : IRequestHandler<DeleteTypeDocumentCommand>
{
    public async Task Handle(DeleteTypeDocumentCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var aggregate = await repository.FindAsync<TypeDocumentAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(aggregate, Errors.TypeDocumentNotFound);

        aggregate.Delete(user.IdUser);

        await repository.DeleteAsync<TypeDocumentAggregate>(aggregate.Id, cancellationToken);

        await pubsub.PublishAsync(aggregate.GetAndClearEvents(), cancellationToken);

        // Sin esto el detalle de un tipo borrado seguia respondiendo 200 durante 6 h (plan 047).
        await cacheManager.RemoveAsync(request.Id.ToString());
    }
}

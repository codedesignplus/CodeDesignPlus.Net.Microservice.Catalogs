namespace CodeDesignPlus.Net.Microservice.Catalogs.Application.TypeDocument.Commands.DeleteTypeDocument;

public class DeleteTypeDocumentCommandHandler(ITypeDocumentRepository repository, IPubSub pubsub) : IRequestHandler<DeleteTypeDocumentCommand>
{
    public async Task Handle(DeleteTypeDocumentCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var aggregate = await repository.FindAsync<TypeDocumentAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(aggregate, Errors.TypeDocumentNotFound);

        aggregate.Delete();

        await repository.DeleteAsync<TypeDocumentAggregate>(aggregate.Id, cancellationToken);

        await pubsub.PublishAsync(aggregate.GetAndClearEvents(), cancellationToken);
    }
}
namespace CodeDesignPlus.Net.Microservice.Catalogs.Application.TypeDocument.Commands.CreateTypeDocument;

public class CreateTypeDocumentCommandHandler(ITypeDocumentRepository repository, IPubSub pubsub, IUserContext user) : IRequestHandler<CreateTypeDocumentCommand>
{
    public async Task Handle(CreateTypeDocumentCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var exist = await repository.ExistsAsync<TypeDocumentAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsTrue(exist, Errors.TypeDocumentAlreadyExists);

        // Los formularios guardan el tipo por su codigo: dos con el mismo serian indistinguibles (plan 049).
        var codeInUse = await repository.ExistsCodeAsync(request.Code, request.Id, cancellationToken);

        ApplicationGuard.IsTrue(codeInUse, Errors.TypeDocumentCodeAlreadyExists);

        var typeDocument = TypeDocumentAggregate.Create(request.Id, request.Name, request.Description, request.Code, request.IsActive, user.IdUser);

        await repository.CreateAsync(typeDocument, cancellationToken);

        await pubsub.PublishAsync(typeDocument.GetAndClearEvents(), cancellationToken);
    }
}

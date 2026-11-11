using CodeDesignPlus.Net.Microservice.Catalogs.Application.Options;
using Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.Microservice.Catalogs.Application.TypeDocument.Commands.CreateTypeDocument;

public class CreateTypeDocumentCommandHandler(ITypeDocumentRepository repository, IPubSub pubsub, IUserContext user, IOptions<SeedOptions> seed) : IRequestHandler<CreateTypeDocumentCommand>
{
    public async Task Handle(CreateTypeDocumentCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        // Ya guardado por un envío anterior del mismo formulario: el reintento termina bien sin repetir nada (pendings/589).
        if (await repository.ExistsAsync<TypeDocumentAggregate>(request.Id, cancellationToken))
            return;

        // Los formularios guardan el tipo por su codigo: dos con el mismo serian indistinguibles (plan 049).
        var codeInUse = await repository.ExistsCodeAsync(request.Code, request.Id, cancellationToken);

        ApplicationGuard.IsTrue(codeInUse, Errors.TypeDocumentCodeAlreadyExists);

        var typeDocument = TypeDocumentAggregate.Create(request.Id, request.Name, request.Description, request.Code, request.Abbreviation, seed.Value.Country, request.IsActive, user.IdUser);

        // Dos envíos que llegan a la vez pasan los dos la comprobación de arriba; el segundo choca contra el id y no publica.
        if (!await repository.CreateIfAbsentAsync(typeDocument, cancellationToken))
            return;

        await pubsub.PublishAsync(typeDocument.GetAndClearEvents(), cancellationToken);
    }
}

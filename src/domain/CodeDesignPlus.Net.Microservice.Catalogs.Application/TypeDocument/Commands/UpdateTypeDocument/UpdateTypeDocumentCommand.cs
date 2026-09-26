namespace CodeDesignPlus.Net.Microservice.Catalogs.Application.TypeDocument.Commands.UpdateTypeDocument;

[DtoGenerator]
public record UpdateTypeDocumentCommand(Guid Id, string Name, string? Description, string Code, bool IsActive) : IRequest;

/// <summary>
/// Solo validadores corrientes y sin texto propio: el SDK los traduce a los cuatro idiomas con el nombre de la
/// propiedad del comando (regla 35 de Microservices/rules/). Nada de WithMessage, que queda congelado en un idioma.
/// </summary>
public class Validator : AbstractValidator<UpdateTypeDocumentCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Description).MaximumLength(512);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(4);
    }
}

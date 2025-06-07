namespace CodeDesignPlus.Net.Microservice.Catalogs.Application.TypeDocument.Commands.DeleteTypeDocument;

[DtoGenerator]
public record DeleteTypeDocumentCommand(Guid Id) : IRequest;

public class Validator : AbstractValidator<DeleteTypeDocumentCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
    }
}

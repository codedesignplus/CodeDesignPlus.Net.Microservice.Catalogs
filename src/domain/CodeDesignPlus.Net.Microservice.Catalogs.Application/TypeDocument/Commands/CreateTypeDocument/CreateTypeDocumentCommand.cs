namespace CodeDesignPlus.Net.Microservice.Catalogs.Application.TypeDocument.Commands.CreateTypeDocument;

[DtoGenerator]
public record CreateTypeDocumentCommand(Guid Id, string Name, string? Description, string Code, bool IsActive) : IRequest;

public class Validator : AbstractValidator<CreateTypeDocumentCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
        RuleFor(x => x.Name).NotEmpty().NotNull().MaximumLength(64);
        RuleFor(x => x.Description).MaximumLength(512);
        RuleFor(x => x.Code).NotEmpty().NotNull().MaximumLength(4);
    }
}

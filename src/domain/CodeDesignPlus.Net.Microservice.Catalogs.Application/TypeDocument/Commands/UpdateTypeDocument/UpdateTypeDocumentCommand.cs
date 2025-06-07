namespace CodeDesignPlus.Net.Microservice.Catalogs.Application.TypeDocument.Commands.UpdateTypeDocument;

[DtoGenerator]
public record UpdateTypeDocumentCommand(Guid Id, string Name, string? Description, string Code, bool IsActive) : IRequest;

public class Validator : AbstractValidator<UpdateTypeDocumentCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
        RuleFor(x => x.Name).NotEmpty().NotNull().MaximumLength(64);
        RuleFor(x => x.Description).MaximumLength(512);
        RuleFor(x => x.Code).NotEmpty().NotNull().MaximumLength(4);
    }
}

namespace CodeDesignPlus.Net.Microservice.Catalogs.Application.TypeDocument.Commands.CreateTypeDocument;

[DtoGenerator]
public record CreateTypeDocumentCommand(Guid Id, string Name, string? Description, string Code, bool IsActive) : IRequest;

public class Validator : AbstractValidator<CreateTypeDocumentCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage(_ => Errors.IdIsRequired.GetMessage());

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(_ => Errors.NameIsRequired.GetMessage())
            .MaximumLength(64).WithMessage(_ => Errors.NameMaxLengthExceeded.GetMessage());

        RuleFor(x => x.Description)
            .MaximumLength(512).WithMessage(_ => Errors.DescriptionMaxLengthExceeded.GetMessage());

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage(_ => Errors.CodeIsRequired.GetMessage())
            .MaximumLength(4).WithMessage(_ => Errors.CodeMaxLengthExceeded.GetMessage());
    }
}

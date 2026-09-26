namespace CodeDesignPlus.Net.Microservice.Catalogs.Application.TypeDocument.Commands.UpdateTypeDocument;

[DtoGenerator]
public record UpdateTypeDocumentCommand(Guid Id, string Name, string? Description, string Code, bool IsActive) : IRequest;

public class Validator : AbstractValidator<UpdateTypeDocumentCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().WithErrorCode(Errors.IdIsRequired.GetCode());

        RuleFor(x => x.Name)
            .NotEmpty().WithErrorCode(Errors.NameIsRequired.GetCode())
            .MaximumLength(64).WithErrorCode(Errors.NameMaxLengthExceeded.GetCode());

        RuleFor(x => x.Description)
            .MaximumLength(512).WithErrorCode(Errors.DescriptionMaxLengthExceeded.GetCode());

        RuleFor(x => x.Code)
            .NotEmpty().WithErrorCode(Errors.CodeIsRequired.GetCode())
            .MaximumLength(4).WithErrorCode(Errors.CodeMaxLengthExceeded.GetCode());
    }
}

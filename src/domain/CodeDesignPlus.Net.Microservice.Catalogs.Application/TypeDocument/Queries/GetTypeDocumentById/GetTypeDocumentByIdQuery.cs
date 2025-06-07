namespace CodeDesignPlus.Net.Microservice.Catalogs.Application.TypeDocument.Queries.GetTypeDocumentById;

public record GetTypeDocumentByIdQuery(Guid Id) : IRequest<TypeDocumentDto>;


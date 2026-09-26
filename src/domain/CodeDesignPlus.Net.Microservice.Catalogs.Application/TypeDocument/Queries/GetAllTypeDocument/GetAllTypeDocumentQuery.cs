using CodeDesignPlus.Net.Core.Abstractions.Models.Pager;

namespace CodeDesignPlus.Net.Microservice.Catalogs.Application.TypeDocument.Queries.GetAllTypeDocument;

public record GetAllTypeDocumentQuery(C.Criteria Criteria) : IRequest<Pagination<TypeDocumentDto>>;

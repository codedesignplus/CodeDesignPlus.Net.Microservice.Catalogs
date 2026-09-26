using CodeDesignPlus.Net.Core.Abstractions.Models.Pager;

namespace CodeDesignPlus.Net.Microservice.Catalogs.Application.TypeDocument.Queries.GetAllTypeDocument;

/// <summary>
/// Lista los tipos de documento aplicando los criterios de la peticion: filtro, orden y paginacion.
/// </summary>
/// <remarks>
/// SIN CACHE DE LA LISTA, A PROPOSITO (plan 047 de pendings). La lista entera se guardaba 6 h bajo una sola clave, sin
/// tener en cuenta los criterios, y ni editar ni borrar la limpiaban: los formularios ofrecian tipos inactivos, con el
/// nombre viejo o ya borrados. El catalogo tiene media docena de filas; consultarlo directamente cuesta menos que
/// mantener una cache correcta por cada combinacion de filtro, orden y pagina.
/// </remarks>
public class GetAllTypeDocumentQueryHandler(ITypeDocumentRepository repository, IMapper mapper) : IRequestHandler<GetAllTypeDocumentQuery, Pagination<TypeDocumentDto>>
{
    public async Task<Pagination<TypeDocumentDto>> Handle(GetAllTypeDocumentQuery request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var typeDocuments = await repository.MatchingAsync<TypeDocumentAggregate>(request.Criteria, cancellationToken);

        return mapper.Map<Pagination<TypeDocumentDto>>(typeDocuments);
    }
}

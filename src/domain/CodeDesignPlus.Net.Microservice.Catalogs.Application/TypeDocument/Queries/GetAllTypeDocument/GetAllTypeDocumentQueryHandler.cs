namespace CodeDesignPlus.Net.Microservice.Catalogs.Application.TypeDocument.Queries.GetAllTypeDocument;

public class GetAllTypeDocumentQueryHandler(ITypeDocumentRepository repository, IMapper mapper, ICacheManager cacheManager) : IRequestHandler<GetAllTypeDocumentQuery, List<TypeDocumentDto>>
{
    public const string CACHE_KEY = "TypeDocumentsList";

    public async Task<List<TypeDocumentDto>> Handle(GetAllTypeDocumentQuery request, CancellationToken cancellationToken)
    {
        var exist = await cacheManager.ExistsAsync(CACHE_KEY);

        if (exist)
            return await cacheManager.GetAsync<List<TypeDocumentDto>>(CACHE_KEY);

        var typeDocuments = await repository.GetAllAsync(cancellationToken);

        var data = mapper.Map<List<TypeDocumentDto>>(typeDocuments);

        await cacheManager.SetAsync(CACHE_KEY, data, TimeSpan.FromHours(6));

        return data;
    }
}

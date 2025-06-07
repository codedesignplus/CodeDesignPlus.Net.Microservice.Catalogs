namespace CodeDesignPlus.Net.Microservice.Catalogs.Application.TypeDocument.Queries.GetTypeDocumentById;

public class GetTypeDocumentByIdQueryHandler(ITypeDocumentRepository repository, IMapper mapper, ICacheManager cacheManager) : IRequestHandler<GetTypeDocumentByIdQuery, TypeDocumentDto>
{
    public async Task<TypeDocumentDto> Handle(GetTypeDocumentByIdQuery request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);
        
        var exist = await cacheManager.ExistsAsync(request.Id.ToString());

        if (exist)
            return await cacheManager.GetAsync<TypeDocumentDto>(request.Id.ToString());

        var typeDocument = await repository.FindAsync<TypeDocumentAggregate>(request.Id, cancellationToken);

        var data = mapper.Map<TypeDocumentDto>(typeDocument);

        await cacheManager.SetAsync(request.Id.ToString(), data, TimeSpan.FromHours(6));

        return data;
    }
}

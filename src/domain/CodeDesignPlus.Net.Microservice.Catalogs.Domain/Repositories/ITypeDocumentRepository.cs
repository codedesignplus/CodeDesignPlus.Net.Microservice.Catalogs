namespace CodeDesignPlus.Net.Microservice.Catalogs.Domain.Repositories;

public interface ITypeDocumentRepository : IRepositoryBase
{
    Task<List<TypeDocumentAggregate>> GetAllAsync(CancellationToken cancellationToken);
}
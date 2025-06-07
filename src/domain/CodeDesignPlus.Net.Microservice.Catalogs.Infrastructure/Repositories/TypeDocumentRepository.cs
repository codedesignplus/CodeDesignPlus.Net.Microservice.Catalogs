
namespace CodeDesignPlus.Net.Microservice.Catalogs.Infrastructure.Repositories;

public class TypeDocumentRepository(IServiceProvider serviceProvider, IOptions<MongoOptions> mongoOptions, ILogger<TypeDocumentRepository> logger)

    : RepositoryBase(serviceProvider, mongoOptions, logger), ITypeDocumentRepository
{
    public Task<List<TypeDocumentAggregate>> GetAllAsync(CancellationToken cancellationToken)
    {
        var collection = GetCollection<TypeDocumentAggregate>();
        var filter = Builders<TypeDocumentAggregate>.Filter.Empty;
        return collection.Find(filter).ToListAsync(cancellationToken);
    }

}
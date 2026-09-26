
namespace CodeDesignPlus.Net.Microservice.Catalogs.Infrastructure.Repositories;

public class TypeDocumentRepository(IServiceProvider serviceProvider, IOptions<MongoOptions> mongoOptions, ILogger<TypeDocumentRepository> logger)

    : RepositoryBase(serviceProvider, mongoOptions, logger), ITypeDocumentRepository
{
    public Task<bool> ExistsCodeAsync(string code, Guid exceptId, CancellationToken cancellationToken)
    {
        var normalized = TypeDocumentAggregate.NormalizeCode(code);
        var filter = Builders<TypeDocumentAggregate>.Filter.And(
            Builders<TypeDocumentAggregate>.Filter.Eq(x => x.Code, normalized),
            Builders<TypeDocumentAggregate>.Filter.Ne(x => x.Id, exceptId));

        return GetCollection<TypeDocumentAggregate>().Find(filter).AnyAsync(cancellationToken);
    }
}

namespace CodeDesignPlus.Net.Microservice.Catalogs.Domain.Repositories;

public interface ITypeDocumentRepository : IRepositoryBase
{
    /// <summary>
    /// Indica si otro tipo de documento ya usa ese codigo. <paramref name="exceptId"/> excluye al propio tipo al editarlo.
    /// </summary>
    Task<bool> ExistsCodeAsync(string code, Guid exceptId, CancellationToken cancellationToken);
}

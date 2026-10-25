namespace CodeDesignPlus.Net.Microservice.Catalogs.Domain.Repositories;

public interface ITypeDocumentRepository : IRepositoryBase
{
    /// <summary>
    /// Indica si otro tipo de documento ya usa ese codigo. <paramref name="exceptId"/> excluye al propio tipo al editarlo.
    /// </summary>
    Task<bool> ExistsCodeAsync(string code, Guid exceptId, CancellationToken cancellationToken);

    /// <summary>
    /// Guarda el registro nuevo si su id no existe todavía (pendings/589, regla 66).
    /// </summary>
    /// <param name="typeDocument">El agregado nuevo.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>false</c> si otro envío del mismo formulario ya lo guardó: el segundo choca contra la clave primaria.</returns>
    Task<bool> CreateIfAbsentAsync(TypeDocumentAggregate typeDocument, CancellationToken cancellationToken);
}

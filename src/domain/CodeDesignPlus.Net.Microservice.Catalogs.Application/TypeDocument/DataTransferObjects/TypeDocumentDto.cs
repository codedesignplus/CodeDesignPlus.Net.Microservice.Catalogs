namespace CodeDesignPlus.Net.Microservice.Catalogs.Application.TypeDocument.DataTransferObjects;

public class TypeDocumentDto : IDtoBase
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Code { get; set; }
    public required bool IsActive { get; set; }

    /// <summary>
    /// El pais del tipo en ISO 3166-1 alfa-2, o <c>null</c> en los tipos guardados antes de pendings/663.
    /// </summary>
    public string? Country { get; set; }
}
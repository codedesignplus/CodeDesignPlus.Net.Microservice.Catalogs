namespace CodeDesignPlus.Net.Microservice.Catalogs.Application.TypeDocument.DataTransferObjects;

public class TypeDocumentDto : IDtoBase
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Code { get; set; }
    public required bool IsActive { get; set; }
}
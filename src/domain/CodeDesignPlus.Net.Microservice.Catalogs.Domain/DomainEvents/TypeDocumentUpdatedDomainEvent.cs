namespace CodeDesignPlus.Net.Microservice.Catalogs.Domain.DomainEvents;

[EventKey<TypeDocumentAggregate>(1, "TypeDocumentUpdatedDomainEvent")]
public class TypeDocumentUpdatedDomainEvent(
    Guid aggregateId,
    string name,
    string? description,
    string code,
    bool isActive,
    Guid? eventId = null,
    Instant? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string Name { get; } = name;
    public string? Description { get; } = description;
    public string Code { get; } = code;
    public bool IsActive { get; } = isActive;

    public static TypeDocumentUpdatedDomainEvent Create(Guid aggregateId, string name, string? description, string code, bool isActive)
    {
        return new TypeDocumentUpdatedDomainEvent(aggregateId, name, description, code, isActive);
    }
}

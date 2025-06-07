namespace CodeDesignPlus.Net.Microservice.Catalogs.Domain.DomainEvents;

[EventKey<TypeDocumentAggregate>(1, "TypeDocumentDeletedDomainEvent")]
public class TypeDocumentDeletedDomainEvent(
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

    public static TypeDocumentDeletedDomainEvent Create(Guid aggregateId, string name, string? description, string code, bool isActive)
    {
        return new TypeDocumentDeletedDomainEvent(aggregateId, name, description, code, isActive);
    }
}

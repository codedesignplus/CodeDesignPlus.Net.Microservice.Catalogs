namespace CodeDesignPlus.Net.Microservice.Catalogs.Domain.DomainEvents;

[EventKey<TypeDocumentAggregate>(1, "TypeDocumentCreatedDomainEvent")]
public class TypeDocumentCreatedDomainEvent(
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

    public static TypeDocumentCreatedDomainEvent Create(Guid aggregateId, string name, string? description, string code, bool isActive)
    {
        return new TypeDocumentCreatedDomainEvent(aggregateId, name, description, code, isActive);
    }
}

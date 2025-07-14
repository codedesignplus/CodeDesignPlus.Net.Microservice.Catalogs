namespace CodeDesignPlus.Net.Microservice.Catalogs.Domain;

public class TypeDocumentAggregate(Guid id) : AggregateRootBase(id)
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; } = null!;
    public string Code { get; private set; } = null!;

    public TypeDocumentAggregate(Guid id, string name, string? description, string code, bool isActive) : this(id)
    {
        DomainGuard.IsNullOrEmpty(name, Errors.NameRequired);
        DomainGuard.IsNullOrEmpty(code, Errors.CodeRequired);

        Name = name;
        Description = description;
        Code = code;
        IsActive = isActive;

        CreatedAt = SystemClock.Instance.GetCurrentInstant();

        AddEvent(TypeDocumentCreatedDomainEvent.Create(Id, Name, Description, Code, IsActive));
    }

    public static TypeDocumentAggregate Create(Guid id, string name, string? description, string code, bool isActive)
    {
        return new TypeDocumentAggregate(id, name, description, code, isActive);
    }

    public void Update(string name, string? description, string code, bool isActive)
    {
        DomainGuard.IsNullOrEmpty(name, Errors.NameRequired);
        DomainGuard.IsNullOrEmpty(code, Errors.CodeRequired);

        Name = name;
        Description = description;
        Code = code;
        IsActive = isActive;

        UpdatedAt = SystemClock.Instance.GetCurrentInstant();
        
        AddEvent(TypeDocumentUpdatedDomainEvent.Create(Id, Name, Description, Code, IsActive));
    }

    public void Delete()
    {
        this.IsDeleted = true;
        this.IsActive = false;
        this.DeletedAt = SystemClock.Instance.GetCurrentInstant();
        
        AddEvent(TypeDocumentDeletedDomainEvent.Create(Id, Name, Description, Code, IsActive));
    }
}

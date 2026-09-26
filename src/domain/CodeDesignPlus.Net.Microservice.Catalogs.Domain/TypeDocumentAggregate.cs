namespace CodeDesignPlus.Net.Microservice.Catalogs.Domain;

public class TypeDocumentAggregate(Guid id) : AggregateRootBase(id)
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; } = null!;
    public string Code { get; private set; } = null!;

    /// <summary>
    /// Quien crea, edita o borra un tipo queda guardado (plan 051): antes <c>CreatedBy</c> quedaba vacio y
    /// <c>UpdatedBy</c> nulo, y no habia forma de saber quien habia cambiado el catalogo. La siembra usa el usuario de
    /// sistema.
    /// </summary>
    public TypeDocumentAggregate(Guid id, string name, string? description, string code, bool isActive, Guid createdBy) : this(id)
    {
        DomainGuard.IsNullOrEmpty(name, Errors.NameRequired);
        DomainGuard.IsNullOrEmpty(code, Errors.CodeRequired);
        DomainGuard.GuidIsEmpty(createdBy, Errors.UserRequired);

        Name = name;
        Description = description;
        Code = NormalizeCode(code);
        IsActive = isActive;

        CreatedAt = SystemClock.Instance.GetCurrentInstant();
        CreatedBy = createdBy;

        AddEvent(TypeDocumentCreatedDomainEvent.Create(Id, Name, Description, Code, IsActive));
    }

    public static TypeDocumentAggregate Create(Guid id, string name, string? description, string code, bool isActive, Guid createdBy)
    {
        return new TypeDocumentAggregate(id, name, description, code, isActive, createdBy);
    }

    public void Update(string name, string? description, string code, bool isActive, Guid updatedBy)
    {
        DomainGuard.IsNullOrEmpty(name, Errors.NameRequired);
        DomainGuard.IsNullOrEmpty(code, Errors.CodeRequired);
        DomainGuard.GuidIsEmpty(updatedBy, Errors.UserRequired);

        Name = name;
        Description = description;
        Code = NormalizeCode(code);
        IsActive = isActive;

        UpdatedAt = SystemClock.Instance.GetCurrentInstant();
        UpdatedBy = updatedBy;

        AddEvent(TypeDocumentUpdatedDomainEvent.Create(Id, Name, Description, Code, IsActive));
    }

    public void Delete(Guid deletedBy)
    {
        DomainGuard.GuidIsEmpty(deletedBy, Errors.UserRequired);

        this.IsDeleted = true;
        this.IsActive = false;
        this.DeletedAt = SystemClock.Instance.GetCurrentInstant();
        this.DeletedBy = deletedBy;

        AddEvent(TypeDocumentDeletedDomainEvent.Create(Id, Name, Description, Code, IsActive));
    }

    /// <summary>
    /// El codigo es la llave con la que los formularios guardan el tipo (<c>CC</c>, <c>NIT</c>), asi que se guarda sin
    /// espacios y en mayusculas: <c>cc</c> y <c>CC</c> serian dos tipos indistinguibles en pantalla (plan 049).
    /// </summary>
    public static string NormalizeCode(string code) => code.Trim().ToUpperInvariant();
}

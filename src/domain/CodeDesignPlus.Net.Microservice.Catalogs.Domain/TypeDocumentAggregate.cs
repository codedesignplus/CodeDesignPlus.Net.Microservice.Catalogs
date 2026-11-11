namespace CodeDesignPlus.Net.Microservice.Catalogs.Domain;

public class TypeDocumentAggregate(Guid id) : AggregateRootBase(id)
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; } = null!;
    public string Code { get; private set; } = null!;

    /// <summary>
    /// El pais del tipo, en ISO 3166-1 alfa-2 (<c>CO</c>). Los tipos de documento son un catalogo del pais: en Colombia,
    /// la tabla de la DIAN, con su codigo como <see cref="Code"/> (pendings/663).
    /// </summary>
    /// <remarks>
    /// Miembro suelto y no argumento del constructor (regla 31): los tipos guardados antes no lo traen y se leen con
    /// <c>null</c>.
    /// </remarks>
    public string? Country { get; private set; }

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

    public static TypeDocumentAggregate Create(Guid id, string name, string? description, string code, string country, bool isActive, Guid createdBy)
    {
        DomainGuard.IsNullOrWhiteSpace(country, Errors.CountryRequired);

        return new TypeDocumentAggregate(id, name, description, code, isActive, createdBy) { Country = NormalizeCountry(country) };
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

    /// <summary>
    /// El pais se guarda en mayusculas: <c>co</c> de la configuracion y <c>CO</c> son el mismo.
    /// </summary>
    public static string NormalizeCountry(string country) => country.Trim().ToUpperInvariant();
}

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
    /// La abreviatura con la que las pantallas muestran el tipo (<c>CC</c>, <c>NIT</c>, <c>PEP</c>): el <see cref="Code"/>
    /// es el de la DIAN (13, 31, 47) y no se lee de un vistazo (pendings/663). Antes la pantalla la tenia escrita en el
    /// codigo; ahora es un dato del catalogo y se administra con el.
    /// </summary>
    /// <remarks>
    /// Miembro suelto y no argumento del constructor (regla 31): los tipos guardados antes no la traen y se leen con
    /// <c>null</c>.
    /// </remarks>
    public string? Abbreviation { get; private set; }

    /// <summary>
    /// Longitud maxima de <see cref="Abbreviation"/>. Es un codigo corto con formato propio, fuera de las familias de
    /// texto libre de la regla 48: la mas larga de Colombia es <c>NITE</c>.
    /// </summary>
    public const int AbbreviationMaxLength = 10;

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

    public static TypeDocumentAggregate Create(Guid id, string name, string? description, string code, string abbreviation, string country, bool isActive, Guid createdBy)
    {
        DomainGuard.IsNullOrWhiteSpace(abbreviation, Errors.AbbreviationRequired);
        DomainGuard.IsNullOrWhiteSpace(country, Errors.CountryRequired);

        return new TypeDocumentAggregate(id, name, description, code, isActive, createdBy)
        {
            Abbreviation = NormalizeAbbreviation(abbreviation),
            Country = NormalizeCountry(country)
        };
    }

    public void Update(string name, string? description, string code, string abbreviation, bool isActive, Guid updatedBy)
    {
        DomainGuard.IsNullOrEmpty(name, Errors.NameRequired);
        DomainGuard.IsNullOrEmpty(code, Errors.CodeRequired);
        DomainGuard.IsNullOrWhiteSpace(abbreviation, Errors.AbbreviationRequired);
        DomainGuard.GuidIsEmpty(updatedBy, Errors.UserRequired);

        Name = name;
        Description = description;
        Code = NormalizeCode(code);
        Abbreviation = NormalizeAbbreviation(abbreviation);
        IsActive = isActive;

        UpdatedAt = SystemClock.Instance.GetCurrentInstant();
        UpdatedBy = updatedBy;

        AddEvent(TypeDocumentUpdatedDomainEvent.Create(Id, Name, Description, Code, IsActive));
    }

    /// <summary>
    /// Pone la abreviatura de la siembra a un tipo sembrado que no la tiene, porque se guardo antes de que existiera el
    /// campo (pendings/663). Completar solo anade (regla 25): si el tipo ya tiene abreviatura, la del usuario se queda.
    /// </summary>
    /// <returns><c>true</c> si la puso; <c>false</c> si el tipo ya tenia una.</returns>
    public bool CompleteAbbreviation(string abbreviation, Guid updatedBy)
    {
        DomainGuard.IsNullOrWhiteSpace(abbreviation, Errors.AbbreviationRequired);
        DomainGuard.GuidIsEmpty(updatedBy, Errors.UserRequired);

        if (!string.IsNullOrWhiteSpace(Abbreviation))
            return false;

        Abbreviation = NormalizeAbbreviation(abbreviation);
        UpdatedAt = SystemClock.Instance.GetCurrentInstant();
        UpdatedBy = updatedBy;

        return true;
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

    /// <summary>
    /// La abreviatura se guarda sin espacios alrededor; las mayusculas son de quien la escribe.
    /// </summary>
    public static string NormalizeAbbreviation(string abbreviation) => abbreviation.Trim();
}

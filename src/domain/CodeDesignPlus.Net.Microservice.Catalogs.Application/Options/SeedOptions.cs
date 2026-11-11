using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.Microservice.Catalogs.Application.Options;

/// <summary>
/// El pais del despliegue (pendings/663): elige la siembra de tipos de documento y es el pais de los tipos que se crean
/// desde la pantalla. Un despliegue, un pais.
/// </summary>
public class SeedOptions
{
    /// <summary>
    /// The configuration section.
    /// </summary>
    public static readonly string Section = "Seed";

    /// <summary>
    /// Gets or sets the country of the deployment in ISO 3166-1 alpha-2, e.g. "co".
    /// </summary>
    [Required]
    public string Country { get; set; } = null!;
}

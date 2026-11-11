using System.Reflection;
using JsonSerializer = System.Text.Json.JsonSerializer;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;
using Microsoft.Extensions.Hosting;

namespace CodeDesignPlus.Net.Microservice.Catalogs.Infrastructure.Seeds;

/// <summary>
/// Siembra los tipos de documento del pais del despliegue al arrancar.
///
/// <para><b>Un despliegue, un pais.</b> El pais sale de <c>Seed:Country</c> (<c>co</c> en <c>appsettings.json</c>) y
/// elige el fichero embebido <c>seed-&lt;pais&gt;-type-documents.json</c>. Si ese fichero no existe, el micro no
/// arranca: sembrar los tipos de otro pais, o ninguno, seria peor que no levantar (plan 049).</para>
///
/// <para><b>Se siembra por id, registro a registro, y nunca se toca lo que ya existe</b> (regla 25 de
/// <c>Microservices/rules/</c>). Se inserta cada tipo del fichero cuyo id no este en Mongo y cuyo codigo no use ya otro
/// tipo. Antes se comprobaba por conteo de activos: con uno desactivado se intentaba sembrar todo de nuevo, y con uno de
/// mas en Mongo lo nuevo del fichero no llegaba nunca.</para>
///
/// <para>Consecuencias: corregir un tipo que ya existe no se hace cambiando el fichero, sino desde la pantalla; un tipo
/// sembrado que se borra vuelve en el siguiente arranque (para retirarlo se desactiva, o se quita del fichero); y un
/// tipo que se quita del fichero no se borra de las bases donde ya estaba.</para>
///
/// <para>La unica excepcion es la abreviatura (pendings/663): un tipo sembrado que se guardo sin ella la recibe del
/// fichero. Es completar un campo vacio por la clave del tipo, no pisar nada: si ya tiene una, se queda la suya.</para>
///
/// <para><b>El codigo es el del pais</b> (pendings/663). En Colombia es el de la DIAN (13 cedula, 31 NIT...), el que
/// pide la factura electronica para el adquiriente, y el id de cada tipo es fijo en todos los entornos
/// (<c>c0d0c000-0000-0000-0000-0000000000NN</c>, con NN el codigo DIAN). ms-microsoftgraph resuelve el codigo que
/// llega del registro de Entra External ID contra esta misma lista, con los mismos ids.</para>
/// </summary>
public class CatalogSeedService(
    ITypeDocumentRepository typeDocumentRepository,
    IConfiguration configuration,
    ILogger<CatalogSeedService> logger
) : BackgroundService
{
    public static readonly Guid SystemUserId = Guid.Parse("10000000-0000-0000-0000-000000000001");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        // Fuera del try a proposito: un pais sin fichero debe tumbar el arranque, no quedarse en un aviso del log.
        var country = configuration["Seed:Country"];
        var data = LoadTypeDocuments(country);

        try
        {
            await SeedTypeDocumentsAsync(data, stoppingToken);

            logger.LogInformation("Catalog seed completed successfully ({Country}).", country);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding catalog data.");
        }
    }

    /// <summary>
    /// Lee los tipos de documento del pais indicado. Falla si no hay pais o si no existe su fichero.
    /// </summary>
    public static List<TypeDocumentSeed> LoadTypeDocuments(string? country)
    {
        if (string.IsNullOrWhiteSpace(country))
            throw new InvalidOperationException("Seed:Country no esta configurado: no se sabe de que pais sembrar los tipos de documento.");

        var fileName = $"seed-{country.Trim().ToLowerInvariant()}-type-documents.json";
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith(fileName))
            ?? throw new InvalidOperationException($"No existe {fileName}: el pais '{country}' no tiene tipos de documento para sembrar.");

        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        var types = JsonSerializer.Deserialize<List<TypeDocumentSeed>>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        // El fichero es el de un pais: cada tipo es de ese pais (pendings/663).
        var countryCode = TypeDocumentAggregate.NormalizeCountry(country);

        return [.. types.Select(t => t with { Country = countryCode })];
    }

    /// <summary>
    /// Inserta los tipos del fichero que falten, sin tocar los que ya existen.
    /// </summary>
    public async Task SeedTypeDocumentsAsync(List<TypeDocumentSeed> data, CancellationToken ct)
    {
        var inserted = 0;
        var completed = 0;

        foreach (var item in data)
        {
            try
            {
                if (await typeDocumentRepository.ExistsAsync<TypeDocumentAggregate>(item.Id, ct))
                {
                    if (await CompleteAbbreviationAsync(item, ct))
                        completed++;

                    continue;
                }

                if (await typeDocumentRepository.ExistsCodeAsync(item.Code, item.Id, ct))
                {
                    logger.LogWarning("Type document {Code} is not seeded: another type already uses that code.", item.Code);
                    continue;
                }

                var aggregate = TypeDocumentAggregate.Create(item.Id, item.Name, item.Description, item.Code, item.Abbreviation!, item.Country!, true, SystemUserId);
                await typeDocumentRepository.CreateAsync(aggregate, ct);
                inserted++;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to seed type document {Code}. Skipping.", item.Code);
            }
        }

        logger.LogInformation("Seeded {Inserted}/{Total} type documents.", inserted, data.Count);

        if (completed > 0)
            logger.LogInformation("Completed the abbreviation of {Completed} seeded type documents.", completed);
    }

    /// <summary>
    /// Un tipo sembrado antes de que existiera la abreviatura la recibe del fichero (pendings/663). Completar solo anade
    /// (regla 25): si ya tiene una, aunque sea otra, no se toca.
    /// </summary>
    private async Task<bool> CompleteAbbreviationAsync(TypeDocumentSeed item, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(item.Abbreviation))
            return false;

        var stored = await typeDocumentRepository.FindAsync<TypeDocumentAggregate>(item.Id, ct);

        if (stored is null || !stored.CompleteAbbreviation(item.Abbreviation, SystemUserId))
            return false;

        await typeDocumentRepository.UpdateAsync(stored, ct);

        return true;
    }
}

public record TypeDocumentSeed(Guid Id, string Name, string? Description, string Code, string? Abbreviation = null, string? Country = null);

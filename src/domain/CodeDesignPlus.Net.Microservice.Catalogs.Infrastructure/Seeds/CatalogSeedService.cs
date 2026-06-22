using System.Reflection;
using JsonSerializer = System.Text.Json.JsonSerializer;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;
using Microsoft.Extensions.Hosting;
using C = CodeDesignPlus.Net.Core.Abstractions.Models.Criteria;

namespace CodeDesignPlus.Net.Microservice.Catalogs.Infrastructure.Seeds;

public class CatalogSeedService(
    ITypeDocumentRepository typeDocumentRepository,
    ILogger<CatalogSeedService> logger
) : BackgroundService
{
    private static readonly Guid SystemUserId = Guid.Parse("10000000-0000-0000-0000-000000000001");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        try
        {
            await SeedTypeDocumentsAsync(stoppingToken);

            logger.LogInformation("Catalog seed completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding catalog data.");
        }
    }

    private async Task SeedTypeDocumentsAsync(CancellationToken ct)
    {
        var data = LoadResource<List<TypeDocumentSeed>>("seed-co-type-documents.json");
        var criteria = new C.Criteria { Filters = "IsActive=true", Limit = 1 };
        var existing = await typeDocumentRepository.MatchingAsync<TypeDocumentAggregate>(criteria, ct);

        if (existing.TotalCount >= data.Count)
        {
            logger.LogInformation("TypeDocuments already seeded ({Count}).", existing.TotalCount);
            return;
        }

        var inserted = 0;
        foreach (var item in data)
        {
            try
            {
                var aggregate = TypeDocumentAggregate.Create(item.Id, item.Name, item.Description, item.Code, true);
                await typeDocumentRepository.CreateAsync(aggregate, ct);
                inserted++;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to seed type document {Code}. Skipping.", item.Code);
            }
        }

        logger.LogInformation("Seeded {Inserted}/{Total} type documents.", inserted, data.Count);
    }

    private static T LoadResource<T>(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames().First(n => n.EndsWith(fileName));
        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        return JsonSerializer.Deserialize<T>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }
}

public record TypeDocumentSeed(Guid Id, string Name, string? Description, string Code);

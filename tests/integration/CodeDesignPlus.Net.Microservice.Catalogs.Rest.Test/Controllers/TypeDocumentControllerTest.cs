using System.Text.Json;
using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.PubSub.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace CodeDesignPlus.Net.Microservice.Catalogs.Rest.Test.Controllers;

/// <summary>
/// La abreviatura del tipo de documento contra el micro de verdad, con Mongo y Redis en contenedores (pendings/663): la API la recibe, la guarda, la devuelve, la exige, y la siembra la pone a los tipos de Colombia.
/// </summary>
public class TypeDocumentControllerTest : ServerBase<Program>, IClassFixture<Server<Program>>
{
    private static readonly JsonSerializerOptions Json = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true };

    public TypeDocumentControllerTest(Server<Program> server) : base(server)
    {
        server.InMemoryCollection = x =>
        {
            x.Add("Vault:Enable", "false");
            x.Add("Vault:Address", "http://localhost:8200");
            x.Add("Vault:Token", "root");
            x.Add("Solution", "CodeDesignPlus");
            x.Add("AppName", "my-test");
            x.Add("RabbitMQ:UserName", "guest");
            x.Add("RabbitMQ:Password", "guest");
            // En el cluster los eventos van por Azure Service Bus, que no tiene contenedor aqui: se apaga y los eventos
            // los recoge NoBus. Lo que se prueba es la API y Mongo, no el bus.
            x.Add("ServiceBus:Enable", "false");
            x.Add("Security:ValidAudiences:0", Guid.NewGuid().ToString());
        };
        server.ConfigureServices = services => services.AddSingleton<IPubSub, NoBus>();
    }

    private sealed class NoBus : IPubSub
    {
        public Task PublishAsync(IDomainEvent @event, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task PublishAsync(IReadOnlyList<IDomainEvent> @event, CancellationToken cancellationToken) => Task.CompletedTask;
    }

    [Fact]
    public async Task Create_StoresTheAbbreviation_AndGetByIdReturnsIt()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var created = await SendAsync(HttpMethod.Post, "api/TypeDocument", Body(id, "Carné diplomático", NewCode(), "CDI"));
        var read = await GetAsync(id);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, created.StatusCode);
        Assert.Equal("CDI", read.Abbreviation);
    }

    [Fact]
    public async Task Update_ChangesTheAbbreviation()
    {
        // Arrange
        var id = Guid.NewGuid();
        var code = NewCode();
        await SendAsync(HttpMethod.Post, "api/TypeDocument", Body(id, "Carné diplomático", code, "CDI"));

        // Act
        var updated = await SendAsync(HttpMethod.Put, $"api/TypeDocument/{id}", Body(id, "Carné diplomático", code, "CD"));
        var read = await GetAsync(id);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, updated.StatusCode);
        Assert.Equal("CD", read.Abbreviation);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("ABCDEFGHIJK")]
    public async Task Create_WithoutAValidAbbreviation_IsRejected(string? abbreviation)
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var response = await SendAsync(HttpMethod.Post, "api/TypeDocument", Body(id, "Carné diplomático", NewCode(), abbreviation));
        var problem = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Abbreviation", problem, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// La siembra de Colombia deja el NIT con su abreviatura. La siembra arranca a los 5 s del micro.
    /// </summary>
    [Fact]
    public async Task Seed_Colombia_GivesTheNitItsAbbreviation()
    {
        // Arrange
        var nit = Guid.Parse("c0d0c000-0000-0000-0000-000000000031");
        TypeDocumentDto? read = null;

        // Act
        for (var attempt = 0; attempt < 30 && read is null; attempt++)
        {
            var response = await SendAsync(HttpMethod.Get, $"api/TypeDocument/{nit}", null);

            if (response.IsSuccessStatusCode)
                read = JsonSerializer.Deserialize<TypeDocumentDto>(await response.Content.ReadAsStringAsync(), Json);
            else
                await Task.Delay(TimeSpan.FromSeconds(1));
        }

        // Assert
        Assert.NotNull(read);
        Assert.Equal(("31", "NIT", "CO"), (read!.Code, read.Abbreviation, read.Country));
    }

    private static string NewCode() => Random.Shared.Next(1000, 9999).ToString();

    private static StringContent Body(Guid id, string name, string code, string? abbreviation) =>
        new(JsonSerializer.Serialize(new { id, name, description = (string?)null, code, abbreviation, isActive = true }, Json), Encoding.UTF8, "application/json");

    private async Task<TypeDocumentDto> GetAsync(Guid id)
    {
        var response = await SendAsync(HttpMethod.Get, $"api/TypeDocument/{id}", null);
        var json = await response.Content.ReadAsStringAsync();

        Assert.True(response.IsSuccessStatusCode, json);

        return JsonSerializer.Deserialize<TypeDocumentDto>(json, Json)!;
    }

    private Task<HttpResponseMessage> SendAsync(HttpMethod method, string path, HttpContent? content)
    {
        var request = new HttpRequestMessage(method, new Uri($"http://localhost/{path}")) { Content = content };
        request.Headers.Authorization = new AuthenticationHeaderValue("TestAuth");

        return Client.SendAsync(request);
    }
}

using Microsoft.AspNetCore.Authorization;

namespace CodeDesignPlus.Net.Microservice.Catalogs.Rest.Controllers;

/// <summary>
/// Controller for managing the TypeDocuments.
/// </summary>
/// <param name="mediator">Mediator instance for sending commands and queries.</param>
/// <param name="mapper">Mapper instance for mapping between DTOs and commands/queries.</param>
[Route("api/[controller]")]
[ApiController]
public class TypeDocumentController(IMediator mediator, IMapper mapper) : ControllerBase
{
    /// <summary>
    /// Get all TypeDocuments.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of TypeDocuments.</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetTypeDocuments(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAllTypeDocumentQuery(), cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Get a TypeDocument by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the TypeDocument.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The TypeDocument.</returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTypeDocumentById(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetTypeDocumentByIdQuery(id), cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Create a new TypeDocument.
    /// </summary>
    /// <param name="data">Data for creating the TypeDocument.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP status code 204 (No Content).</returns>
    [HttpPost]
    public async Task<IActionResult> CreateTypeDocument([FromBody] CreateTypeDocumentDto data, CancellationToken cancellationToken)
    {
        await mediator.Send(mapper.Map<CreateTypeDocumentCommand>(data), cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Update an existing TypeDocument.
    /// </summary>
    /// <param name="id">The unique identifier of the TypeDocument.</param>
    /// <param name="data">Data for updating the TypeDocument.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP status code 204 (No Content).</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTypeDocument(Guid id, [FromBody] UpdateTypeDocumentDto data, CancellationToken cancellationToken)
    {
        data.Id = id;

        await mediator.Send(mapper.Map<UpdateTypeDocumentCommand>(data), cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Delete an existing TypeDocument.
    /// </summary>
    /// <param name="id">The unique identifier of the TypeDocument.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP status code 204 (No Content).</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTypeDocument(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteTypeDocumentCommand(id), cancellationToken);

        return NoContent();
    }
}
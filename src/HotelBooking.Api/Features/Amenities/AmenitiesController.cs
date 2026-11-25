using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Features.Amenities;

/// <summary>
/// CRUD-ендпоінти для зручностей. Бізнес-логіки тут немає: лише виклик сервісу і вибір статус-коду.
/// </summary>
[ApiController]
[Route("api/amenities")]
public class AmenitiesController(IAmenityService amenities) : ControllerBase
{
    /// <summary>GET /api/amenities — список усіх зручностей.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AmenityResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await amenities.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>GET /api/amenities/{id} — одна зручність або 404.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AmenityResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var amenity = await amenities.GetByIdAsync(id, cancellationToken);
        if (amenity is null)
        {
            return NotFound();
        }

        return amenity;
    }

    /// <summary>POST /api/amenities — створює зручність, повертає 201 і заголовок Location.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<AmenityResponse>> Create(
        CreateAmenityRequest request, CancellationToken cancellationToken)
    {
        var amenity = await amenities.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = amenity.Id }, amenity);
    }

    /// <summary>PUT /api/amenities/{id} — оновлює зручність: 204 або 404.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id, UpdateAmenityRequest request, CancellationToken cancellationToken)
    {
        var updated = await amenities.UpdateAsync(id, request, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    /// <summary>DELETE /api/amenities/{id} — видаляє зручність: 204 або 404.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await amenities.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}

using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Features.Hotels;

/// <summary>
/// CRUD-ендпоінти для готелів. Бізнес-логіки тут немає: лише виклик сервісу і вибір статус-коду.
/// </summary>
[ApiController]
[Route("api/hotels")]
public class HotelsController(IHotelService hotels) : ControllerBase
{
    /// <summary>GET /api/hotels — список усіх готелів.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<HotelResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await hotels.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>GET /api/hotels/{id} — один готель або 404.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HotelResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var hotel = await hotels.GetByIdAsync(id, cancellationToken);
        if (hotel is null)
        {
            return NotFound();
        }

        return hotel;
    }

    /// <summary>POST /api/hotels — створює готель, повертає 201 і заголовок Location.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<HotelResponse>> Create(
        CreateHotelRequest request, CancellationToken cancellationToken)
    {
        var hotel = await hotels.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = hotel.Id }, hotel);
    }

    /// <summary>PUT /api/hotels/{id} — оновлює готель: 204 або 404.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id, UpdateHotelRequest request, CancellationToken cancellationToken)
    {
        var updated = await hotels.UpdateAsync(id, request, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    /// <summary>DELETE /api/hotels/{id} — видаляє готель: 204 або 404.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await hotels.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}

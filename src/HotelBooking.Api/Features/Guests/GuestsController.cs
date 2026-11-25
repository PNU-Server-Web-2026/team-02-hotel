using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Features.Guests;

/// <summary>
/// CRUD-ендпоінти для гостей. Бізнес-логіки тут немає: лише виклик сервісу і вибір статус-коду.
/// </summary>
[ApiController]
[Route("api/guests")]
public class GuestsController(IGuestService guests) : ControllerBase
{
    /// <summary>GET /api/guests — список усіх гостей.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<GuestResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await guests.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>GET /api/guests/{id} — один гість або 404.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GuestResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var guest = await guests.GetByIdAsync(id, cancellationToken);
        if (guest is null)
        {
            return NotFound();
        }

        return guest;
    }

    /// <summary>POST /api/guests — створює гостя, повертає 201 і заголовок Location.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<GuestResponse>> Create(
        CreateGuestRequest request, CancellationToken cancellationToken)
    {
        var guest = await guests.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = guest.Id }, guest);
    }

    /// <summary>PUT /api/guests/{id} — оновлює гостя: 204 або 404.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id, UpdateGuestRequest request, CancellationToken cancellationToken)
    {
        var updated = await guests.UpdateAsync(id, request, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    /// <summary>DELETE /api/guests/{id} — видаляє гостя: 204 або 404.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await guests.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}

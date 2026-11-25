using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Features.Rooms;

/// <summary>
/// CRUD-ендпоінти для номерів. Бізнес-логіки тут немає: лише виклик сервісу і вибір статус-коду.
/// </summary>
[ApiController]
[Route("api/rooms")]
public class RoomsController(IRoomService rooms) : ControllerBase
{
    /// <summary>GET /api/rooms — список усіх номерів.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RoomResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await rooms.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>GET /api/rooms/{id} — один номер або 404.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoomResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var room = await rooms.GetByIdAsync(id, cancellationToken);
        if (room is null)
        {
            return NotFound();
        }

        return room;
    }

    /// <summary>POST /api/rooms — створює номер, повертає 201 і заголовок Location.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<RoomResponse>> Create(
        CreateRoomRequest request, CancellationToken cancellationToken)
    {
        var room = await rooms.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
    }

    /// <summary>PUT /api/rooms/{id} — оновлює номер: 204 або 404.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id, UpdateRoomRequest request, CancellationToken cancellationToken)
    {
        var updated = await rooms.UpdateAsync(id, request, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    /// <summary>DELETE /api/rooms/{id} — видаляє номер: 204 або 404.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await rooms.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}

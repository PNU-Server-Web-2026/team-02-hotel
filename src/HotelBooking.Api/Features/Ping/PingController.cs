using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Features.Ping;

/// <summary>
/// Найпростіший ендпоінт для перевірки, що API запущене.
/// </summary>
[ApiController]
[Route("api/ping")]
public class PingController : ControllerBase
{
    /// <summary>
    /// GET /api/ping — повертає "pong" і поточний час UTC.
    /// </summary>
    [HttpGet]
    public IActionResult Get() => Ok(new { message = "pong", utc = DateTime.UtcNow });
}

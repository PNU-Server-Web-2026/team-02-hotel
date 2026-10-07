using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Features.Info;

/// <summary>Відповідь ендпоінта /api/info.</summary>
public sealed record InfoResponse(string Name, string Version, string Environment, DateTime UtcNow);

/// <summary>
/// Службова інформація про API: назва, версія збірки, середовище, час сервера.
/// </summary>
[ApiController]
[Route("api/info")]
public class InfoController(IWebHostEnvironment env) : ControllerBase
{
    /// <summary>GET /api/info.</summary>
    [HttpGet]
    public ActionResult<InfoResponse> Get()
    {
        var assembly = typeof(Program).Assembly;

        var version = assembly
                          .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                      ?? assembly.GetName().Version?.ToString()
                      ?? "unknown";

        return new InfoResponse(
            assembly.GetName().Name ?? "HotelBooking.Api",
            version,
            env.EnvironmentName,
            DateTime.UtcNow);
    }
}

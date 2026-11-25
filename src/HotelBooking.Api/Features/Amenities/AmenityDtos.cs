namespace HotelBooking.Api.Features.Amenities;

/// <summary>
/// Тіло запиту POST /api/amenities. Id не передається: його призначає сервер.
/// </summary>
public sealed record CreateAmenityRequest(
    string Name,
    string? Icon);

/// <summary>
/// Тіло запиту PUT /api/amenities/{id}: повна заміна даних зручності.
/// </summary>
public sealed record UpdateAmenityRequest(
    string Name,
    string? Icon);

/// <summary>
/// Зручність номера (Wi-Fi, кондиціонер…) у відповідях API.
/// </summary>
public sealed record AmenityResponse(
    int Id,
    string Name,
    string? Icon);

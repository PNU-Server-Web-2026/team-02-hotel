namespace HotelBooking.Api.Features.Hotels;

/// <summary>
/// Тіло запиту POST /api/hotels. Id не передається: його призначає сервер.
/// </summary>
public sealed record CreateHotelRequest(
    string Name,
    string City,
    string? Address,
    int Stars,
    string? Description);

/// <summary>
/// Тіло запиту PUT /api/hotels/{id}: повна заміна даних готелю.
/// </summary>
public sealed record UpdateHotelRequest(
    string Name,
    string City,
    string? Address,
    int Stars,
    string? Description);

/// <summary>
/// Готель у відповідях API.
/// </summary>
public sealed record HotelResponse(
    int Id,
    string Name,
    string City,
    string? Address,
    int Stars,
    string? Description);

namespace HotelBooking.Api.Features.Guests;

/// <summary>
/// Тіло запиту POST /api/guests. Id не передається: його призначає сервер.
/// </summary>
public sealed record CreateGuestRequest(
    string FirstName,
    string LastName,
    string Email,
    string? Phone);

/// <summary>
/// Тіло запиту PUT /api/guests/{id}: повна заміна даних гостя.
/// </summary>
public sealed record UpdateGuestRequest(
    string FirstName,
    string LastName,
    string Email,
    string? Phone);

/// <summary>
/// Гість у відповідях API.
/// </summary>
public sealed record GuestResponse(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string? Phone);

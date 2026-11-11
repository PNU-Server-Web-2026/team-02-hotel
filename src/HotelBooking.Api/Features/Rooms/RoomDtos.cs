namespace HotelBooking.Api.Features.Rooms;

/// <summary>
/// Тіло запиту POST /api/rooms. Id не передається: його призначає сервер.
/// </summary>
public sealed record CreateRoomRequest(
    string Number,
    RoomType Type,
    int Capacity,
    decimal PricePerNight,
    int Floor);

/// <summary>
/// Тіло запиту PUT /api/rooms/{id}: повна заміна даних номера.
/// </summary>
public sealed record UpdateRoomRequest(
    string Number,
    RoomType Type,
    int Capacity,
    decimal PricePerNight,
    int Floor);

/// <summary>
/// Номер у відповідях API.
/// </summary>
public sealed record RoomResponse(
    int Id,
    string Number,
    RoomType Type,
    int Capacity,
    decimal PricePerNight,
    int Floor);

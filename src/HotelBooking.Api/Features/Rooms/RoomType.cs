using System.Text.Json.Serialization;

namespace HotelBooking.Api.Features.Rooms;

/// <summary>
/// Тип номера. У JSON передається рядком ("Double"), а не числом (1):
/// так API зрозуміліше, а додавання нового значення не зсуває старі.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<RoomType>))]
public enum RoomType
{
    Single,
    Double,
    Suite,
    Family,
}

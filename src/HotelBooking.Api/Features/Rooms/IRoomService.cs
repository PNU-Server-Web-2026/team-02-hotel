namespace HotelBooking.Api.Features.Rooms;

/// <summary>
/// Операції з номерами. Контролер працює лише з цим інтерфейсом,
/// тож реалізацію (in-memory зараз, EF Core пізніше) можна замінити без змін у контролері.
/// </summary>
public interface IRoomService
{
    /// <summary>Усі номери, впорядковані за Id.</summary>
    Task<IReadOnlyList<RoomResponse>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>Номер за Id або <c>null</c>, якщо його немає.</summary>
    Task<RoomResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>Створює номер і повертає його разом із новим Id.</summary>
    Task<RoomResponse> CreateAsync(CreateRoomRequest request, CancellationToken cancellationToken);

    /// <summary>Оновлює номер. <c>false</c>, якщо номера з таким Id немає.</summary>
    Task<bool> UpdateAsync(int id, UpdateRoomRequest request, CancellationToken cancellationToken);

    /// <summary>Видаляє номер. <c>false</c>, якщо номера з таким Id немає.</summary>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}

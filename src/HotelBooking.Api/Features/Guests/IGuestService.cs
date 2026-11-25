namespace HotelBooking.Api.Features.Guests;

/// <summary>
/// Операції з гостями. Контролер працює лише з цим інтерфейсом,
/// тож реалізацію (in-memory зараз, EF Core пізніше) можна замінити без змін у контролері.
/// </summary>
public interface IGuestService
{
    /// <summary>Усі гості, впорядковані за Id.</summary>
    Task<IReadOnlyList<GuestResponse>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>Гість за Id або <c>null</c>, якщо його немає.</summary>
    Task<GuestResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>Створює гостя і повертає його разом із новим Id.</summary>
    Task<GuestResponse> CreateAsync(CreateGuestRequest request, CancellationToken cancellationToken);

    /// <summary>Оновлює гостя. <c>false</c>, якщо гостя з таким Id немає.</summary>
    Task<bool> UpdateAsync(int id, UpdateGuestRequest request, CancellationToken cancellationToken);

    /// <summary>Видаляє гостя. <c>false</c>, якщо гостя з таким Id немає.</summary>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}

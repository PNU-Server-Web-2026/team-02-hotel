namespace HotelBooking.Api.Features.Hotels;

/// <summary>
/// Операції з готелями. Контролер працює лише з цим інтерфейсом,
/// тож реалізацію (in-memory зараз, EF Core пізніше) можна замінити без змін у контролері.
/// </summary>
public interface IHotelService
{
    /// <summary>Усі готелі, впорядковані за Id.</summary>
    Task<IReadOnlyList<HotelResponse>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>Готель за Id або <c>null</c>, якщо його немає.</summary>
    Task<HotelResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>Створює готель і повертає його разом із новим Id.</summary>
    Task<HotelResponse> CreateAsync(CreateHotelRequest request, CancellationToken cancellationToken);

    /// <summary>Оновлює готель. <c>false</c>, якщо готелю з таким Id немає.</summary>
    Task<bool> UpdateAsync(int id, UpdateHotelRequest request, CancellationToken cancellationToken);

    /// <summary>Видаляє готель. <c>false</c>, якщо готелю з таким Id немає.</summary>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}

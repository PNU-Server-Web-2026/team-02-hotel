namespace HotelBooking.Api.Features.Amenities;

/// <summary>
/// Операції зі зручностями. Контролер працює лише з цим інтерфейсом,
/// тож реалізацію (in-memory зараз, EF Core пізніше) можна замінити без змін у контролері.
/// </summary>
public interface IAmenityService
{
    /// <summary>Усі зручності, впорядковані за Id.</summary>
    Task<IReadOnlyList<AmenityResponse>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>Зручність за Id або <c>null</c>, якщо її немає.</summary>
    Task<AmenityResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>Створює зручність і повертає її разом із новим Id.</summary>
    Task<AmenityResponse> CreateAsync(CreateAmenityRequest request, CancellationToken cancellationToken);

    /// <summary>Оновлює зручність. <c>false</c>, якщо зручності з таким Id немає.</summary>
    Task<bool> UpdateAsync(int id, UpdateAmenityRequest request, CancellationToken cancellationToken);

    /// <summary>Видаляє зручність. <c>false</c>, якщо зручності з таким Id немає.</summary>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}

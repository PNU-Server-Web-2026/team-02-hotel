namespace HotelBooking.Api.Features.Reviews;

/// <summary>
/// Операції з відгуками. Контролер працює лише з цим інтерфейсом,
/// тож реалізацію (in-memory зараз, EF Core пізніше) можна замінити без змін у контролері.
/// </summary>
public interface IReviewService
{
    /// <summary>Усі відгуки, впорядковані за Id.</summary>
    Task<IReadOnlyList<ReviewResponse>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>Відгук за Id або <c>null</c>, якщо його немає.</summary>
    Task<ReviewResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>Створює відгук, встановлює CreatedAt і повертає його разом із новим Id.</summary>
    Task<ReviewResponse> CreateAsync(CreateReviewRequest request, CancellationToken cancellationToken);

    /// <summary>Оновлює відгук. <c>false</c>, якщо відгуку з таким Id немає.</summary>
    Task<bool> UpdateAsync(int id, UpdateReviewRequest request, CancellationToken cancellationToken);

    /// <summary>Видаляє відгук. <c>false</c>, якщо відгуку з таким Id немає.</summary>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}

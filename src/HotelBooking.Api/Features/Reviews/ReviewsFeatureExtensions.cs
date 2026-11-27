using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HotelBooking.Api.Features.Reviews;

/// <summary>
/// Реєстрація сервісів фічі Reviews.
/// </summary>
public static class ReviewsFeatureExtensions
{
    public static IServiceCollection AddReviewsFeature(this IServiceCollection services)
    {
        // Системний годинник для CreatedAt. TryAdd — щоб не перезаписати TimeProvider,
        // якщо його вже зареєструвала інша фіча або тест.
        services.TryAddSingleton(TimeProvider.System);

        // Singleton: in-memory сховище має жити весь час роботи застосунку
        // і бути спільним для всіх запитів. Зі Scoped кожен HTTP-запит
        // отримував би порожній словник і дані губилися б.
        services.AddSingleton<IReviewService, InMemoryReviewService>();

        return services;
    }
}

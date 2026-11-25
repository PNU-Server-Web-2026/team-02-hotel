namespace HotelBooking.Api.Features.Amenities;

/// <summary>
/// Реєстрація сервісів фічі Amenities.
/// </summary>
public static class AmenitiesFeatureExtensions
{
    public static IServiceCollection AddAmenitiesFeature(this IServiceCollection services)
    {
        // Singleton: in-memory сховище має жити весь час роботи застосунку
        // і бути спільним для всіх запитів. Зі Scoped кожен HTTP-запит
        // отримував би порожній словник і дані губилися б.
        services.AddSingleton<IAmenityService, InMemoryAmenityService>();

        return services;
    }
}

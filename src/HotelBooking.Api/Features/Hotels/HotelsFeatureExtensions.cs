namespace HotelBooking.Api.Features.Hotels;

/// <summary>
/// Реєстрація сервісів фічі Hotels.
/// </summary>
public static class HotelsFeatureExtensions
{
    public static IServiceCollection AddHotelsFeature(this IServiceCollection services)
    {
        // Singleton: in-memory сховище має жити весь час роботи застосунку
        // і бути спільним для всіх запитів. Зі Scoped кожен HTTP-запит
        // отримував би порожній словник і дані губилися б.
        services.AddSingleton<IHotelService, InMemoryHotelService>();

        return services;
    }
}

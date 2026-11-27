namespace HotelBooking.Api.Features.Guests;

/// <summary>
/// Реєстрація сервісів фічі Guests.
/// </summary>
public static class GuestsFeatureExtensions
{
    public static IServiceCollection AddGuestsFeature(this IServiceCollection services)
    {
        // Singleton: in-memory сховище має жити весь час роботи застосунку
        // і бути спільним для всіх запитів. Зі Scoped кожен HTTP-запит
        // отримував би порожній словник і дані губилися б.
        services.AddSingleton<IGuestService, InMemoryGuestService>();

        return services;
    }
}

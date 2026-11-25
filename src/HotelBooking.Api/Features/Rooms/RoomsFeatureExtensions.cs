namespace HotelBooking.Api.Features.Rooms;

/// <summary>
/// Реєстрація сервісів фічі Rooms.
/// </summary>
public static class RoomsFeatureExtensions
{
    public static IServiceCollection AddRoomsFeature(this IServiceCollection services)
    {
        // Singleton: in-memory сховище має жити весь час роботи застосунку
        // і бути спільним для всіх запитів. Зі Scoped кожен HTTP-запит
        // отримував би порожній словник і дані губилися б.
        services.AddSingleton<IRoomService, InMemoryRoomService>();

        return services;
    }
}

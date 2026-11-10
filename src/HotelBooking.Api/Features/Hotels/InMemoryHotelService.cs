using System.Collections.Concurrent;

namespace HotelBooking.Api.Features.Hotels;

/// <summary>
/// Тимчасова реалізація <see cref="IHotelService"/>: дані зберігаються в пам'яті процесу
/// і зникають після перезапуску. Буде замінена на EF Core у задачі #18.
/// </summary>
public sealed class InMemoryHotelService : IHotelService
{
    private readonly ConcurrentDictionary<int, HotelResponse> _hotels = new();
    private int _lastId;

    public Task<IReadOnlyList<HotelResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyList<HotelResponse> hotels = _hotels.Values.OrderBy(h => h.Id).ToList();
        return Task.FromResult(hotels);
    }

    public Task<HotelResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _hotels.TryGetValue(id, out var hotel);
        return Task.FromResult(hotel);
    }

    public Task<HotelResponse> CreateAsync(CreateHotelRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Interlocked.Increment атомарно збільшує лічильник:
        // два одночасні запити ніколи не отримають однаковий Id.
        var id = Interlocked.Increment(ref _lastId);

        var hotel = new HotelResponse(
            id, request.Name, request.City, request.Address, request.Stars, request.Description);

        _hotels[id] = hotel;
        return Task.FromResult(hotel);
    }

    public Task<bool> UpdateAsync(int id, UpdateHotelRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_hotels.TryGetValue(id, out var current))
        {
            return Task.FromResult(false);
        }

        var updated = current with
        {
            Name = request.Name,
            City = request.City,
            Address = request.Address,
            Stars = request.Stars,
            Description = request.Description,
        };

        // TryUpdate замінить значення, лише якщо його ніхто не змінив і не видалив
        // між читанням вище і цим рядком.
        return Task.FromResult(_hotels.TryUpdate(id, updated, current));
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(_hotels.TryRemove(id, out _));
    }
}

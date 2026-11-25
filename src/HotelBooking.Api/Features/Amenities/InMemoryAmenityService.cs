using System.Collections.Concurrent;

namespace HotelBooking.Api.Features.Amenities;

/// <summary>
/// Тимчасова реалізація <see cref="IAmenityService"/>: дані зберігаються в пам'яті процесу
/// і зникають після перезапуску. Буде замінена на EF Core окремою задачею.
/// </summary>
public sealed class InMemoryAmenityService : IAmenityService
{
    private readonly ConcurrentDictionary<int, AmenityResponse> _amenities = new();
    private int _lastId;

    public Task<IReadOnlyList<AmenityResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyList<AmenityResponse> amenities = _amenities.Values.OrderBy(a => a.Id).ToList();
        return Task.FromResult(amenities);
    }

    public Task<AmenityResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _amenities.TryGetValue(id, out var amenity);
        return Task.FromResult(amenity);
    }

    public Task<AmenityResponse> CreateAsync(CreateAmenityRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Interlocked.Increment атомарно збільшує лічильник:
        // два одночасні запити ніколи не отримають однаковий Id.
        var id = Interlocked.Increment(ref _lastId);

        var amenity = new AmenityResponse(id, request.Name, request.Icon);

        _amenities[id] = amenity;
        return Task.FromResult(amenity);
    }

    public Task<bool> UpdateAsync(int id, UpdateAmenityRequest request, CancellationToken cancellationToken)
    {
        // Оптимістичне оновлення: замінюємо значення, лише якщо його ніхто
        // не змінив між читанням і записом; інакше перечитуємо і пробуємо ще раз.
        // Цикл завершується, коли запис удався (true → 204)
        // або зручності вже немає (false → 404).
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!_amenities.TryGetValue(id, out var current))
            {
                return Task.FromResult(false);
            }

            var updated = current with
            {
                Name = request.Name,
                Icon = request.Icon,
            };

            if (_amenities.TryUpdate(id, updated, current))
            {
                return Task.FromResult(true);
            }

            // Інший запит устиг змінити цю зручність — це не означає, що її немає.
        }
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(_amenities.TryRemove(id, out _));
    }
}

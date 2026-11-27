using System.Collections.Concurrent;

namespace HotelBooking.Api.Features.Guests;

/// <summary>
/// Тимчасова реалізація <see cref="IGuestService"/>: дані зберігаються в пам'яті процесу
/// і зникають після перезапуску. Буде замінена на EF Core окремою задачею.
/// </summary>
public sealed class InMemoryGuestService : IGuestService
{
    private readonly ConcurrentDictionary<int, GuestResponse> _guests = new();
    private int _lastId;

    public Task<IReadOnlyList<GuestResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyList<GuestResponse> guests = _guests.Values.OrderBy(g => g.Id).ToList();
        return Task.FromResult(guests);
    }

    public Task<GuestResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _guests.TryGetValue(id, out var guest);
        return Task.FromResult(guest);
    }

    public Task<GuestResponse> CreateAsync(CreateGuestRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Interlocked.Increment атомарно збільшує лічильник:
        // два одночасні запити ніколи не отримають однаковий Id.
        var id = Interlocked.Increment(ref _lastId);

        var guest = new GuestResponse(id, request.FirstName, request.LastName, request.Email, request.Phone);

        _guests[id] = guest;
        return Task.FromResult(guest);
    }

    public Task<bool> UpdateAsync(int id, UpdateGuestRequest request, CancellationToken cancellationToken)
    {
        // Оптимістичне оновлення: замінюємо значення, лише якщо його ніхто
        // не змінив між читанням і записом; інакше перечитуємо і пробуємо ще раз.
        // Цикл завершується, коли запис удався (true → 204)
        // або гостя вже немає (false → 404).
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!_guests.TryGetValue(id, out var current))
            {
                return Task.FromResult(false);
            }

            var updated = current with
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
            };

            if (_guests.TryUpdate(id, updated, current))
            {
                return Task.FromResult(true);
            }

            // Інший запит устиг змінити цього гостя — це не означає, що гостя немає.
        }
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(_guests.TryRemove(id, out _));
    }
}

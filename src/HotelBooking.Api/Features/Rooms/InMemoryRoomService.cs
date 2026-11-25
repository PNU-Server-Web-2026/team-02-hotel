using System.Collections.Concurrent;

namespace HotelBooking.Api.Features.Rooms;

/// <summary>
/// Тимчасова реалізація <see cref="IRoomService"/>: дані зберігаються в пам'яті процесу
/// і зникають після перезапуску. Буде замінена на EF Core окремою задачею.
/// </summary>
public sealed class InMemoryRoomService : IRoomService
{
    private readonly ConcurrentDictionary<int, RoomResponse> _rooms = new();
    private int _lastId;

    public Task<IReadOnlyList<RoomResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyList<RoomResponse> rooms = _rooms.Values.OrderBy(r => r.Id).ToList();
        return Task.FromResult(rooms);
    }

    public Task<RoomResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _rooms.TryGetValue(id, out var room);
        return Task.FromResult(room);
    }

    public Task<RoomResponse> CreateAsync(CreateRoomRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Interlocked.Increment атомарно збільшує лічильник:
        // два одночасні запити ніколи не отримають однаковий Id.
        var id = Interlocked.Increment(ref _lastId);

        var room = new RoomResponse(
            id, request.Number, request.Type, request.Capacity, request.PricePerNight, request.Floor);

        _rooms[id] = room;
        return Task.FromResult(room);
    }

    public Task<bool> UpdateAsync(int id, UpdateRoomRequest request, CancellationToken cancellationToken)
    {
        // Оптимістичне оновлення: замінюємо значення, лише якщо його ніхто
        // не змінив між читанням і записом; інакше перечитуємо і пробуємо ще раз.
        // Цикл завершується, коли запис удався (true → 204)
        // або номера вже немає (false → 404).
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!_rooms.TryGetValue(id, out var current))
            {
                return Task.FromResult(false);
            }

            var updated = current with
            {
                Number = request.Number,
                Type = request.Type,
                Capacity = request.Capacity,
                PricePerNight = request.PricePerNight,
                Floor = request.Floor,
            };

            if (_rooms.TryUpdate(id, updated, current))
            {
                return Task.FromResult(true);
            }

            // Інший запит устиг змінити цей номер — це не означає, що номера немає.
        }
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(_rooms.TryRemove(id, out _));
    }
}

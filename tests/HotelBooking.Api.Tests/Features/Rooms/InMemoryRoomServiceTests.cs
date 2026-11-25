using HotelBooking.Api.Features.Rooms;

namespace HotelBooking.Api.Tests.Features.Rooms;

/// <summary>
/// Unit-тести in-memory сервісу без HTTP і без бази: так легше створити
/// справжню конкуренцію потоків за один і той самий номер.
/// </summary>
public class InMemoryRoomServiceTests
{
    [Fact]
    public async Task UpdateAsync_ConcurrentUpdatesOfSameRoom_AllSucceed()
    {
        var service = new InMemoryRoomService();
        var room = await service.CreateAsync(
            new CreateRoomRequest("101", RoomType.Single, 1, 1000m, 1), CancellationToken.None);

        const int updates = 1000;

        // Task.Run запускає оновлення в пулі потоків одночасно,
        // тож вони конкурують між TryGetValue і TryUpdate.
        var results = await Task.WhenAll(Enumerable.Range(1, updates).Select(i => Task.Run(() =>
            service.UpdateAsync(
                room.Id,
                new UpdateRoomRequest($"R{i}", RoomType.Double, 1 + (i % 8), 1000m + i, i % 10),
                CancellationToken.None))));

        // Номер ніхто не видаляв, тож жодне оновлення не має повернути false (404).
        Assert.DoesNotContain(false, results);

        var final = await service.GetByIdAsync(room.Id, CancellationToken.None);
        Assert.NotNull(final);
        Assert.StartsWith("R", final.Number);
    }

    [Fact]
    public async Task UpdateAsync_DeletedRoom_ReturnsFalse()
    {
        var service = new InMemoryRoomService();
        var room = await service.CreateAsync(
            new CreateRoomRequest("101", RoomType.Single, 1, 1000m, 1), CancellationToken.None);
        await service.DeleteAsync(room.Id, CancellationToken.None);

        var updated = await service.UpdateAsync(
            room.Id, new UpdateRoomRequest("102", RoomType.Suite, 3, 2000m, 2), CancellationToken.None);

        Assert.False(updated);
    }
}

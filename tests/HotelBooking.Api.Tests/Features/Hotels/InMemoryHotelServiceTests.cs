using HotelBooking.Api.Features.Hotels;

namespace HotelBooking.Api.Tests.Features.Hotels;

/// <summary>
/// Unit-тести in-memory сервісу без HTTP і без бази: так легше створити
/// справжню конкуренцію потоків за один і той самий готель.
/// </summary>
public class InMemoryHotelServiceTests
{
    [Fact]
    public async Task UpdateAsync_ConcurrentUpdatesOfSameHotel_AllSucceed()
    {
        var service = new InMemoryHotelService();
        var hotel = await service.CreateAsync(
            new CreateHotelRequest("Готель", "Львів", null, 3, null), CancellationToken.None);

        const int updates = 1000;

        // Task.Run запускає оновлення в пулі потоків одночасно,
        // тож вони конкурують між TryGetValue і TryUpdate.
        var results = await Task.WhenAll(Enumerable.Range(1, updates).Select(i => Task.Run(() =>
            service.UpdateAsync(
                hotel.Id,
                new UpdateHotelRequest($"Готель {i}", "Львів", null, 1 + (i % 5), null),
                CancellationToken.None))));

        // Готель ніхто не видаляв, тож жодне оновлення не має повернути false (404).
        Assert.DoesNotContain(false, results);

        // Переможе одне з оновлень — яке саме, залежить від планувальника потоків.
        var final = await service.GetByIdAsync(hotel.Id, CancellationToken.None);
        Assert.NotNull(final);
        Assert.StartsWith("Готель ", final.Name);
    }

    [Fact]
    public async Task UpdateAsync_DeletedHotel_ReturnsFalse()
    {
        var service = new InMemoryHotelService();
        var hotel = await service.CreateAsync(
            new CreateHotelRequest("Готель", "Львів", null, 3, null), CancellationToken.None);
        await service.DeleteAsync(hotel.Id, CancellationToken.None);

        var updated = await service.UpdateAsync(
            hotel.Id, new UpdateHotelRequest("Новий", "Київ", null, 4, null), CancellationToken.None);

        Assert.False(updated);
    }
}

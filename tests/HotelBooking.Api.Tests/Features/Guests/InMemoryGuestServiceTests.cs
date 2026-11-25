using HotelBooking.Api.Features.Guests;

namespace HotelBooking.Api.Tests.Features.Guests;

/// <summary>
/// Unit-тести in-memory сервісу без HTTP і без бази: так легше створити
/// справжню конкуренцію потоків за одного й того самого гостя.
/// </summary>
public class InMemoryGuestServiceTests
{
    [Fact]
    public async Task UpdateAsync_ConcurrentUpdatesOfSameGuest_AllSucceed()
    {
        var service = new InMemoryGuestService();
        var guest = await service.CreateAsync(
            new CreateGuestRequest("Іван", "Петренко", "ivan@example.com", null), CancellationToken.None);

        const int updates = 1000;

        // Task.Run запускає оновлення в пулі потоків одночасно,
        // тож вони конкурують між TryGetValue і TryUpdate.
        var results = await Task.WhenAll(Enumerable.Range(1, updates).Select(i => Task.Run(() =>
            service.UpdateAsync(
                guest.Id,
                new UpdateGuestRequest($"Іван {i}", "Петренко", $"ivan{i}@example.com", null),
                CancellationToken.None))));

        // Гостя ніхто не видаляв, тож жодне оновлення не має повернути false (404).
        Assert.DoesNotContain(false, results);

        var final = await service.GetByIdAsync(guest.Id, CancellationToken.None);
        Assert.NotNull(final);
        Assert.StartsWith("Іван ", final.FirstName);
    }

    [Fact]
    public async Task UpdateAsync_DeletedGuest_ReturnsFalse()
    {
        var service = new InMemoryGuestService();
        var guest = await service.CreateAsync(
            new CreateGuestRequest("Іван", "Петренко", "ivan@example.com", null), CancellationToken.None);
        await service.DeleteAsync(guest.Id, CancellationToken.None);

        var updated = await service.UpdateAsync(
            guest.Id, new UpdateGuestRequest("Олег", "Коваль", "oleh@example.com", null), CancellationToken.None);

        Assert.False(updated);
    }
}

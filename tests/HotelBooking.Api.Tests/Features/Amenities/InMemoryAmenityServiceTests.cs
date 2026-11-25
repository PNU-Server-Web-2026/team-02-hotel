using HotelBooking.Api.Features.Amenities;

namespace HotelBooking.Api.Tests.Features.Amenities;

/// <summary>
/// Unit-тести in-memory сервісу без HTTP і без бази: так легше створити
/// справжню конкуренцію потоків за одну й ту саму зручність.
/// </summary>
public class InMemoryAmenityServiceTests
{
    [Fact]
    public async Task UpdateAsync_ConcurrentUpdatesOfSameAmenity_AllSucceed()
    {
        var service = new InMemoryAmenityService();
        var amenity = await service.CreateAsync(
            new CreateAmenityRequest("Wi-Fi", "wifi"), CancellationToken.None);

        const int updates = 1000;

        // Task.Run запускає оновлення в пулі потоків одночасно,
        // тож вони конкурують між TryGetValue і TryUpdate.
        var results = await Task.WhenAll(Enumerable.Range(1, updates).Select(i => Task.Run(() =>
            service.UpdateAsync(
                amenity.Id,
                new UpdateAmenityRequest($"Wi-Fi {i}", $"wifi-{i}"),
                CancellationToken.None))));

        // Зручність ніхто не видаляв, тож жодне оновлення не має повернути false (404).
        Assert.DoesNotContain(false, results);

        var final = await service.GetByIdAsync(amenity.Id, CancellationToken.None);
        Assert.NotNull(final);
        Assert.StartsWith("Wi-Fi ", final.Name);
    }

    [Fact]
    public async Task UpdateAsync_DeletedAmenity_ReturnsFalse()
    {
        var service = new InMemoryAmenityService();
        var amenity = await service.CreateAsync(
            new CreateAmenityRequest("Wi-Fi", "wifi"), CancellationToken.None);
        await service.DeleteAsync(amenity.Id, CancellationToken.None);

        var updated = await service.UpdateAsync(
            amenity.Id, new UpdateAmenityRequest("Сейф", "safe"), CancellationToken.None);

        Assert.False(updated);
    }
}

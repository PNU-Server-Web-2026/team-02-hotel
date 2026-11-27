using HotelBooking.Api.Features.Reviews;

namespace HotelBooking.Api.Tests.Features.Reviews;

/// <summary>
/// Unit-тести in-memory сервісу без HTTP і без бази. Час підставляємо фіксований,
/// щоб перевіряти CreatedAt точно, а не «приблизно зараз».
/// </summary>
public class InMemoryReviewServiceTests
{
    private static readonly DateTimeOffset FixedNow = new(2026, 11, 25, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task CreateAsync_SetsCreatedAtFromTimeProvider()
    {
        var service = new InMemoryReviewService(new FixedTimeProvider(FixedNow));

        var review = await service.CreateAsync(new CreateReviewRequest(5, "Супер"), CancellationToken.None);

        Assert.Equal(FixedNow, review.CreatedAt);
    }

    [Fact]
    public async Task UpdateAsync_ConcurrentUpdatesOfSameReview_AllSucceed()
    {
        var service = new InMemoryReviewService(new FixedTimeProvider(FixedNow));
        var review = await service.CreateAsync(new CreateReviewRequest(3, "Початковий"), CancellationToken.None);

        const int updates = 1000;

        // Task.Run запускає оновлення в пулі потоків одночасно,
        // тож вони конкурують між TryGetValue і TryUpdate.
        var results = await Task.WhenAll(Enumerable.Range(1, updates).Select(i => Task.Run(() =>
            service.UpdateAsync(
                review.Id,
                new UpdateReviewRequest(1 + (i % 5), $"Коментар {i}"),
                CancellationToken.None))));

        // Відгук ніхто не видаляв, тож жодне оновлення не має повернути false (404).
        Assert.DoesNotContain(false, results);

        var final = await service.GetByIdAsync(review.Id, CancellationToken.None);
        Assert.NotNull(final);
        Assert.StartsWith("Коментар ", final.Comment);
        Assert.Equal(FixedNow, final.CreatedAt);
    }

    [Fact]
    public async Task UpdateAsync_DeletedReview_ReturnsFalse()
    {
        var service = new InMemoryReviewService(new FixedTimeProvider(FixedNow));
        var review = await service.CreateAsync(new CreateReviewRequest(3, "Початковий"), CancellationToken.None);
        await service.DeleteAsync(review.Id, CancellationToken.None);

        var updated = await service.UpdateAsync(
            review.Id, new UpdateReviewRequest(4, "Новий"), CancellationToken.None);

        Assert.False(updated);
    }

    /// <summary>Годинник, що завжди показує один і той самий час.</summary>
    private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }
}

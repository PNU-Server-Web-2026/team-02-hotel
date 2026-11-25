using System.Collections.Concurrent;

namespace HotelBooking.Api.Features.Reviews;

/// <summary>
/// Тимчасова реалізація <see cref="IReviewService"/>: дані зберігаються в пам'яті процесу
/// і зникають після перезапуску. Буде замінена на EF Core окремою задачею.
/// </summary>
/// <param name="timeProvider">
/// Джерело поточного часу для CreatedAt. Через абстракцію, а не DateTimeOffset.UtcNow,
/// щоб у тестах можна було підставити фіксований час.
/// </param>
public sealed class InMemoryReviewService(TimeProvider timeProvider) : IReviewService
{
    private readonly ConcurrentDictionary<int, ReviewResponse> _reviews = new();
    private int _lastId;

    public Task<IReadOnlyList<ReviewResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyList<ReviewResponse> reviews = _reviews.Values.OrderBy(r => r.Id).ToList();
        return Task.FromResult(reviews);
    }

    public Task<ReviewResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _reviews.TryGetValue(id, out var review);
        return Task.FromResult(review);
    }

    public Task<ReviewResponse> CreateAsync(CreateReviewRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Interlocked.Increment атомарно збільшує лічильник:
        // два одночасні запити ніколи не отримають однаковий Id.
        var id = Interlocked.Increment(ref _lastId);

        // CreatedAt встановлює сервер, а не клієнт.
        var review = new ReviewResponse(id, request.Rating, request.Comment, timeProvider.GetUtcNow());

        _reviews[id] = review;
        return Task.FromResult(review);
    }

    public Task<bool> UpdateAsync(int id, UpdateReviewRequest request, CancellationToken cancellationToken)
    {
        // Оптимістичне оновлення: замінюємо значення, лише якщо його ніхто
        // не змінив між читанням і записом; інакше перечитуємо і пробуємо ще раз.
        // Цикл завершується, коли запис удався (true → 204)
        // або відгуку вже немає (false → 404).
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!_reviews.TryGetValue(id, out var current))
            {
                return Task.FromResult(false);
            }

            // CreatedAt не входить у with, тож лишається часом створення.
            var updated = current with
            {
                Rating = request.Rating,
                Comment = request.Comment,
            };

            if (_reviews.TryUpdate(id, updated, current))
            {
                return Task.FromResult(true);
            }

            // Інший запит устиг змінити цей відгук — це не означає, що його немає.
        }
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(_reviews.TryRemove(id, out _));
    }
}

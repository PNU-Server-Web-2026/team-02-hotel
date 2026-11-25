namespace HotelBooking.Api.Features.Reviews;

/// <summary>
/// Тіло запиту POST /api/reviews. Id і CreatedAt не передаються: їх встановлює сервер.
/// </summary>
public sealed record CreateReviewRequest(
    int Rating,
    string? Comment);

/// <summary>
/// Тіло запиту PUT /api/reviews/{id}: заміна оцінки й коментаря. CreatedAt не змінюється.
/// </summary>
public sealed record UpdateReviewRequest(
    int Rating,
    string? Comment);

/// <summary>
/// Відгук у відповідях API.
/// </summary>
public sealed record ReviewResponse(
    int Id,
    int Rating,
    string? Comment,
    DateTimeOffset CreatedAt);

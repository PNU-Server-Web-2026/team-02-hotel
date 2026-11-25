using System.Net;
using System.Net.Http.Json;
using HotelBooking.Api.Features.Reviews;

namespace HotelBooking.Api.Tests.Features.Reviews;

/// <summary>
/// HTTP-тести контракту /api/reviews.
/// Сховище in-memory спільне для всіх тестів (Singleton), тому тести
/// не розраховують на порожній список і працюють лише з відгуками, які створили самі.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public class ReviewsApiTests(ApiFactory factory) : IntegrationTestBase(factory)
{
    private const int UnknownId = 999_999;

    [Fact]
    public async Task GetAll_ReturnsOkWithArray()
    {
        var created = await CreateReviewAsync("Чудово");

        var response = await Client.GetAsync("/api/reviews");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var reviews = await response.Content.ReadFromJsonAsync<List<ReviewResponse>>();
        Assert.NotNull(reviews);
        Assert.Contains(created, reviews);
    }

    [Fact]
    public async Task GetById_ExistingReview_ReturnsOk()
    {
        var created = await CreateReviewAsync("Добре");

        var response = await Client.GetAsync($"/api/reviews/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(created, await response.Content.ReadFromJsonAsync<ReviewResponse>());
    }

    [Fact]
    public async Task GetById_UnknownId_ReturnsNotFound()
    {
        var response = await Client.GetAsync($"/api/reviews/{UnknownId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsCreatedWithLocationAndServerCreatedAt()
    {
        var before = DateTimeOffset.UtcNow;
        var request = new CreateReviewRequest(5, "Чистий номер, привітний персонал");

        var response = await Client.PostAsJsonAsync("/api/reviews", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<ReviewResponse>();
        Assert.NotNull(created);
        Assert.True(created.Id > 0);
        Assert.Equal(5, created.Rating);
        Assert.Equal("Чистий номер, привітний персонал", created.Comment);

        // CreatedAt встановив сервер у момент запиту.
        Assert.InRange(created.CreatedAt, before.AddSeconds(-1), DateTimeOffset.UtcNow.AddSeconds(1));

        // Location має вести на щойно створений відгук.
        Assert.NotNull(response.Headers.Location);
        Assert.Equal($"/api/reviews/{created.Id}", response.Headers.Location.AbsolutePath);
        var byLocation = await Client.GetFromJsonAsync<ReviewResponse>(response.Headers.Location);
        Assert.Equal(created, byLocation);
    }

    [Fact]
    public async Task Update_ExistingReview_ReturnsNoContentAndKeepsCreatedAt()
    {
        var created = await CreateReviewAsync("Було так собі");
        var request = new UpdateReviewRequest(4, "Після повторного візиту — краще");

        var response = await Client.PutAsJsonAsync($"/api/reviews/{created.Id}", request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var updated = await Client.GetFromJsonAsync<ReviewResponse>($"/api/reviews/{created.Id}");
        Assert.Equal(
            new ReviewResponse(created.Id, 4, "Після повторного візиту — краще", created.CreatedAt),
            updated);
    }

    [Fact]
    public async Task Update_UnknownId_ReturnsNotFound()
    {
        var request = new UpdateReviewRequest(3, null);

        var response = await Client.PutAsJsonAsync($"/api/reviews/{UnknownId}", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingReview_ReturnsNoContentAndReviewIsGone()
    {
        var created = await CreateReviewAsync("На видалення");

        var response = await Client.DeleteAsync($"/api/reviews/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var afterDelete = await Client.GetAsync($"/api/reviews/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, afterDelete.StatusCode);
    }

    [Fact]
    public async Task Delete_UnknownId_ReturnsNotFound()
    {
        var response = await Client.DeleteAsync($"/api/reviews/{UnknownId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<ReviewResponse> CreateReviewAsync(string comment)
    {
        var request = new CreateReviewRequest(3, comment);
        var response = await Client.PostAsJsonAsync("/api/reviews", request);
        response.EnsureSuccessStatusCode();

        var review = await response.Content.ReadFromJsonAsync<ReviewResponse>();
        Assert.NotNull(review);
        return review;
    }
}

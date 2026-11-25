using System.Net;
using System.Net.Http.Json;
using HotelBooking.Api.Features.Guests;

namespace HotelBooking.Api.Tests.Features.Guests;

/// <summary>
/// HTTP-тести контракту /api/guests.
/// Сховище in-memory спільне для всіх тестів (Singleton), тому тести
/// не розраховують на порожній список і працюють лише з гостями, яких створили самі.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public class GuestsApiTests(ApiFactory factory) : IntegrationTestBase(factory)
{
    private const int UnknownId = 999_999;

    [Fact]
    public async Task GetAll_ReturnsOkWithArray()
    {
        var created = await CreateGuestAsync("Іван");

        var response = await Client.GetAsync("/api/guests");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var guests = await response.Content.ReadFromJsonAsync<List<GuestResponse>>();
        Assert.NotNull(guests);
        Assert.Contains(created, guests);
    }

    [Fact]
    public async Task GetById_ExistingGuest_ReturnsOk()
    {
        var created = await CreateGuestAsync("Олена");

        var response = await Client.GetAsync($"/api/guests/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(created, await response.Content.ReadFromJsonAsync<GuestResponse>());
    }

    [Fact]
    public async Task GetById_UnknownId_ReturnsNotFound()
    {
        var response = await Client.GetAsync($"/api/guests/{UnknownId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsCreatedWithLocation()
    {
        var request = new CreateGuestRequest("Тарас", "Шевченко", "taras@example.com", "+380501234567");

        var response = await Client.PostAsJsonAsync("/api/guests", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<GuestResponse>();
        Assert.NotNull(created);
        Assert.True(created.Id > 0);
        Assert.Equal(
            new GuestResponse(created.Id, "Тарас", "Шевченко", "taras@example.com", "+380501234567"),
            created);

        // Location має вести на щойно створеного гостя.
        Assert.NotNull(response.Headers.Location);
        Assert.Equal($"/api/guests/{created.Id}", response.Headers.Location.AbsolutePath);
        var byLocation = await Client.GetFromJsonAsync<GuestResponse>(response.Headers.Location);
        Assert.Equal(created, byLocation);
    }

    [Fact]
    public async Task Update_ExistingGuest_ReturnsNoContentAndChangesData()
    {
        var created = await CreateGuestAsync("Старе");
        var request = new UpdateGuestRequest("Нове", "Прізвище", "new@example.com", "+380671112233");

        var response = await Client.PutAsJsonAsync($"/api/guests/{created.Id}", request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var updated = await Client.GetFromJsonAsync<GuestResponse>($"/api/guests/{created.Id}");
        Assert.Equal(
            new GuestResponse(created.Id, "Нове", "Прізвище", "new@example.com", "+380671112233"),
            updated);
    }

    [Fact]
    public async Task Update_UnknownId_ReturnsNotFound()
    {
        var request = new UpdateGuestRequest("Ніхто", "Ніхто", "nobody@example.com", null);

        var response = await Client.PutAsJsonAsync($"/api/guests/{UnknownId}", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingGuest_ReturnsNoContentAndGuestIsGone()
    {
        var created = await CreateGuestAsync("На видалення");

        var response = await Client.DeleteAsync($"/api/guests/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var afterDelete = await Client.GetAsync($"/api/guests/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, afterDelete.StatusCode);
    }

    [Fact]
    public async Task Delete_UnknownId_ReturnsNotFound()
    {
        var response = await Client.DeleteAsync($"/api/guests/{UnknownId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<GuestResponse> CreateGuestAsync(string firstName)
    {
        var request = new CreateGuestRequest(firstName, "Петренко", $"{Guid.NewGuid():N}@example.com", null);
        var response = await Client.PostAsJsonAsync("/api/guests", request);
        response.EnsureSuccessStatusCode();

        var guest = await response.Content.ReadFromJsonAsync<GuestResponse>();
        Assert.NotNull(guest);
        return guest;
    }
}

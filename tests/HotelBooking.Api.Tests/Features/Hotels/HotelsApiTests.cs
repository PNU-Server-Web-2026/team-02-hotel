using System.Net;
using System.Net.Http.Json;
using HotelBooking.Api.Features.Hotels;

namespace HotelBooking.Api.Tests.Features.Hotels;

/// <summary>
/// HTTP-тести контракту /api/hotels.
/// Сховище in-memory спільне для всіх тестів (Singleton), тому тести
/// не розраховують на порожній список і працюють лише з готелями, які створили самі.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public class HotelsApiTests(ApiFactory factory) : IntegrationTestBase(factory)
{
    private const int UnknownId = 999_999;

    [Fact]
    public async Task GetAll_ReturnsOkWithArray()
    {
        var created = await CreateHotelAsync("Карпати");

        var response = await Client.GetAsync("/api/hotels");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var hotels = await response.Content.ReadFromJsonAsync<List<HotelResponse>>();
        Assert.NotNull(hotels);
        Assert.Contains(created, hotels);
    }

    [Fact]
    public async Task GetById_ExistingHotel_ReturnsOk()
    {
        var created = await CreateHotelAsync("Говерла");

        var response = await Client.GetAsync($"/api/hotels/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(created, await response.Content.ReadFromJsonAsync<HotelResponse>());
    }

    [Fact]
    public async Task GetById_UnknownId_ReturnsNotFound()
    {
        var response = await Client.GetAsync($"/api/hotels/{UnknownId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsCreatedWithLocation()
    {
        var request = new CreateHotelRequest("Буковель", "Яремче", "вул. Свободи, 1", 4, "Біля підйомників");

        var response = await Client.PostAsJsonAsync("/api/hotels", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<HotelResponse>();
        Assert.NotNull(created);
        Assert.True(created.Id > 0);
        Assert.Equal(request.Name, created.Name);
        Assert.Equal(request.City, created.City);
        Assert.Equal(request.Address, created.Address);
        Assert.Equal(request.Stars, created.Stars);
        Assert.Equal(request.Description, created.Description);

        // Location має вести на щойно створений готель.
        Assert.NotNull(response.Headers.Location);
        Assert.Equal($"/api/hotels/{created.Id}", response.Headers.Location.AbsolutePath);
        var byLocation = await Client.GetFromJsonAsync<HotelResponse>(response.Headers.Location);
        Assert.Equal(created, byLocation);
    }

    [Fact]
    public async Task Update_ExistingHotel_ReturnsNoContentAndChangesData()
    {
        var created = await CreateHotelAsync("Старий");
        var request = new UpdateHotelRequest("Новий", "Львів", "пл. Ринок, 1", 5, "Оновлено");

        var response = await Client.PutAsJsonAsync($"/api/hotels/{created.Id}", request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var updated = await Client.GetFromJsonAsync<HotelResponse>($"/api/hotels/{created.Id}");
        Assert.Equal(
            new HotelResponse(created.Id, "Новий", "Львів", "пл. Ринок, 1", 5, "Оновлено"),
            updated);
    }

    [Fact]
    public async Task Update_UnknownId_ReturnsNotFound()
    {
        var request = new UpdateHotelRequest("Ніде", "Ніде", null, 3, null);

        var response = await Client.PutAsJsonAsync($"/api/hotels/{UnknownId}", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingHotel_ReturnsNoContentAndHotelIsGone()
    {
        var created = await CreateHotelAsync("На видалення");

        var response = await Client.DeleteAsync($"/api/hotels/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var afterDelete = await Client.GetAsync($"/api/hotels/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, afterDelete.StatusCode);
    }

    [Fact]
    public async Task Delete_UnknownId_ReturnsNotFound()
    {
        var response = await Client.DeleteAsync($"/api/hotels/{UnknownId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<HotelResponse> CreateHotelAsync(string name)
    {
        var request = new CreateHotelRequest(name, "Івано-Франківськ", null, 3, null);
        var response = await Client.PostAsJsonAsync("/api/hotels", request);
        response.EnsureSuccessStatusCode();

        var hotel = await response.Content.ReadFromJsonAsync<HotelResponse>();
        Assert.NotNull(hotel);
        return hotel;
    }
}

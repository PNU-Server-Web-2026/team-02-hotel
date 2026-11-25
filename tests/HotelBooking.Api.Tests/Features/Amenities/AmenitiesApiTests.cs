using System.Net;
using System.Net.Http.Json;
using HotelBooking.Api.Features.Amenities;

namespace HotelBooking.Api.Tests.Features.Amenities;

/// <summary>
/// HTTP-тести контракту /api/amenities.
/// Сховище in-memory спільне для всіх тестів (Singleton), тому тести
/// не розраховують на порожній список і працюють лише зі зручностями, які створили самі.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public class AmenitiesApiTests(ApiFactory factory) : IntegrationTestBase(factory)
{
    private const int UnknownId = 999_999;

    [Fact]
    public async Task GetAll_ReturnsOkWithArray()
    {
        var created = await CreateAmenityAsync("Wi-Fi");

        var response = await Client.GetAsync("/api/amenities");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var amenities = await response.Content.ReadFromJsonAsync<List<AmenityResponse>>();
        Assert.NotNull(amenities);
        Assert.Contains(created, amenities);
    }

    [Fact]
    public async Task GetById_ExistingAmenity_ReturnsOk()
    {
        var created = await CreateAmenityAsync("Кондиціонер");

        var response = await Client.GetAsync($"/api/amenities/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(created, await response.Content.ReadFromJsonAsync<AmenityResponse>());
    }

    [Fact]
    public async Task GetById_UnknownId_ReturnsNotFound()
    {
        var response = await Client.GetAsync($"/api/amenities/{UnknownId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsCreatedWithLocation()
    {
        var request = new CreateAmenityRequest("Міні-бар", "minibar");

        var response = await Client.PostAsJsonAsync("/api/amenities", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<AmenityResponse>();
        Assert.NotNull(created);
        Assert.True(created.Id > 0);
        Assert.Equal(new AmenityResponse(created.Id, "Міні-бар", "minibar"), created);

        // Location має вести на щойно створену зручність.
        Assert.NotNull(response.Headers.Location);
        Assert.Equal($"/api/amenities/{created.Id}", response.Headers.Location.AbsolutePath);
        var byLocation = await Client.GetFromJsonAsync<AmenityResponse>(response.Headers.Location);
        Assert.Equal(created, byLocation);
    }

    [Fact]
    public async Task Update_ExistingAmenity_ReturnsNoContentAndChangesData()
    {
        var created = await CreateAmenityAsync("Стара назва");
        var request = new UpdateAmenityRequest("Сейф", "safe");

        var response = await Client.PutAsJsonAsync($"/api/amenities/{created.Id}", request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var updated = await Client.GetFromJsonAsync<AmenityResponse>($"/api/amenities/{created.Id}");
        Assert.Equal(new AmenityResponse(created.Id, "Сейф", "safe"), updated);
    }

    [Fact]
    public async Task Update_UnknownId_ReturnsNotFound()
    {
        var request = new UpdateAmenityRequest("Нічого", null);

        var response = await Client.PutAsJsonAsync($"/api/amenities/{UnknownId}", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingAmenity_ReturnsNoContentAndAmenityIsGone()
    {
        var created = await CreateAmenityAsync("На видалення");

        var response = await Client.DeleteAsync($"/api/amenities/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var afterDelete = await Client.GetAsync($"/api/amenities/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, afterDelete.StatusCode);
    }

    [Fact]
    public async Task Delete_UnknownId_ReturnsNotFound()
    {
        var response = await Client.DeleteAsync($"/api/amenities/{UnknownId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<AmenityResponse> CreateAmenityAsync(string name)
    {
        var request = new CreateAmenityRequest(name, null);
        var response = await Client.PostAsJsonAsync("/api/amenities", request);
        response.EnsureSuccessStatusCode();

        var amenity = await response.Content.ReadFromJsonAsync<AmenityResponse>();
        Assert.NotNull(amenity);
        return amenity;
    }
}

using System.Net;
using System.Net.Http.Json;
using HotelBooking.Api.Features.Rooms;

namespace HotelBooking.Api.Tests.Features.Rooms;

/// <summary>
/// HTTP-тести контракту /api/rooms.
/// Сховище in-memory спільне для всіх тестів (Singleton), тому тести
/// не розраховують на порожній список і працюють лише з номерами, які створили самі.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public class RoomsApiTests(ApiFactory factory) : IntegrationTestBase(factory)
{
    private const int UnknownId = 999_999;

    [Fact]
    public async Task GetAll_ReturnsOkWithArray()
    {
        var created = await CreateRoomAsync("101");

        var response = await Client.GetAsync("/api/rooms");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var rooms = await response.Content.ReadFromJsonAsync<List<RoomResponse>>();
        Assert.NotNull(rooms);
        Assert.Contains(created, rooms);
    }

    [Fact]
    public async Task GetById_ExistingRoom_ReturnsOk()
    {
        var created = await CreateRoomAsync("102");

        var response = await Client.GetAsync($"/api/rooms/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(created, await response.Content.ReadFromJsonAsync<RoomResponse>());
    }

    [Fact]
    public async Task GetById_UnknownId_ReturnsNotFound()
    {
        var response = await Client.GetAsync($"/api/rooms/{UnknownId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsCreatedWithLocation()
    {
        var request = new CreateRoomRequest("305", RoomType.Suite, 4, 2500.50m, 3);

        var response = await Client.PostAsJsonAsync("/api/rooms", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<RoomResponse>();
        Assert.NotNull(created);
        Assert.True(created.Id > 0);
        Assert.Equal(
            new RoomResponse(created.Id, "305", RoomType.Suite, 4, 2500.50m, 3),
            created);

        // Location має вести на щойно створений номер.
        Assert.NotNull(response.Headers.Location);
        Assert.Equal($"/api/rooms/{created.Id}", response.Headers.Location.AbsolutePath);
        var byLocation = await Client.GetFromJsonAsync<RoomResponse>(response.Headers.Location);
        Assert.Equal(created, byLocation);
    }

    [Fact]
    public async Task Create_TypeIsSerializedAsString()
    {
        var response = await Client.PostAsJsonAsync(
            "/api/rooms", new CreateRoomRequest("410", RoomType.Family, 5, 3000m, 4));

        var json = await response.Content.ReadAsStringAsync();

        Assert.Contains("\"type\":\"Family\"", json);
    }

    [Fact]
    public async Task Update_ExistingRoom_ReturnsNoContentAndChangesData()
    {
        var created = await CreateRoomAsync("201");
        var request = new UpdateRoomRequest("201A", RoomType.Double, 2, 1800m, 2);

        var response = await Client.PutAsJsonAsync($"/api/rooms/{created.Id}", request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var updated = await Client.GetFromJsonAsync<RoomResponse>($"/api/rooms/{created.Id}");
        Assert.Equal(new RoomResponse(created.Id, "201A", RoomType.Double, 2, 1800m, 2), updated);
    }

    [Fact]
    public async Task Update_UnknownId_ReturnsNotFound()
    {
        var request = new UpdateRoomRequest("000", RoomType.Single, 1, 100m, 0);

        var response = await Client.PutAsJsonAsync($"/api/rooms/{UnknownId}", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingRoom_ReturnsNoContentAndRoomIsGone()
    {
        var created = await CreateRoomAsync("B1");

        var response = await Client.DeleteAsync($"/api/rooms/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var afterDelete = await Client.GetAsync($"/api/rooms/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, afterDelete.StatusCode);
    }

    [Fact]
    public async Task Delete_UnknownId_ReturnsNotFound()
    {
        var response = await Client.DeleteAsync($"/api/rooms/{UnknownId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<RoomResponse> CreateRoomAsync(string number)
    {
        var request = new CreateRoomRequest(number, RoomType.Single, 1, 1200m, 1);
        var response = await Client.PostAsJsonAsync("/api/rooms", request);
        response.EnsureSuccessStatusCode();

        var room = await response.Content.ReadFromJsonAsync<RoomResponse>();
        Assert.NotNull(room);
        return room;
    }
}

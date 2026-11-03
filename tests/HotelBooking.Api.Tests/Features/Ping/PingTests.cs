using System.Net;
using System.Net.Http.Json;

namespace HotelBooking.Api.Tests.Features.Ping;

public class PingTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Get_ReturnsOkWithPong()
    {
        var response = await _client.GetAsync("/api/ping");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<PingResponse>();
        Assert.NotNull(body);
        Assert.Equal("pong", body.Message);
    }

    private sealed record PingResponse(string Message, DateTime Utc);
}

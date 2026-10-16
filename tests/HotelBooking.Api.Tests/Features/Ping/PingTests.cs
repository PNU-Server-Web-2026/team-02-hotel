using System.Net;
using System.Net.Http.Json;

namespace HotelBooking.Api.Tests.Features.Ping;

[Collection(IntegrationTestCollection.Name)]
public class PingTests(ApiFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task Get_ReturnsOkWithPong()
    {
        var response = await Client.GetAsync("/api/ping");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<PingResponse>();
        Assert.NotNull(body);
        Assert.Equal("pong", body.Message);
    }

    private sealed record PingResponse(string Message, DateTime Utc);
}

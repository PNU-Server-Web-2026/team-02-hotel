using System.Net;
using System.Net.Http.Json;
using HotelBooking.Api.Features.Info;

namespace HotelBooking.Api.Tests.Features.Info;

public class InfoTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Get_ReturnsApiInfo()
    {
        var response = await _client.GetAsync("/api/info");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<InfoResponse>();
        Assert.NotNull(body);
        Assert.Equal("HotelBooking.Api", body.Name);
        Assert.False(string.IsNullOrWhiteSpace(body.Version));
        Assert.Equal("Testing", body.Environment);
        Assert.InRange(body.UtcNow, DateTime.UtcNow.AddMinutes(-1), DateTime.UtcNow.AddMinutes(1));
    }
}

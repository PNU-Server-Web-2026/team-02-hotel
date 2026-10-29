using System.Net;
using System.Net.Http.Json;
using HotelBooking.Api.Features.Info;

namespace HotelBooking.Api.Tests.Features.Info;

[Collection(IntegrationTestCollection.Name)]
public class InfoTests(ApiFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task Get_ReturnsApiInfo()
    {
        var response = await Client.GetAsync("/api/info");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<InfoResponse>();
        Assert.NotNull(body);
        Assert.Equal("HotelBooking.Api", body.Name);
        Assert.False(string.IsNullOrWhiteSpace(body.Version));
        Assert.Equal("Testing", body.Environment);
        Assert.InRange(body.UtcNow, DateTime.UtcNow.AddMinutes(-1), DateTime.UtcNow.AddMinutes(1));
    }
}

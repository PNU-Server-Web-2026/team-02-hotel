using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HotelBooking.Api.Tests.Features.Health;

[Collection(IntegrationTestCollection.Name)]
public class HealthTests(ApiFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task Get_Health_ReturnsOkWithoutChecks()
    {
        var response = await Client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

        var body = await response.Content.ReadFromJsonAsync<HealthBody>();
        Assert.NotNull(body);
        Assert.Equal("Healthy", body.Status);
        Assert.Empty(body.Checks);
    }

    [Fact]
    public async Task Get_Ready_WhenDatabaseIsAvailable_ReturnsOkWithDatabaseCheck()
    {
        var response = await Client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<HealthBody>();
        Assert.NotNull(body);
        Assert.Equal("Healthy", body.Status);
        var check = Assert.Single(body.Checks);
        Assert.Equal("database", check.Name);
        Assert.Equal("Healthy", check.Status);
    }
}

public sealed class UnavailableDatabaseApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting(
            "ConnectionStrings:Default",
            "Host=127.0.0.1;Port=1;Database=hotel_booking;Username=postgres;Password=postgres;Timeout=1;Command Timeout=1");
    }
}

public class HealthReadyUnavailableTests(UnavailableDatabaseApiFactory factory)
    : IClassFixture<UnavailableDatabaseApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Get_Health_WhenDatabaseIsDown_ReturnsOk()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_Ready_WhenDatabaseIsDown_ReturnsServiceUnavailable()
    {
        var response = await _client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

        var body = await response.Content.ReadFromJsonAsync<HealthBody>();
        Assert.NotNull(body);
        Assert.Equal("Unhealthy", body.Status);
        var check = Assert.Single(body.Checks);
        Assert.Equal("database", check.Name);
        Assert.Equal("Unhealthy", check.Status);
    }
}

internal sealed record HealthBody(string Status, IReadOnlyList<HealthCheckItem> Checks);

internal sealed record HealthCheckItem(string Name, string Status);

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using HotelBooking.Api.Features.Booking;

namespace HotelBooking.Api.Tests.Features.Booking;

public class BookingOptionsTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    [Fact]
    public void Options_AreBoundFromConfiguration()
    {
        var options = factory.Services.GetRequiredService<IOptions<BookingOptions>>().Value;

        Assert.Equal(30, options.MaxNights);
        Assert.Equal(30, options.PendingTimeoutMinutes);
        Assert.Equal(48, options.FreeCancellationHours);
        Assert.Equal(7, options.LongStayNights);
        Assert.Equal(10, options.LongStayDiscountPercent);
    }

    [Fact]
    public void Options_CanBeOverriddenByConfiguration()
    {
        using var custom = factory.WithWebHostBuilder(b => b.UseSetting("Booking:MaxNights", "14"));

        var options = custom.Services.GetRequiredService<IOptions<BookingOptions>>().Value;

        Assert.Equal(14, options.MaxNights);
    }

    [Fact]
    public void InvalidOptions_FailOnStartup()
    {
        using var broken = factory.WithWebHostBuilder(b => b.UseSetting("Booking:MaxNights", "0"));

        Assert.Throws<OptionsValidationException>(() => broken.CreateClient());
    }
}

namespace HotelBooking.Api.Features.Booking;

public static class BookingFeatureExtensions
{
    public static IServiceCollection AddBookingFeature(
        this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<BookingOptions>()
            .Bind(configuration.GetSection(BookingOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}

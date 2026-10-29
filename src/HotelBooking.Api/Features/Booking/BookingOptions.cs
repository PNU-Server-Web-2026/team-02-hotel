using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Api.Features.Booking;

/// <summary>
/// Параметри бізнес-правил бронювання.
/// </summary>
public sealed class BookingOptions
{
    public const string SectionName = "Booking";

    /// <summary>Максимальна тривалість проживання, ночей.</summary>
    [Range(1, 365)]
    public int MaxNights { get; set; } = 30;

    /// <summary>Через скільки хвилин скасовується непідтверджене бронювання.</summary>
    [Range(1, 1440)]
    public int PendingTimeoutMinutes { get; set; } = 30;

    /// <summary>За скільки годин до заїзду скасування безкоштовне.</summary>
    [Range(0, 720)]
    public int FreeCancellationHours { get; set; } = 48;

    /// <summary>Від скількох ночей діє знижка за довге проживання.</summary>
    [Range(1, 365)]
    public int LongStayNights { get; set; } = 7;

    /// <summary>Розмір знижки за довге проживання, %.</summary>
    [Range(0, 100)]
    public decimal LongStayDiscountPercent { get; set; } = 10;
}

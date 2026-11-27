using HotelBooking.Api.Data;
using HotelBooking.Api.Features.Amenities;
using HotelBooking.Api.Features.Booking;
using HotelBooking.Api.Features.Guests;
using HotelBooking.Api.Features.Health;
using HotelBooking.Api.Features.Hotels;
using HotelBooking.Api.Features.Reviews;
using HotelBooking.Api.Features.Rooms;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Сервіси. Реєстрацію сервісів конкретних фіч додавайте через extension-методи
// у папці фічі (наприклад, builder.Services.AddHotelsFeature();), див. CONTRIBUTING.md.
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

builder.Services.AddGuestsFeature();
builder.Services.AddBookingFeature(builder.Configuration);
builder.Services.AddAmenitiesFeature();
builder.Services.AddHealthFeature();
builder.Services.AddHotelsFeature();
builder.Services.AddReviewsFeature();
builder.Services.AddRoomsFeature();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

// HTTP-конвеєр.
if (app.Environment.IsDevelopment())
{
    // OpenAPI-документ: /openapi/v1.json
    app.MapOpenApi();
}

app.MapHealthFeature();
app.MapControllers();

app.Run();

// Потрібно для інтеграційних тестів (WebApplicationFactory<Program>).
public partial class Program;

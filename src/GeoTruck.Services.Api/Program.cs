using GeoTruck.Services.CrossCutting.Extensions;
using GeoTruck.Services.CrossCutting.Middlewares;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMediatorExtensions();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowedOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContextExtensions(builder.Configuration);
builder.Services.AddRepositoryService();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options
        .WithTitle("API Documentation")
        .WithTheme(ScalarTheme.Default)
        .WithDarkMode());
}

// app.MapScalarApiReference("documentation"); // Removed because no such method exists for WebApplication
app.UseMiddleware<GlobalExceptionHandler>();
app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AllowedOrigins");
app.MapControllers();

app.Run();

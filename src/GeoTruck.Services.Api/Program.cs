using GeoTruck.Services.CrossCutting.Extensions;
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
// Erros em application/problem+json (RFC 9457). As exceções do domínio viram 400, 404, 409 ou 422;
// as demais viram 500 sem expor a mensagem.
builder.Services.AddDomainProblemDetails();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContextExtensions(builder.Configuration);
builder.Services.AddRepositoryService();

var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options
        .WithTitle("API Documentation")
        .WithTheme(ScalarTheme.Default)
        .EnableDarkMode());
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AllowedOrigins");
app.MapControllers();

app.Run();

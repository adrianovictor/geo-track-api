using System.Net;
using System.Text.Json;
using GeoTruck.Services.Domain.Exceptions;
using Microsoft.AspNetCore.Http;

namespace GeoTruck.Services.CrossCutting.Middlewares;

public class GlobalExceptionHandler(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            //_logger.LogError(ex, "Ocorreu uma exceção não tratada");
            await HandleExceptionAsync(context, ex);
        }
    }

   private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            success = false,
            message = "Erro interno do servidor",
            details = (string?)null
        };

        switch (exception)
        {
            case DomainException domainEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new { success = false, message = domainEx.Message, details = (string?)null };
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }        
}

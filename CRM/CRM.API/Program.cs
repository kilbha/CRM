using CRM.Infrastructure;
using CRM.Infrastructure.Identity;
using CRM.API.Extensions;
using Serilog;
using CRM.Application.Interfaces.Repositories;
using CRM.Infrastructure.Repositories;
using CRM.Application.Interfaces.Generators;
using CRM.Infrastructure.Generators;
using CRM.Application.Interfaces.Services;
using CRM.Infrastructure.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddSwaggerDocumentation();

builder.Services.AddInfrastructure(builder.Configuration);



// Logger configuration
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // Connects Swagger UI to the native .NET 9 OpenAPI JSON endpoint
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "CRM API v1");
        options.RoutePrefix = "swagger"; // Access it at /swagger
    });
}

// Seed the database with initial data
await app.SeedDatabaseAsync();

app.UseSwagger();

app.UseSwaggerUI();

app.UseGlobalExceptionMiddleware();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

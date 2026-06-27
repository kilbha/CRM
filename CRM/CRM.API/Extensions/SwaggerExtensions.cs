using Microsoft.OpenApi.Models;

namespace CRM.API.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(
        this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Title = "CRM API",
                    Version = "v1",
                    Description = "Enterprise CRM Backend API"
                });

            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                BearerFormat = "JWT",

                Name = "Authorization",

                In = ParameterLocation.Header,

                Type = SecuritySchemeType.Http,

                Scheme = "bearer",

                Description =
                    "Enter JWT Token only. Example: eyJhbGc...",

                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };

            options.AddSecurityDefinition(
                "Bearer",
                jwtSecurityScheme);

            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        jwtSecurityScheme,
                        Array.Empty<string>()
                    }
                });
        });

        return services;
    }
}
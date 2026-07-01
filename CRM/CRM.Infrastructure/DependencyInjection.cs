using CRM.Infrastructure.Identity;
using CRM.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CRM.Infrastructure.Authentication;
using CRM.Infrastructure.Configuration;
using CRM.Application.Features.Authentication.Interfaces;
using MyIdentityConstants = CRM.Infrastructure.Identity.IdentityConstants;
using CRM.Application.Common.Authorization;
using System.Security.Claims;
using CRM.Application.Interfaces.Repositories;
using CRM.Infrastructure.Repositories;
using CRM.Application.Interfaces.Generators;
using CRM.Infrastructure.Generators;
using CRM.Application.Interfaces.Services;
using CRM.Infrastructure.Services;

namespace CRM.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;

            options.Lockout.MaxFailedAccessAttempts = 5;

            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        services.Configure<JwtSettings>(
            configuration.GetSection(JwtSettings.SectionName));

        var jwtSettings = configuration
            .GetSection(JwtSettings.SectionName)
            .Get<JwtSettings>()!;

        services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuer = false,

                    ValidateAudience = false,

                    ValidateLifetime = true,

                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSettings.Issuer,

                    ValidAudience = jwtSettings.Audience,

                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    // NameClaimType = ClaimTypes.Name,
                    // RoleClaimType = ClaimTypes.Role
                };

            
        });


        services.AddScoped<IJwtTokenGenerator,
                  JwtTokenGenerator>();


        services.AddScoped<IdentitySeeder>();

        services.Configure<SeedDataSettings>(
            configuration.GetSection(SeedDataSettings.SectionName));


        services.AddScoped<IAuthService, AuthService>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                AuthorizationPolicies.RequireSuperAdmin,
                policy =>
                    policy.RequireRole(
                        MyIdentityConstants.SuperAdmin));

            options.AddPolicy(
                AuthorizationPolicies.RequireAdmin,
                policy =>
                    policy.RequireRole(
                        MyIdentityConstants.Admin,
                        MyIdentityConstants.SuperAdmin));

            options.AddPolicy(
                AuthorizationPolicies.RequireSales,
                policy =>
                    policy.RequireRole(
                        MyIdentityConstants.SalesManager,
                        MyIdentityConstants.SalesExecutive));

            options.AddPolicy(
                AuthorizationPolicies.RequireSupport,
                policy =>
                    policy.RequireRole(
                        MyIdentityConstants.SupportExecutive));
        });

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ICustomerCodeGenerator, CustomerCodeGenerator>();
        services.AddScoped<ICustomerService, CustomerService>();


        return services;
    }
}
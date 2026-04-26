using Infrasructure.Auth;
using Infrasructure.MongoDb;
using Infrasructure.Notifications;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Infrasructure.MongoDb;
using System.Text;

namespace Infrasructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDb"));
        services.Configure<EmailSettings>(configuration.GetSection("Email"));

        services.AddTransient<IJwtTokenService, JwtTokenService>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddSingleton<IRefreshTokenRepository, MongoRefreshTokenRepository>();

        var jwtSettings = new JwtSettings();
        configuration.Bind("Jwt", jwtSettings);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
                };
            });

        return services;
    }
}
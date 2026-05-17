using Infrasructure.Auth;
using Infrasructure.MongoDb;
using Infrasructure.Notifications;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Infrasructure.Auth;
using PsychoSupCenterBackend.Infrasructure.MongoDb;
using PsychoSupCenterBackend.Infrasructure.Photos;
using System.Text;

namespace Infrasructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDb"));
        services.Configure<EmailSettings>(configuration.GetSection("Email"));
        services.Configure<CloudinarySettings>(configuration.GetSection("Cloudinary"));

        services.AddTransient<IJwtTokenService, JwtTokenService>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddScoped<IPhotoService, PhotoService>();
        services.AddSingleton<IRefreshTokenRepository, MongoRefreshTokenRepository>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();
        services.AddSingleton<IUserIdProvider, SignalRUserIdProvider>();

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

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }
}
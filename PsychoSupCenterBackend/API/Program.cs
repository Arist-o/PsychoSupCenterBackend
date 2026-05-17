using Infrasructure;
using Microsoft.AspNetCore.Identity;

using PsychoSupCenterBackend.API.Middleware;
using PsychoSupCenterBackend.Application;
using PsychoSupCenterBackend.Application.Chat.Hubs;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Domain.Entities;
using PsychoSupCenterBackend.Infrasructure.Photos;
using PsychoSupCenterBackend.Persistence;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.SignalR;
using PsychoSupCenterBackend.Infrasructure.Auth;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3} {SourceContext} | {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() 
                     ?? ["http://localhost:3000", "http://localhost:5173"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
         {
             options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
         });
builder.Services.AddEndpointsApiExplorer();



builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, SignalRUserIdProvider>();
builder.Services.AddScoped<IPasswordHasher<ApplicationUser>, PasswordHasher<ApplicationUser>>();
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

builder.Services.AddScoped<IPhotoService, PhotoService>();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await DbInitializer.SeedAsync(scope.ServiceProvider);
}

app.UseMiddleware<ExceptionHandlingMiddleware>();


app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

app.Run();
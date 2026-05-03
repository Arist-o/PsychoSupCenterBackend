using Infrasructure; 
using Microsoft.AspNetCore.Identity;
using PsychoSupCenterBackend.Application;
using PsychoSupCenterBackend.Application.Chat.Hubs;
using PsychoSupCenterBackend.Domain.Entities;
using PsychoSupCenterBackend.Persistence;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration); 
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddSignalR();
builder.Services.AddScoped<IPasswordHasher<ApplicationUser>, PasswordHasher<ApplicationUser>>();

var app = builder.Build();


app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

app.Run();
using Microsoft.AspNetCore.Identity;
using PsychoSupCenterBackend.Application;
using Infrasructure; 
using PsychoSupCenterBackend.Persistence;
using PsychoSupCenterBackend.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration); 
builder.Services.AddPersistence(builder.Configuration);

builder.Services.AddScoped<IPasswordHasher<ApplicationUser>, PasswordHasher<ApplicationUser>>();

var app = builder.Build();


app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
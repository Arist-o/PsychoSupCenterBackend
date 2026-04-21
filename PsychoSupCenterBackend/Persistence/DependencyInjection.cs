using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Persistence.Context;
using PsychoSupCenterBackend.Persistence.Repositories;

namespace PsychoSupCenterBackend.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql =>
                {
                    sql.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);

                    sql.MigrationsAssembly(
                        typeof(AppDbContext).Assembly.FullName);
                }));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
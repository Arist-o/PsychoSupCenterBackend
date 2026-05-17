using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PsychoSupCenterBackend.Domain.Entities;
using PsychoSupCenterBackend.Domain.Enums;
using PsychoSupCenterBackend.Persistence.Context;

namespace PsychoSupCenterBackend.Persistence;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<ApplicationUser>>();

        await context.Database.MigrateAsync();

        if (!await context.ApplicationUsers.AnyAsync())
        {
            var adminUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = "admin@admin.com",
                FirstName = "System",
                LastName = "Admin",
                Age = 35,
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Admin123!");
            await context.ApplicationUsers.AddAsync(adminUser);
            
            var spec1 = new DoctorSpecialization { Id = Guid.NewGuid(), Name = "Клінічний психолог", Description = "Діагностика та терапія розладів." };
            var spec2 = new DoctorSpecialization { Id = Guid.NewGuid(), Name = "Психотерапевт", Description = "Довготривала терапія та робота з травмами." };
            var spec3 = new DoctorSpecialization { Id = Guid.NewGuid(), Name = "Сімейний психолог", Description = "Робота з парами та сімейними кризами." };
            await context.DoctorSpecializations.AddRangeAsync(spec1, spec2, spec3);

            var doctorUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = "doctor@test.com",
                FirstName = "Іван",
                LastName = "Коваленко",
                Age = 40,
                Role = UserRole.Doctor,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            doctorUser.PasswordHash = passwordHasher.HashPassword(doctorUser, "Doctor123!");
            var doctorProfile = new DoctorProfile
            {
                Id = Guid.NewGuid(),
                UserId = doctorUser.Id,
                CareerStartDate = DateTime.UtcNow.AddYears(-10),
                Status = DoctorStatus.Active,
                AverageRating = 4.8,
                Bio = "Досвідчений психотерапевт",
                Specializations = [spec1, spec2]
            };
            await context.ApplicationUsers.AddAsync(doctorUser);
            await context.DoctorProfiles.AddAsync(doctorProfile);

            var patientUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = "patient@test.com",
                FirstName = "Олена",
                LastName = "Петренко",
                Age = 25,
                Role = UserRole.Patient,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            patientUser.PasswordHash = passwordHasher.HashPassword(patientUser, "Patient123!");
            var patientProfile = new PatientProfile
            {
                Id = Guid.NewGuid(),
                UserId = patientUser.Id,
                DateOfBirth = DateTime.UtcNow.AddYears(-25),
                Type = PatientType.Standard
            };
            await context.ApplicationUsers.AddAsync(patientUser);
            await context.PatientProfiles.AddAsync(patientProfile);

            await context.SaveChangesAsync();
        }
    }
}
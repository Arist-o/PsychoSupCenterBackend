// Application/Common/Interfaces/IUnitOfWork.cs
using Microsoft.EntityFrameworkCore.Storage;
using PsychoSupCenterBackend.Domain.Entities;

namespace PsychoSupCenterBackend.Application.Common.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
    IRepository<ApplicationUser> Users { get; }
    IRepository<DoctorProfile> DoctorProfiles { get; }
    IRepository<PatientProfile> PatientProfiles { get; }
    IRepository<DoctorService> DoctorServices { get; }
    IRepository<DoctorCertificate> DoctorCertificates { get; }
    IRepository<DoctorAvailability> DoctorAvailabilities { get; }
    IRepository<DoctorUnavailability> DoctorUnavailabilities { get; }
    IRepository<DoctorSpecialization> DoctorSpecializations { get; }
    IRepository<Appointment> Appointments { get; }
    IRepository<Billing> Billings { get; }
    IRepository<ChatRoom> ChatRooms { get; }
    IRepository<ChatParticipant> ChatParticipants { get; }
    IRepository<ChatMessage> ChatMessages { get; }
    IRepository<Review> Reviews { get; }
    IRepository<PsychologicalTest> PsychologicalTests { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    IExecutionStrategy CreateExecutionStrategy();
}
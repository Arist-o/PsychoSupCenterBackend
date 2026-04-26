
using Microsoft.EntityFrameworkCore.Storage;

using AppointmentEntity = PsychoSupCenterBackend.Domain.Entities.Appointment;
using BillingEntity = PsychoSupCenterBackend.Domain.Entities.Billing;
using ChatRoomEntity = PsychoSupCenterBackend.Domain.Entities.ChatRoom;
using ChatMessageEntity = PsychoSupCenterBackend.Domain.Entities.ChatMessage;
using ChatParticipantEntity = PsychoSupCenterBackend.Domain.Entities.ChatParticipant;
using ReviewEntity = PsychoSupCenterBackend.Domain.Entities.Review;
using DoctorServiceEntity = PsychoSupCenterBackend.Domain.Entities.DoctorService;
using PsychoSupCenterBackend.Domain.Entities;

namespace PsychoSupCenterBackend.Application.Common.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
    IRepository<ApplicationUser> Users { get; }
    IRepository<DoctorProfile> DoctorProfiles { get; }
    IRepository<PatientProfile> PatientProfiles { get; }
    IRepository<DoctorServiceEntity> DoctorServices { get; }
    IRepository<DoctorCertificate> DoctorCertificates { get; }
    IRepository<DoctorAvailability> DoctorAvailabilities { get; }
    IRepository<DoctorUnavailability> DoctorUnavailabilities { get; }
    IRepository<DoctorSpecialization> DoctorSpecializations { get; }
    IRepository<AppointmentEntity> Appointments { get; }
    IRepository<BillingEntity> Billings { get; }
    IRepository<ChatRoomEntity> ChatRooms { get; }
    IRepository<ChatParticipantEntity> ChatParticipants { get; }
    IRepository<ChatMessageEntity> ChatMessages { get; }
    IRepository<ReviewEntity> Reviews { get; }
    IRepository<PsychologicalTest> PsychologicalTests { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    IExecutionStrategy CreateExecutionStrategy();
}
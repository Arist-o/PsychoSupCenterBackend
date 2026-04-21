using Microsoft.EntityFrameworkCore.Storage;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Domain.Entities;
using PsychoSupCenterBackend.Persistence.Context;

namespace PsychoSupCenterBackend.Persistence.Repositories;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _currentTransaction;

    private IRepository<ApplicationUser>? _users;
    private IRepository<DoctorProfile>? _doctorProfiles;
    private IRepository<PatientProfile>? _patientProfiles;
    private IRepository<DoctorService>? _doctorServices;
    private IRepository<DoctorCertificate>? _doctorCertificates;
    private IRepository<DoctorAvailability>? _doctorAvailabilities;
    private IRepository<DoctorUnavailability>? _doctorUnavailabilities;
    private IRepository<DoctorSpecialization>? _doctorSpecializations;
    private IRepository<Appointment>? _appointments;
    private IRepository<Billing>? _billings;
    private IRepository<ChatRoom>? _chatRooms;
    private IRepository<ChatParticipant>? _chatParticipants;
    private IRepository<ChatMessage>? _chatMessages;
    private IRepository<Review>? _reviews;
    private IRepository<PsychologicalTest>? _psychologicalTests;

    public UnitOfWork(AppDbContext context) => _context = context;

    // ── Репозиторії (lazy init) ──────────────────────────────────

    public IRepository<ApplicationUser> Users
        => _users ??= new Repository<ApplicationUser>(_context);

    public IRepository<DoctorProfile> DoctorProfiles
        => _doctorProfiles ??= new Repository<DoctorProfile>(_context);

    public IRepository<PatientProfile> PatientProfiles
        => _patientProfiles ??= new Repository<PatientProfile>(_context);

    public IRepository<DoctorService> DoctorServices
        => _doctorServices ??= new Repository<DoctorService>(_context);

    public IRepository<DoctorCertificate> DoctorCertificates
        => _doctorCertificates ??= new Repository<DoctorCertificate>(_context);

    public IRepository<DoctorAvailability> DoctorAvailabilities
        => _doctorAvailabilities ??= new Repository<DoctorAvailability>(_context);

    public IRepository<DoctorUnavailability> DoctorUnavailabilities
        => _doctorUnavailabilities ??= new Repository<DoctorUnavailability>(_context);

    public IRepository<DoctorSpecialization> DoctorSpecializations
        => _doctorSpecializations ??= new Repository<DoctorSpecialization>(_context);

    public IRepository<Appointment> Appointments
        => _appointments ??= new Repository<Appointment>(_context);

    public IRepository<Billing> Billings
        => _billings ??= new Repository<Billing>(_context);

    public IRepository<ChatRoom> ChatRooms
        => _chatRooms ??= new Repository<ChatRoom>(_context);

    public IRepository<ChatParticipant> ChatParticipants
        => _chatParticipants ??= new Repository<ChatParticipant>(_context);

    public IRepository<ChatMessage> ChatMessages
        => _chatMessages ??= new Repository<ChatMessage>(_context);

    public IRepository<Review> Reviews
        => _reviews ??= new Repository<Review>(_context);

    public IRepository<PsychologicalTest> PsychologicalTests
        => _psychologicalTests ??= new Repository<PsychologicalTest>(_context);


    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

  
    public async Task<IDbContextTransaction> BeginTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is not null)
            throw new InvalidOperationException(
                "Транзакція вже активна. Вкладені транзакції не підтримуються.");

        _currentTransaction =
            await _context.Database.BeginTransactionAsync(cancellationToken);

        return _currentTransaction;
    }

    public async Task CommitTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null)
            throw new InvalidOperationException(
                "Немає активної транзакції для коміту.");

        try
        {
            await _currentTransaction.CommitAsync(cancellationToken);
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task RollbackTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null) return;

        try
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public IExecutionStrategy CreateExecutionStrategy()
        => _context.Database.CreateExecutionStrategy();

 
    public async ValueTask DisposeAsync()
    {
        if (_currentTransaction is not null)
            await _currentTransaction.DisposeAsync();

        await _context.DisposeAsync();
    }
}
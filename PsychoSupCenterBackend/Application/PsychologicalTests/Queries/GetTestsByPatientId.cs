
using MediatR;
using Microsoft.EntityFrameworkCore;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.PsychologicalTests.DTOs;

namespace PsychoSupCenterBackend.Application.PsychologicalTests.Queries;

public static class GetTestsByPatientId
{
    public sealed record Query(Guid PatientProfileId)
        : IQuery<Result<IReadOnlyList<PsychologicalTestResponseDto>>>;

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Result<IReadOnlyList<PsychologicalTestResponseDto>>>
    {
        public async Task<Result<IReadOnlyList<PsychologicalTestResponseDto>>> Handle(
            Query request, CancellationToken cancellationToken)
        {
            var tests = await unitOfWork.PsychologicalTests
                .Query()
                .Include(t => t.Appointment)
                .Where(t => t.Appointment.PatientProfileId == request.PatientProfileId)
                .OrderByDescending(t => t.TakenAt)
                .Select(t => new PsychologicalTestResponseDto(
                    t.Id, t.AppointmentId, t.TestType, t.ResultJson, t.ScoreTotal, t.TakenAt))
                .ToListAsync(cancellationToken);

            return Result<IReadOnlyList<PsychologicalTestResponseDto>>.Success(tests);
        }
    }
}
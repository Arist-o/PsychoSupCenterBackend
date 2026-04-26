
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.PsychologicalTests.DTOs;

namespace PsychoSupCenterBackend.Application.PsychologicalTests.Queries;

public static class GetAllPsychologicalTests
{
    public sealed record Query(int Page = 1, int PageSize = 20)
        : IQuery<Result<IReadOnlyList<PsychologicalTestResponseDto>>>;

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Result<IReadOnlyList<PsychologicalTestResponseDto>>>
    {
        public async Task<Result<IReadOnlyList<PsychologicalTestResponseDto>>> Handle(
            Query request, CancellationToken cancellationToken)
        {
            var all = await unitOfWork.PsychologicalTests.GetAllAsync(cancellationToken);

            var paged = all
                .OrderByDescending(t => t.TakenAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(t => new PsychologicalTestResponseDto(
                    t.Id, t.AppointmentId, t.TestType, t.ResultJson, t.ScoreTotal, t.TakenAt))
                .ToList();

            return Result<IReadOnlyList<PsychologicalTestResponseDto>>.Success(paged);
        }
    }
}
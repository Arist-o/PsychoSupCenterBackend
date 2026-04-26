// Application/PsychologicalTests/Queries/GetPsychologicalTestById.cs
using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.PsychologicalTests.DTOs;

namespace PsychoSupCenterBackend.Application.PsychologicalTests.Queries;

public static class GetPsychologicalTestById
{
    public sealed record Query(Guid TestId)
        : IQuery<Result<PsychologicalTestResponseDto>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator() => RuleFor(x => x.TestId).NotEmpty();
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Result<PsychologicalTestResponseDto>>
    {
        public async Task<Result<PsychologicalTestResponseDto>> Handle(
            Query request, CancellationToken cancellationToken)
        {
            var test = await unitOfWork.PsychologicalTests
                .GetByIdAsync(request.TestId, cancellationToken);

            if (test is null)
                return Result<PsychologicalTestResponseDto>.Failure("Тест не знайдено.");

            return Result<PsychologicalTestResponseDto>.Success(
                new PsychologicalTestResponseDto(
                    test.Id, test.AppointmentId, test.TestType,
                    test.ResultJson, test.ScoreTotal, test.TakenAt));
        }
    }
}
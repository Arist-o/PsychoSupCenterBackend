
using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.PsychologicalTests.DTOs;

namespace PsychoSupCenterBackend.Application.PsychologicalTests.Commands;

public static class UpdatePsychologicalTest
{
    public sealed record Command(Guid TestId, UpdatePsychologicalTestDto Dto)
        : ICommand<Result<PsychologicalTestResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.TestId).NotEmpty();
            RuleFor(x => x.Dto.ScoreTotal).GreaterThanOrEqualTo(0);
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Command, Result<PsychologicalTestResponseDto>>
    {
        public async Task<Result<PsychologicalTestResponseDto>> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var test = await unitOfWork.PsychologicalTests
                .GetByIdAsync(request.TestId, cancellationToken);

            if (test is null)
                return Result<PsychologicalTestResponseDto>.Failure("Тест не знайдено.");

            test.ResultJson = request.Dto.ResultJson;
            test.ScoreTotal = request.Dto.ScoreTotal;
            unitOfWork.PsychologicalTests.Update(test);

            return Result<PsychologicalTestResponseDto>.Success(
                new PsychologicalTestResponseDto(
                    test.Id, test.AppointmentId, test.TestType,
                    test.ResultJson, test.ScoreTotal, test.TakenAt));
        }
    }
}
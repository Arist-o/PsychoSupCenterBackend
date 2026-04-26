
using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;
using PsychoSupCenterBackend.Application.PsychologicalTests.DTOs;
using PsychoSupCenterBackend.Domain.Entities;
using PsychoSupCenterBackend.Domain.Enums;

namespace PsychoSupCenterBackend.Application.PsychologicalTests.Commands;

public static class CreatePsychologicalTest
{
    public sealed record Command(CreatePsychologicalTestDto Dto)
        : ICommand<Result<PsychologicalTestResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Dto.AppointmentId).NotEmpty();
            RuleFor(x => x.Dto.TestType)
                .NotEmpty().MaximumLength(100);
            RuleFor(x => x.Dto.ScoreTotal)
                .GreaterThanOrEqualTo(0);
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Command, Result<PsychologicalTestResponseDto>>
    {
        public async Task<Result<PsychologicalTestResponseDto>> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var appointment = await unitOfWork.Appointments
                .GetByIdAsync(request.Dto.AppointmentId, cancellationToken);

            if (appointment is null)
                return Result<PsychologicalTestResponseDto>.Failure(
                    "Запис на прийом не знайдено.");

            if (appointment.Status != AppointmentStatus.Scheduled
             && appointment.Status != AppointmentStatus.Completed)
                return Result<PsychologicalTestResponseDto>.Failure(
                    "Тест можна призначити лише для активного або завершеного запису.");

            var alreadyExists = await unitOfWork.PsychologicalTests.AnyAsync(
                t => t.AppointmentId == request.Dto.AppointmentId, cancellationToken);

            if (alreadyExists)
                return Result<PsychologicalTestResponseDto>.Failure(
                    "Тест для цього запису вже існує.");

            var test = new PsychologicalTest
            {
                Id = Guid.NewGuid(),
                AppointmentId = request.Dto.AppointmentId,
                TestType = request.Dto.TestType,
                ResultJson = request.Dto.ResultJson,
                ScoreTotal = request.Dto.ScoreTotal,
                TakenAt = DateTime.UtcNow,
            };

            await unitOfWork.PsychologicalTests.AddAsync(test, cancellationToken);

            return Result<PsychologicalTestResponseDto>.Success(
                new PsychologicalTestResponseDto(
                    test.Id, test.AppointmentId, test.TestType,
                    test.ResultJson, test.ScoreTotal, test.TakenAt));
        }
    }
}
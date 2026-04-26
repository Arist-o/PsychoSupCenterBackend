
using FluentValidation;
using MediatR;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;

namespace PsychoSupCenterBackend.Application.PsychologicalTests.Commands;

public static class DeletePsychologicalTest
{
    public sealed record Command(Guid TestId) : ICommand<Result<bool>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator() => RuleFor(x => x.TestId).NotEmpty();
    }

    public sealed class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Command, Result<bool>>
    {
        public async Task<Result<bool>> Handle(
            Command request, CancellationToken cancellationToken)
        {
            var test = await unitOfWork.PsychologicalTests
                .GetByIdAsync(request.TestId, cancellationToken);

            if (test is null)
                return Result<bool>.Failure("Тест не знайдено.");

            unitOfWork.PsychologicalTests.Remove(test);
            return Result<bool>.Success(true);
        }
    }
}
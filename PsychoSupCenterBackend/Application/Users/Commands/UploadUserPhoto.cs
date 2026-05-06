using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using PsychoSupCenterBackend.Application.Common.Behaviors;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;

namespace PsychoSupCenterBackend.Application.Users.Commands;

public static class UploadUserPhoto
{
    public sealed record Command(Guid UserId, IFormFile File) : ICommand<Result<string>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.File).NotNull().WithMessage("Файл не обрано.");
            RuleFor(x => x.File.Length).LessThanOrEqualTo(5 * 1024 * 1024).WithMessage("Розмір файлу не повинен перевищувати 5МБ.");
            RuleFor(x => x.File.ContentType).Must(x => x.StartsWith("image/")).WithMessage("Файл повинен бути зображенням.");
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork, IPhotoService photoService)
        : IRequestHandler<Command, Result<string>>
    {
        public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null) return Result<string>.Failure("Користувача не знайдено.");

            var uploadResult = await photoService.UploadPhotoAsync(request.File);
            if (uploadResult is null) return Result<string>.Failure("Помилка завантаження фото.");

            // Якщо вже було фото, можна його видалити з Cloudinary (опціонально)
            // if (!string.IsNullOrEmpty(user.PhotoUrl)) { ... }

            user.PhotoUrl = uploadResult.Url;
            user.UpdatedAt = DateTime.UtcNow;

            unitOfWork.Users.Update(user);

            return Result<string>.Success(user.PhotoUrl);
        }
    }
}
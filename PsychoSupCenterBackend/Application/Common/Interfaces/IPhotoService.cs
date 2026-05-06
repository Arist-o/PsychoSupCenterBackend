using Microsoft.AspNetCore.Http;
using PsychoSupCenterBackend.Application.Common.Models;

namespace PsychoSupCenterBackend.Application.Common.Interfaces;

public interface IPhotoService
{
    Task<PhotoUploadResult?> UploadPhotoAsync(IFormFile file);
    Task<string> DeletePhotoAsync(string publicId);
}
namespace PsychoSupCenterBackend.Application.Reviews.DTOs;

public sealed record UpdateReviewDto(int Rating, string? Comment, bool IsAnonymous);
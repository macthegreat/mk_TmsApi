using TmsApi.Application.Dtos;

namespace TmsApi.Application.Services;

public interface ICachedCourseService
{
    Task<CourseDto> GetCourseAsync(
        string code,
        CancellationToken ct);

    Task<List<CourseDto>> GetAllCoursesAsync(
        CancellationToken ct);

    Task InvalidateCourseCacheAsync(
        CancellationToken ct);

    //Task<List<CourseDto>>GetAllAsync(CancellationToken ct);
}
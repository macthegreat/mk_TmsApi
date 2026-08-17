using TmsApi.Domain.Entities;
using TmsApi.Application.Dtos;


namespace TmsApi.Application.Services;
public interface ICourseService
{
    Task<Course?> GetByIdAsync(int id, CancellationToken ct);

    Task<bool> CodeExistsAsync(string code, CancellationToken ct);
    Task<Course> CreateAsync(CreateCourseRequest request, CancellationToken ct);
    Task<PagedResponse<CourseResponseDto>> GetCoursesAsync(PagedRequest request, CancellationToken ct);

    
}

using MK_TmsApi.Entities;
using MK_TmsApi.Dtos;


namespace MK_TmsApi.Services;
public interface ICourseService
{
    Task<Course?> GetByIdAsync(int id, CancellationToken ct);

    Task<bool> CodeExistsAsync(string code, CancellationToken ct);
    Task<Course> CreateAsync(CreateCourseRequest request, CancellationToken ct);
    Task<PagedResponse<CourseResponseDto>> GetCoursesAsync(PagedRequest request, CancellationToken ct);

    
}

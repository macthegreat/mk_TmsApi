using TmsApi.Domain.Entities;
using TmsApi.Application.Dtos;
using TmsApi.Application.Courses.Commands;


namespace TmsApi.Application.Services;

public interface ICourseService
{
    Task<Course?> GetByIdAsync(int id, CancellationToken ct);

    Task<bool> CodeExistsAsync(string code, CancellationToken ct);
    Task<Course> CreateAsync(CreateCourseRequest request, CancellationToken ct);
    Task<PagedResponse<CourseResponseDto>> GetCoursesAsync(PagedRequest request, CancellationToken ct);
    Task<Course?> GetByCodeAsync(string code, CancellationToken ct);

    Task<IReadOnlyList<Course>> GetAllAsync(CancellationToken ct);
    Task<bool> UpdateAsync(UpdateCourseCommand command,CancellationToken ct);


}



